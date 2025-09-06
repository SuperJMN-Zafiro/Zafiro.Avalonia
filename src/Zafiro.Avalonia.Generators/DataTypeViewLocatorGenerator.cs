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
        var compilation = context.Compilation;
        var axamls = context.AdditionalFiles.Where(f => f.Path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase));
        var pairs = new List<(string viewModel, string view)>();

        foreach (var file in axamls)
        {
            var text = file.GetText(context.CancellationToken);
            if (text is null)
            {
                continue;
            }

            XDocument doc;
            try
            {
                doc = XDocument.Parse(text.ToString());
            }
            catch
            {
                continue;
            }

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
            var vmFullNameFromXaml = ToFullName(clrNs, typeName);
            if (vmFullNameFromXaml is null)
            {
                continue;
            }

            // Resolve the symbol of the x:DataType
            var vmSymbol = compilation.GetTypeByMetadataName(vmFullNameFromXaml);
            string chosenVmFullName = vmFullNameFromXaml;

            if (vmSymbol is INamedTypeSymbol named && named.TypeKind == TypeKind.Interface)
            {
                // Identify the view identifier (SomeView -> Some)
                var viewId = GetViewIdentifier(classAttr);

                // Find implementations of the interface
                var impls = EnumerateAllTypes(compilation.GlobalNamespace)
                    .OfType<INamedTypeSymbol>()
                    .Where(t => t.TypeKind == TypeKind.Class && !t.IsAbstract && Implements(t, named))
                    .OrderBy(t => t.ToDisplayString())
                    .ToList();

                if (impls.Count > 0)
                {
                    // Prefer the implementation whose identifier matches the view identifier (SomeViewModel -> Some)
                    var matching = impls.FirstOrDefault(t => GetViewModelIdentifier(t.Name).Equals(viewId, StringComparison.Ordinal));

                    var chosen = matching ?? impls.First();
                    chosenVmFullName = ToQualifiedName(chosen);

                    if (impls.Count > 1)
                    {
                        var descriptor = new DiagnosticDescriptor(
                            id: "ZAV0002",
                            title: "Multiple implementations for interface",
                            messageFormat: $"Multiple implementations found for interface {named.ToDisplayString()}. Using {chosenVmFullName} for view {classAttr}",
                            category: "ViewLocation",
                            DiagnosticSeverity.Warning,
                            isEnabledByDefault: true);
                        context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None));
                    }

                    if (matching is null && impls.Count >= 1)
                    {
                        var descriptorNoMatch = new DiagnosticDescriptor(
                            id: "ZAV0003",
                            title: "No identifier match for interface implementations",
                            messageFormat: $"No implementation matching identifier '{viewId}' for interface {named.ToDisplayString()} and view {classAttr}. Using {chosenVmFullName}",
                            category: "ViewLocation",
                            DiagnosticSeverity.Warning,
                            isEnabledByDefault: true);
                        context.ReportDiagnostic(Diagnostic.Create(descriptorNoMatch, Location.None));
                    }
                }
                else
                {
                    // No implementations found: keep interface as-is, but warn
                    var descriptorNone = new DiagnosticDescriptor(
                        id: "ZAV0004",
                        title: "No implementations found for interface",
                        messageFormat: $"No implementations found for interface {named.ToDisplayString()} referenced by view {classAttr}. Keeping interface mapping.",
                        category: "ViewLocation",
                        DiagnosticSeverity.Warning,
                        isEnabledByDefault: true);
                    context.ReportDiagnostic(Diagnostic.Create(descriptorNone, Location.None));
                }
            }

            pairs.Add((chosenVmFullName, classAttr));
        }

        // Group by ViewModel and choose a single View when multiple Views share the same VM
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
            if (string.IsNullOrEmpty(chosen.view))
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

    private static bool Implements(INamedTypeSymbol type, INamedTypeSymbol @interface)
    {
        return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, @interface));
    }

    private static string GetViewIdentifier(string viewFullName)
    {
        var simple = viewFullName.Split('.').Last();
        return simple.EndsWith("View", StringComparison.Ordinal)
            ? simple.Substring(0, simple.Length - "View".Length)
            : simple;
    }

    private static string GetViewModelIdentifier(string vmSimpleName)
    {
        return vmSimpleName.EndsWith("ViewModel", StringComparison.Ordinal)
            ? vmSimpleName.Substring(0, vmSimpleName.Length - "ViewModel".Length)
            : vmSimpleName;
    }

    private static string ToQualifiedName(INamedTypeSymbol type)
    {
        var parts = new Stack<string>();
        for (var t = type; t is not null; t = t.ContainingType)
        {
            parts.Push(t.Name);
        }

        var ns = type.ContainingNamespace?.ToDisplayString();
        var name = string.Join(".", parts);
        return string.IsNullOrEmpty(ns) ? name : ns + "." + name;
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
