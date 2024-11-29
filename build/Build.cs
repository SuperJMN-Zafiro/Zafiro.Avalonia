using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Zafiro.Deployment;
using Zafiro.Misc;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    [Parameter("GitHub Authentication Token")] [Secret] readonly string GitHubApiKey;
    [Parameter("NuGet Authentication Token")] [Secret] readonly string NuGetApiKey;
    [Parameter] readonly bool Force;
    [Solution] readonly Solution Solution;
    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository Repository;

    public static int Main() => Execute<Build>(x => x.Publish);

    Target RestoreWorkloads => _ => _
        .Executes(() =>
        {
            DotNetWorkloadRestore(x => x.SetProject(Solution));

            if (!IsWasmToolsInstalled())
            {
                DotNetWorkloadInstall(x => x.SetWorkloadId("wasm-tools"));
            }
        });

    bool IsWasmToolsInstalled()
    {
        var result = ProcessTasks.StartProcess("dotnet", "workload list")
            .AssertZeroExitCode()
            .Output
            .Any(line => line.Text.Contains("wasm-tools"));
        return result;
    }

    Target PublishNugetPackages => d => d
        .Requires(() => NuGetApiKey)
        .DependsOn(RestoreWorkloads)
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch() || Force)
        .Executes(async () =>
        {
            var version = GitVersion.NuGetVersion;
            
            await Deployer.Instance.PublishPackages(PackableProjects, version, NuGetApiKey)
                .TapError(error => Assert.Fail(error.ToString()))
                .LogInfo("Nuget packages published");
        });

    Target PublishSite => d => d
        .Requires(() => GitHubApiKey)
        .DependsOn(RestoreWorkloads)
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch() || Force)
        .Executes(async () =>
        {
            await Solution.AllProjects.TryFirst(project => project.Name.EndsWith(".Browser"))
                .ToResult("Browser project not found")
                .Map(project => project.Path.ToString())
                .Bind(projectPath => Deployer.Instance.PublishAvaloniaAppToGitHubPages(projectPath, "SuperJMN-Zafiro", "SuperJMN-Zafiro.github.io", GitHubApiKey))
                .TapError(error => Assert.Fail(error.ToString()))
                .LogInfo("Site published");
        });

    Target Publish => td => td
        .DependsOn(PublishNugetPackages, PublishSite);

    IEnumerable<string> PackableProjects =>
        Solution.AllProjects
            .Where(x => x.GetProperty<bool>("IsPackable"))
            .Where(x => !(x.Path.ToString().Contains("Test", StringComparison.InvariantCultureIgnoreCase) || x.Path.ToString().Contains("Sample", StringComparison.InvariantCultureIgnoreCase)))
            .Select(x => x.Path.ToString());
}