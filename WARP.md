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

Publishing (DotnetDeployer)

- This repo prefers DotnetDeployer for packaging and releases (no NUKE)
- Install tool (once): dotnet tool install --global DotnetDeployer.Tool
- Set secrets as env vars in your shell (example)
  - export NUGET_API_KEY={{NUGET_API_KEY}}
- Dry-run (no push)
  - dotnetdeployer nuget --api-key "$NUGET_API_KEY" --no-push
- Real publish
  - dotnetdeployer nuget --api-key "$NUGET_API_KEY"
  - Optionally: dotnetdeployer release

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

