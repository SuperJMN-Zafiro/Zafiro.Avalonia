using System;
using System.Diagnostics;
using System.Linq;
using CSharpFunctionalExtensions;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Octokit;
using Serilog;
using Zafiro.FileSystem.Core;
using Zafiro.Nuke;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    [Parameter("GitHub Authentication Token")] [Secret] readonly string GitHubAuthenticationToken;
    [Parameter("NuGet Authentication Token")] [Secret] readonly string NuGetApiKey;

    [Solution] readonly Solution Solution;

    [Parameter("version-suffix")] public string VersionSuffix { get; set; }

    [Parameter("publish-framework")] public string PublishFramework { get; set; }

    [Parameter("publish-runtime")] public string PublishRuntime { get; set; }

    [Parameter("publish-project")] public string PublishProject { get; set; }

    [Parameter("publish-self-contained")] public bool PublishSelfContained { get; set; } = true;

    [GitVersion] readonly GitVersion GitVersion;

    [GitRepository] readonly GitRepository Repository;

    [Parameter]
    readonly Configuration Configuration = IsServerBuild
        ? Configuration.Release
        : Configuration.Debug;

    Actions Actions;

    AbsolutePath OutputDirectory => RootDirectory / "output";

    protected override void OnBuildInitialized()
    {
        Actions = new Actions(Solution, Repository, RootDirectory, GitVersion, Configuration);
    }
    
    Target Clean => _ => _
        .Executes(() =>
        {
            OutputDirectory.CreateOrCleanDirectory();
            var absolutePaths = RootDirectory.GlobDirectories("**/bin", "**/obj").Where(a => !((string) a).Contains("build")).ToList();
            Log.Information("Deleting {Dirs}", absolutePaths);
            absolutePaths.DeleteDirectories();
        });

    Target RestoreWorkloads => td => td
        .Executes(() =>
        {
            DotNetWorkloadRestore(x => x.SetProject(Solution));
        });

    Target PublishNugetPackages => d => d
        .Requires(() => NuGetApiKey)
        .DependsOn(Clean)
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch())
        .Executes(() =>
        {
            Actions.PushNuGetPackages(NuGetApiKey)
                .TapError(error => throw new ApplicationException(error));
        });

    Target PublishSite => d => d
        .Requires(() => GitHubAuthenticationToken)
        .DependsOn(Clean)
        .DependsOn(RestoreWorkloads)
        .Executes(async () =>
        {
            var client = new GitHubClient(new ProductHeaderValue("Zafiro.Avalonia"))
            {
                Credentials = new Credentials(GitHubAuthenticationToken),
            };

            var github = new GitHub(client, "SuperJMN-Zafiro.github.io", "SuperJMN-Zafiro");

            var project = Solution.AllProjects.TryFirst(project => project.Name.EndsWith(".Browser"))
                .ToResult("Browser project not found");

            await project
                .Map(project1 => (ZafiroPath)project1.Path.ToString())
                .Bind(zafiroPath => github.PublishToPages(zafiroPath))
                .TapError(error => throw new ApplicationException(error));
        });

    Target Publish => td => td
        .DependsOn(PublishNugetPackages, PublishSite);

    public static int Main() => Execute<Build>(x => x.Publish);
}