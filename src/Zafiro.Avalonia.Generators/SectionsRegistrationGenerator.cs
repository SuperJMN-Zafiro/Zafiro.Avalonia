using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Zafiro.Avalonia.Generators;

[Generator]
public sealed class SectionsRegistrationGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var sections = FindAnnotatedSections(context).OrderBy(s => s.sortIndex).ToList();

        var sb = new StringBuilder();
        var asm = context.Compilation.AssemblyName ?? "Assembly";
        var safeAsm = new string(asm.Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray());
        sb.AppendLine($"namespace Zafiro.UI.Shell.Utils.SectionsGen.{safeAsm};");
        sb.AppendLine();
        sb.AppendLine("public static class GeneratedSectionRegistrations");
        sb.AppendLine("{");
        // Combined registration: DI + sections
        sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddSectionsFromAttributes(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services, global::Serilog.ILogger? logger = null, global::System.Reactive.Concurrency.IScheduler? scheduler = null)");
        sb.AppendLine("    {");
        sb.AppendLine("        RegisterAnnotatedSections(services);");
        sb.AppendLine("        AddAnnotatedSections(services, logger, scheduler);");
        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine();
        // Terse alias
        sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddZafiroSections(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services, global::Serilog.ILogger? logger = null, global::System.Reactive.Concurrency.IScheduler? scheduler = null)");
        sb.AppendLine("    {");
        sb.AppendLine("        return AddSectionsFromAttributes(services, logger, scheduler);");
        sb.AppendLine("    }");
        sb.AppendLine();
        // DI registrations
        sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection RegisterAnnotatedSections(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
        sb.AppendLine("    {");
        foreach (var s in sections)
        {
            if (s.contractFqn == s.implFqn)
            {
                sb.AppendLine($"        global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddScoped(services, typeof({s.implFqn}));");
            }
            else
            {
                sb.AppendLine($"        global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddScoped(services, typeof({s.contractFqn}), typeof({s.implFqn}));");
            }
        }

        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Sections registrations
        sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddAnnotatedSections(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services, global::Serilog.ILogger? logger = null, global::System.Reactive.Concurrency.IScheduler? scheduler = null)");
        sb.AppendLine("    {");
        sb.AppendLine("        global::Zafiro.UI.Navigation.AddNavigation.RegisterSections(services, builder =>");
        sb.AppendLine("        {");
        foreach (var s in sections)
        {
            var iconSource = s.icon ?? "fa-window-maximize";
            sb.Append("            builder.Add<");
            sb.Append(s.contractFqn);
            sb.Append(">(\"");
            sb.Append(Escape(s.displayName));
            sb.Append("\", new global::Zafiro.UI.Icon { Source = \"");
            sb.Append(Escape(iconSource));
            sb.Append("\" }, true);");
            sb.AppendLine();
        }

        sb.AppendLine("        }, logger: logger, scheduler: scheduler ?? global::ReactiveUI.RxApp.MainThreadScheduler);");
        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("GeneratedSectionRegistrations.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));

        // Emit a global using to bring the unique namespace into scope within this project
        var globalUsing = $"global using Zafiro.UI.Shell.Utils.SectionsGen.{safeAsm};";
        context.AddSource("GeneratedSectionRegistrations.GlobalUsing.g.cs", SourceText.From(globalUsing, Encoding.UTF8));
    }

    private static IEnumerable<(string implFqn, string contractFqn, int sortIndex, string displayName, string? icon)> FindAnnotatedSections(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;
        var attr = compilation.GetTypeByMetadataName("Zafiro.UI.Shell.Utils.SectionAttribute");
        if (attr is null)
            yield break;

        foreach (var type in EnumerateAllTypes(compilation.GlobalNamespace))
        {
            if (type.TypeKind != TypeKind.Class)
                continue;

            var sectionAttr = type.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attr));
            if (sectionAttr is null)
                continue;

            // Defaults
            string? icon = null;
            int sortIndex = 0;
            ITypeSymbol? contract = null;

            // Read ctor args by position: (string? name, string? icon, int sortIndex, Type? contractType)
            var ctorArgs = sectionAttr.ConstructorArguments;
            if (ctorArgs.Length >= 2 && ctorArgs[1].Value is string iconStr)
            {
                icon = iconStr;
            }

            if (ctorArgs.Length >= 3 && ctorArgs[2].Value is int si)
            {
                sortIndex = si;
            }

            if (ctorArgs.Length >= 4 && ctorArgs[3].Value is ITypeSymbol contractSym)
            {
                contract = contractSym;
            }

            var implFqn = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var contractFqn = (contract ?? type).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // Display name: TypeName without "ViewModel" with spaces before caps
            var simple = type.Name;
            var baseName = simple.EndsWith("ViewModel", StringComparison.Ordinal)
                ? simple.Substring(0, simple.Length - "ViewModel".Length)
                : simple;
            var display = ToSpaced(baseName);

            yield return (implFqn, contractFqn, sortIndex, display, icon);
        }
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

    private static string ToSpaced(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        var sb = new StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            var ch = name[i];
            if (i > 0 && char.IsUpper(ch) && !char.IsWhiteSpace(name[i - 1]))
            {
                sb.Append(' ');
            }

            sb.Append(ch);
        }

        return sb.ToString().TrimStart(' ');
    }

    private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}