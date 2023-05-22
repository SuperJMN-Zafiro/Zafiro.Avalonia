using System.Linq;
using GlobExpressions;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[AzurePipelines(AzurePipelinesImage.WindowsLatest, ImportSecrets = new[]{ nameof(NuGetApiKey)})]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter] [Secret] readonly string NuGetApiKey;
    
    [Solution]
    readonly Solution Solution;

    [GitRepository]
    readonly GitRepository GitRepository;

    [Parameter("configuration")]
    public string Configuration { get; set; }

    [Parameter("version-suffix")]
    public string VersionSuffix { get; set; }

    [Parameter("publish-framework")]
    public string PublishFramework { get; set; }

    [Parameter("publish-runtime")]
    public string PublishRuntime { get; set; }

    [Parameter("publish-project")]
    public string PublishProject { get; set; }

    [Parameter("publish-self-contained")]
    public bool PublishSelfContained { get; set; } = true;

    AbsolutePath SourceDirectory => RootDirectory / "src";

    AbsolutePath TestsDirectory => RootDirectory / "tests";

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    
    [GitVersion]
    readonly GitVersion GitVersion;
    
    protected override void OnBuildInitialized()
    {
        Configuration = Configuration ?? "Release";
        VersionSuffix = VersionSuffix ?? "";
    }

    Target Print => _ => _
        .Executes(() =>
        {
            Log.Information("GitVersion = {Value}", GitVersion.MajorMinorPatch);
        });
    
    Target Pack => _ => _
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetVersion(GitVersion.NuGetVersion)
                .SetOutputDirectory(ArtifactsDirectory / "NuGet"));
        });
    
    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Executes(() =>
        {
            DotNetNuGetPush(settings => settings
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(NuGetApiKey)
                .CombineWith(
                    (ArtifactsDirectory / "NuGet").GlobFiles("*.nupkg").NotEmpty(), (_, v) => _
                        .SetTargetPath(v)),
                degreeOfParallelism: 5, completeOnFailure: true);
        });
}