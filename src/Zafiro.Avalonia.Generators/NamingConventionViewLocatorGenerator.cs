using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Zafiro.Avalonia.Generators;

[Generator]
public class NamingConventionViewLocatorGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var pairs = FindPairs(context);

        // Resolve locator to avoid hardcoding its namespace
        var (locatorFqn, locatorNs) = ResolveLocator(context.Compilation, "NamingConventionGeneratedViewLocator");

        var sb = new StringBuilder();
        sb.AppendLine($"namespace {locatorNs};");
        sb.AppendLine();
        sb.AppendLine("file static class NamingConventionGeneratedViewLocator_GlobalRegistrations");
        sb.AppendLine("{");
        sb.AppendLine("    [global::System.Runtime.CompilerServices.ModuleInitializer]");
        sb.AppendLine("    internal static void Initialize()");
        sb.AppendLine("    {");
        foreach (var pair in pairs)
        {
            sb.AppendLine($"        {locatorFqn}.RegisterGlobal<{pair.vm}, {pair.view}>();");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("NamingConventionGeneratedViewLocator.GlobalRegistrations.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static IEnumerable<(string vm, string view)> FindPairs(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;
        var controlType = compilation.GetTypeByMetadataName("Avalonia.Controls.Control");
        if (controlType is null)
        {
            yield break;
        }

        var pairs = new List<(string vm, string view)>();
        foreach (var (vmSymbol, viewSymbol) in EnumerateCandidates(compilation, controlType))
        {
            var vmName = vmSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var viewName = viewSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            pairs.Add((vmName, viewName));
        }

        foreach (var group in pairs.GroupBy(p => p.vm))
        {
            yield return group.First();
        }
    }

    private static IEnumerable<(INamedTypeSymbol vm, INamedTypeSymbol view)> EnumerateCandidates(Compilation compilation, INamedTypeSymbol controlType)
    {
        foreach (var vm in EnumerateAllTypes(compilation.GlobalNamespace))
        {
            if (vm.TypeKind != TypeKind.Class)
                continue;

            if (!vm.Name.EndsWith("ViewModel", StringComparison.Ordinal))
                continue;

            var ns = vm.ContainingNamespace;
            if (ns is null)
                continue;

            var baseName = vm.Name.Substring(0, vm.Name.Length - "ViewModel".Length);
            var viewCandidates = ns.GetTypeMembers(baseName + "View");
            if (viewCandidates.Length == 0)
                continue;

            foreach (var viewCandidate in viewCandidates)
            {
                if (viewCandidate.TypeKind != TypeKind.Class)
                    continue;

                if (IsDerivedFrom(viewCandidate, controlType))
                {
                    yield return (vm, viewCandidate);
                    break;
                }
            }
        }
    }

    private static bool IsDerivedFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var t = type; t != null; t = t.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(t, baseType))
                return true;
        }

        return false;
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateAllTypes(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            yield return type;

            foreach (var nested in EnumerateNestedTypes(type))
                yield return nested;
        }

        foreach (var sub in ns.GetNamespaceMembers())
        {
            foreach (var t in EnumerateAllTypes(sub))
                yield return t;
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateNestedTypes(INamedTypeSymbol type)
    {
        foreach (var nested in type.GetTypeMembers())
        {
            yield return nested;
            foreach (var deeper in EnumerateNestedTypes(nested))
                yield return deeper;
        }
    }

    private static (string locatorFqn, string locatorNs) ResolveLocator(Compilation compilation, string simpleName)
    {
        var candidates = new[]
        {
            $"Zafiro.Avalonia.ViewLocators.{simpleName}",
            $"Zafiro.Avalonia.Misc.{simpleName}"
        };

        INamedTypeSymbol? symbol = null;
        foreach (var md in candidates)
        {
            symbol = compilation.GetTypeByMetadataName(md);
            if (symbol is not null) break;
        }

        if (symbol is null)
        {
            return ($"global::Zafiro.Avalonia.ViewLocators.{simpleName}", "Zafiro.Avalonia.ViewLocators");
        }

        var fqn = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var ns = symbol.ContainingNamespace?.ToDisplayString() ?? "Zafiro.Avalonia.ViewLocators";
        return (fqn, ns);
    }
}