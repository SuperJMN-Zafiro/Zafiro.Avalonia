using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using DotnetPackaging;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Zafiro.Deployment;
using Zafiro.Misc;
using Zafiro.Nuke;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Options = DotnetPackaging.Options;

class Build : NukeBuild
{
    // Android
    [Parameter("The alias for the key in the keystore.")] readonly string AndroidSigningKeyAlias;
    [Parameter("The password of the key within the keystore file.")] [Secret] readonly string AndroidSigningKeyPass;
    [Parameter("The password for the keystore file.")] [Secret] readonly string AndroidSigningStorePass;
    [Parameter("Contents of the keystore encoded as Base64.")] readonly string Base64Keystore;
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [Parameter] readonly bool Force;
    [Parameter("GitHub Authentication Token")] [Secret] readonly string GitHubApiKey;
    [GitVersion] readonly GitVersion GitVersion;
    [Parameter("NuGet Authentication Token")] [Secret] readonly string NuGetApiKey;
    [GitRepository] readonly GitRepository Repository;
    [Solution] readonly Solution Solution;

    Actions Actions { get; set; }

    Target RestoreWorkloads => _ => _
        .Executes(() =>
        {
            DotNetWorkloadRestore(x => x.SetProject(Solution));

            if (!IsWasmToolsInstalled())
            {
                DotNetWorkloadInstall(x => x.SetWorkloadId("wasm-tools"));
            }
        });

    Target PublishNugetPackages => d => d
        .Requires(() => NuGetApiKey)
        .DependsOn(RestoreWorkloads)
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch() || Force)
        .Executes(async () =>
        {
            var version = GitVersion.MajorMinorPatch;

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

    Target PublishGitHubRelease => td => td
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch())
        .Requires(() => GitHubApiKey)
        .Executes(async () =>
        {
            await Solution.Projects
                .TryFirst(x => x.GetOutputType().Contains("Exe", StringComparison.InvariantCultureIgnoreCase))
                .ToResult("Could not find the executable project")
                .Bind(project =>
                {
                    return new DeploymentBuilder(Actions, project)
                        .ForLinux(Options())
                        .ForWindows()
                        .ForAndroid(Base64Keystore, AndroidSigningKeyAlias, AndroidSigningKeyPass, AndroidSigningStorePass)
                        .Build()
                        .Bind(paths => Actions.CreateGitHubRelease(GitHubApiKey, paths.ToArray()));
                })
                .TapError(err => throw new ApplicationException(err));
        });

    Target Publish => td => td
        .DependsOn(PublishNugetPackages, PublishSite, PublishGitHubRelease);

    IEnumerable<string> PackableProjects =>
        Solution.AllProjects
            .Where(x => x.GetProperty<bool>("IsPackable"))
            .Where(x => !(x.Path.ToString().Contains("Test", StringComparison.InvariantCultureIgnoreCase) || x.Path.ToString().Contains("Sample", StringComparison.InvariantCultureIgnoreCase)))
            .Select(x => x.Path.ToString());

    protected override void OnBuildInitialized()
    {
        Actions = new Actions(Solution, Repository, RootDirectory, GitVersion, Configuration);
    }

    public static int Main() => Execute<Build>(x => x.Publish);

    bool IsWasmToolsInstalled()
    {
        var result = ProcessTasks.StartProcess("dotnet", "workload list")
            .AssertZeroExitCode()
            .Output
            .Any(line => line.Text.Contains("wasm-tools"));
        return result;
    }

    Options Options()
    {
        IEnumerable<AdditionalCategory> additionalCategories = [];

        return new Options
        {
            MainCategory = MainCategory.Development,
            AdditionalCategories = Maybe.From(additionalCategories),
            Name = "Zafiro.Avalonia Toolkit Catalog",
            Version = GitVersion.MajorMinorPatch,
            Comment = "Catalog of controls and other utilities in Zafiro.Avalonia",
            Id = "com.SuperJMN.Zafiro.Avalonia",
            StartupWmClass = "Zafiro.Avalonia",
            HomePage = new Uri("https://github.com/SuperJMN-Zafiro/Zafiro.Avalonia"),
            Keywords = new List<string>
            {
                "Toolkit",
                "Avalonia",
                "UI",
                "UI Framework",
                "Cross-platform",
                "Multiplatform",
                "Productivity",
                "Library",
            },
            License = "MIT",
            //ScreenshotUrls = Maybe.From(screenShots),
            Summary = "Shows a catalog of controls and other utilities in Zafiro.Avalonia",
        };
    }
}