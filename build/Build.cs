using System;
using System.Linq;
using GlobExpressions;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.IO;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

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

    protected override void OnBuildInitialized()
    {
        Configuration = Configuration ?? "Release";
        VersionSuffix = VersionSuffix ?? "";

        InitializeLinuxBuild();
    }

    void InitializeLinuxBuild()
    {
        if (OperatingSystem.IsLinux())
        {
            var iosProjects = Solution.GetAllProjects("*.iOS");
            foreach (var project in iosProjects)
            {
                Console.WriteLine($"Removed project: {project.Name}");
                Solution.RemoveProject(project);
            }
            Solution.Save();
        }
    }

    Target Clean => _ => _
        .Executes(() =>
        {
            var dirs = new[]
                {
                    Glob.Directories(SourceDirectory, "**/bin"),
                    Glob.Directories(SourceDirectory, "**/obj"),
                    Glob.Directories(TestsDirectory, "**/bin"),
                    Glob.Directories(TestsDirectory, "**/obj"),
                }
                .SelectMany(a => a)
                .Select(AbsolutePath.Create);

            dirs.ToList().ForEach(path => path.DeleteDirectory());

            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetVersionSuffix(VersionSuffix)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetLoggers("trx")
                .SetResultsDirectory(ArtifactsDirectory / "TestResults")
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetVersionSuffix(VersionSuffix)
                .SetOutputDirectory(ArtifactsDirectory / "NuGet")
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .DependsOn(Restore)
        .Requires(() => PublishRuntime)
        .Requires(() => PublishFramework)
        .Requires(() => PublishProject)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(Solution.GetProject(PublishProject))
                .SetConfiguration(Configuration)
                .SetVersionSuffix(VersionSuffix)
                .SetFramework(PublishFramework)
                .SetRuntime(PublishRuntime)
                .SetSelfContained(PublishSelfContained)
                .SetOutput(ArtifactsDirectory / "Publish" / PublishProject + "-" + PublishFramework + "-" + PublishRuntime));
        });
}