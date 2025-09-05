using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Zafiro.Avalonia.Generators;

[Generator]
public class DataTypeViewLocatorGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var pairs = FindPairs(context);

        // Resolve target locator symbol to avoid hardcoding its namespace
        var (locatorFqn, locatorNs) = ResolveLocator(context.Compilation, "DataTypeViewLocator");

        var sb = new StringBuilder();
        sb.AppendLine($"namespace {locatorNs};");
        sb.AppendLine();
        sb.AppendLine("file static class DataTypeViewLocator_GlobalRegistrations");
        sb.AppendLine("{");
        sb.AppendLine("    [global::System.Runtime.CompilerServices.ModuleInitializer]");
        sb.AppendLine("    internal static void Initialize()");
        sb.AppendLine("    {");
        foreach (var pair in pairs)
        {
            sb.AppendLine($"        {locatorFqn}.RegisterGlobal<global::{pair.viewModel}, global::{pair.view}>();");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("DataTypeViewLocator.GlobalRegistrations.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static IEnumerable<(string viewModel, string view)> FindPairs(GeneratorExecutionContext context)
    {
        var axamls = context.AdditionalFiles.Where(f => f.Path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase));
        var pairs = new List<(string viewModel, string view)>();

        foreach (var file in axamls)
        {
            var text = file.GetText(context.CancellationToken);
            if (text is null)
            {
                continue;
            }

            var doc = XDocument.Parse(text.ToString());
            var root = doc.Root;
            if (root is null)
            {
                continue;
            }

            var xNs = root.GetNamespaceOfPrefix("x");
            var classAttr = root.Attribute(xNs + "Class")?.Value;
            var dataTypeAttr = root.Attribute(xNs + "DataType")?.Value;
            if (classAttr is null || dataTypeAttr is null)
            {
                continue;
            }

            var (prefix, typeName) = Split(dataTypeAttr);
            var clrNs = root.Attributes()
                .FirstOrDefault(a => a.IsNamespaceDeclaration && a.Name.LocalName == prefix)?.Value;
            var fullVm = ToFullName(clrNs, typeName);
            if (fullVm is null)
            {
                continue;
            }

            pairs.Add((fullVm, classAttr));
        }

        var groups = pairs.GroupBy(p => p.viewModel);
        foreach (var group in groups)
        {
            // Prefer view whose simple name matches the ViewModel simple name without the "ViewModel" suffix, plus "View".
            var vmFull = group.Key;
            var vmSimple = vmFull.Split('.').Last();
            var baseName = vmSimple.EndsWith("ViewModel", StringComparison.Ordinal)
                ? vmSimple.Substring(0, vmSimple.Length - "ViewModel".Length)
                : vmSimple;
            var desiredViewSimple = baseName + "View";

            var chosen = group.FirstOrDefault(p => p.view.Split('.').Last() == desiredViewSimple);
            if (chosen.view is null)
            {
                // Fallback: keep previous behavior (first)
                chosen = group.First();
            }

            var multiple = group.Skip(1).Any();
            if (multiple)
            {
                var descriptor = new DiagnosticDescriptor(
                    id: "ZAV0001",
                    title: "Multiple views for view model",
                    messageFormat: $"Multiple views found for {group.Key}. Using {chosen.view}",
                    category: "ViewLocation",
                    DiagnosticSeverity.Warning,
                    isEnabledByDefault: true);
                context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None));
            }

            yield return chosen;
        }
    }

    private static (string prefix, string name) Split(string value)
    {
        var parts = value.Split(':');
        return parts.Length == 2 ? (parts[0], parts[1]) : ("", value);
    }

    private static string? ToFullName(string? clrNamespace, string name)
    {
        if (clrNamespace is null)
        {
            return null;
        }

        var ns = clrNamespace.Split(';').FirstOrDefault()?.Replace("clr-namespace:", "");
        if (ns is null)
        {
            return null;
        }

        return ns + "." + name;
    }

    private static (string locatorFqn, string locatorNs) ResolveLocator(Compilation compilation, string simpleName)
    {
        // Try common namespaces first
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
            // Fallback to default namespace string
            return ($"global::Zafiro.Avalonia.ViewLocators.{simpleName}", "Zafiro.Avalonia.ViewLocators");
        }

        var fqn = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var ns = symbol.ContainingNamespace?.ToDisplayString() ?? "Zafiro.Avalonia.ViewLocators";
        return (fqn, ns);
    }
}