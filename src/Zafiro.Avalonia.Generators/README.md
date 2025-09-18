# Zafiro.Avalonia.Generators

Source generator for the Zafiro.Avalonia ecosystem.

What it does:
- Registers ViewModel-View pairs by naming convention and x:DataType discovery
- Emits DI registration helpers for section navigation

How to use (NuGet):
- dotnet add package Zafiro.Avalonia.Generators
- The package includes a buildTransitive props that exposes **/*.axaml as AdditionalFiles automatically.

How to use (local dev in this repo):
- Add a ProjectReference to src/Zafiro.Avalonia.Generators with OutputItemType="Analyzer" and ReferenceOutputAssembly="false".
