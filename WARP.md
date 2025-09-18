# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository (Zafiro.Avalonia).

Scope: repository root of Zafiro.Avalonia (UI library, controls, samples, platform heads).

Tech stack

- .NET 8 for libraries and samples; Android head targets net9.0-android
- Avalonia 11.3.x, ReactiveUI
- Submodule: libs/Zafiro points to the core Zafiro library repo (managed as a git submodule)

Common commands

- Restore
  - dotnet restore Zafiro.Avalonia.sln

- Build
  - Debug: dotnet build Zafiro.Avalonia.sln -c Debug
  - Release: dotnet build Zafiro.Avalonia.sln -c Release

- Run desktop sample(s)
  - dotnet run --project samples/TestApp/TestApp.Desktop/TestApp.Desktop.csproj -c Debug

- Android (net9.0-android)
  - Debug build: dotnet build samples/TestApp/TestApp.Android/TestApp.Android.csproj -c Debug
  - Release AOT (publishing): dotnet publish samples/TestApp/TestApp.Android/TestApp.Android.csproj -c Release
    - The Android head enables AOT + LLVM in Release via csproj properties.

- Browser (WASM)
  - dotnet run --project samples/TestApp/TestApp.Browser/TestApp.Browser.csproj -c Debug

Android AOT + XAML Behaviors

- Always prefer the Avalonia XML namespace URI for behaviors instead of assembly-qualified namespaces in XAML
  - Use xmlns:b="https://github.com/avaloniaui" and tags like b:DropTargetBehavior, b:DragDrop, etc.
  - Do not use xmlns with assembly=... for Avalonia.Xaml.Interactions.* or Xaml.Behaviors.Interactions.* — assembly-qualified loads can fail in trimmed/AOT builds.
- Consolidate behavior packages for Android
  - Prefer the Xaml.Behaviors.* family and avoid mixing Avalonia.Xaml.Interactions.* with Xaml.Behaviors.* in the same app head.
  - Rationale: duplicate assemblies with different identities complicate trimming and packaging.
- Root assemblies for the linker in Release/AOT
  - Add TrimmerRootAssembly entries for Xaml.Behaviors and the Xaml.Behaviors.Interactions.* assemblies used by the app.
  - This prevents the linker from removing assemblies referenced indirectly via XAML.
- Android csproj defaults
  - AndroidMinSdkVersion is 21
  - InvariantGlobalization=true to reduce ICU size and silence globalization warnings

Implementation notes (TestApp.Android)

- The Android head references Xaml.Behaviors.Avalonia and roots the specific Xaml.Behaviors.Interactions.* assemblies for Release/AOT so XmlnsDefinitions resolve behaviors via the Avalonia URI without runtime assembly loads.
- Avoid assembly-qualified XAML namespaces for behaviors; rely on xmlns="https://github.com/avaloniaui" mappings. This keeps APK contents minimal and eliminates FileNotFoundException during startup.
- Note: Rooting the umbrella assembly "Xaml.Behaviors" may fail with IL1032 (not present as a concrete assembly in the meta-package). Keep only the Interactions.* roots in this repo.

Publishing (DotnetDeployer)

- This repo prefers DotnetDeployer for packaging and releases (no NUKE)
- Local usage
  - Install tool (once): dotnet tool install --global DotnetDeployer.Tool
  - Set secrets as env vars in your shell (example)
    - export NUGET_API_KEY={{NUGET_API_KEY}}
  - Dry-run (no push)
    - dotnetdeployer nuget --api-key "$NUGET_API_KEY" --no-push
  - Real publish
    - dotnetdeployer nuget --api-key "$NUGET_API_KEY"
    - Optionally: dotnetdeployer release

Azure Pipelines (CI/CD)

- Location: azure-pipelines.yml
- Triggers: push to master, PRs to master or any branch
- Agent: ubuntu-latest
- Steps summary
  - Full checkout with submodules (fetchDepth: 0)
  - Install .NET 9 SDK (UseDotNet@2)
  - Install Android workload (dotnet workload install android)
    - Note: not strictly required for NuGet-only publishing, but kept for optional app release tasks
  - Install DotnetDeployer.Tool globally and print version
  - Package and publish with DotnetDeployer
    - On master: dotnetdeployer nuget --api-key '$(NuGetApiKey)'
    - On other branches: dotnetdeployer nuget --api-key '$(NuGetApiKey)' --no-push (dry run)
- Secrets/variables
  - Variable group: api-keys
  - Variables used: NuGetApiKey (consumed by DotnetDeployer)
- Optional (commented in YAML): release flow using dotnetdeployer release with GitHub token and Android signing inputs

Source generators: Zafiro.Avalonia.Generators

- What changed (rationale)
  - Previously, the Zafiro.Avalonia package embedded and auto-wired the source generator as an Analyzer. This made the generator run implicitly when consuming Zafiro.Avalonia.
  - Now the generator is decoupled and shipped as its own NuGet package (Zafiro.Avalonia.Generators). Consumers opt in explicitly.
- Packaging
  - src/Zafiro.Avalonia.Generators is IsPackable=true
  - The analyzer assembly is included under analyzers/dotnet/cs in the .nupkg
  - A buildTransitive props file (buildTransitive/Zafiro.Avalonia.Generators.props) exposes **/*.axaml as AdditionalFiles automatically in consuming projects
- Consumption guidelines
  - NuGet (recommended for consumers)
    - Add a PackageReference with PrivateAssets=all and IncludeAssets including analyzers and buildTransitive:
      ```xml path=null start=null
      <ItemGroup>
        <PackageReference Include="Zafiro.Avalonia.Generators">
          <PrivateAssets>all</PrivateAssets>
          <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
      </ItemGroup>
      ```
  - Local development inside this repo
    - Projects can reference the generator project as an Analyzer for fast iteration:
      ```xml path=null start=null
      <ItemGroup>
        <ProjectReference Include="..\Zafiro.Avalonia.Generators\Zafiro.Avalonia.Generators.csproj"
                          OutputItemType="Analyzer"
                          ReferenceOutputAssembly="false" />
      </ItemGroup>
      ```
    - The repo uses the UseLocalZafiroReferences switch to toggle between local ProjectReference (true) and NuGet PackageReference (false). This is already configured in Zafiro.Avalonia.Dialogs and samples/TestApp/TestApp.
- Migration note
  - Zafiro.Avalonia no longer includes the generator in its package and no longer references it as an Analyzer. Any project that requires the generated registrations must explicitly reference Zafiro.Avalonia.Generators as shown above.

Submodule: libs/Zafiro

- This repo includes Zafiro as a git submodule in libs/Zafiro
- To fetch and update:
  - git submodule update --init --recursive
  - To pull latest in submodule: (cd libs/Zafiro && git pull)
  - After updating the submodule, commit the submodule pointer in this repository

Coding guidelines (high level)

- Prefer ReactiveUI.Validation and CSharpFunctionalExtensions; lean functional where sensible
- Do not suffix method names with Async even if they return Task
- Keep code, comments, and commit messages in English
- Don't compile the TestApp.Android unless it's necessary, because it slows down the build
