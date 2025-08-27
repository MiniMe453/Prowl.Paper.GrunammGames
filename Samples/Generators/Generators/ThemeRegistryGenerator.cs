// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Prowl.PaperUI;

[Generator]
public sealed class ThemeRegistryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Pick out classes with attributes
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
                transform: static (ctx, _) => GetCandidateClass(ctx))
            .Where(static symbol => symbol is not null);

        // Collect them together
        var allClasses = classDeclarations.Collect();

        // Generate registry when we have symbols
        context.RegisterSourceOutput(allClasses, (spc, classes) =>
        {
            var typeRegistry = new List<(string Name, string FullTypeName)>();

            foreach (var classSymbol in classes!)
            {
                if (classSymbol == null)
                    continue;

                var attribute = classSymbol.GetAttributes()
                    .FirstOrDefault(attr =>
                        attr.AttributeClass?.Name == "ThemePaletteAttribute" ||
                        attr.AttributeClass?.ToDisplayString().EndsWith(".ThemePaletteAttribute") == true);

                if (attribute != null)
                {
                    string name = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString()
                                  ?? classSymbol.Name;

                    typeRegistry.Add((name, classSymbol.ToDisplayString()));
                }
            }

            if (typeRegistry.Count > 0)
            {
                string source = GenerateRegistryClass(typeRegistry);
                spc.AddSource("ThemePaletteRegistry.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        });
    }

    private static INamedTypeSymbol? GetCandidateClass(GeneratorSyntaxContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        return context.SemanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
    }

    private static string GenerateRegistryClass(List<(string Name, string FullTypeName)> types)
    {
        var sb = new StringBuilder();
        sb.AppendLine("public static class ThemePaletteRegistry");
        sb.AppendLine("{");
        sb.AppendLine("    private static readonly (string Name, System.Type Type)[] _types = new (string, System.Type)[]");
        sb.AppendLine("    {");

        foreach (var (name, typeName) in types)
        {
            sb.AppendLine($"        (\"{name}\", typeof({typeName})),");
        }

        sb.AppendLine("    };");
        sb.AppendLine("    public static IReadOnlyList<(string Name, System.Type Type)> Types => _types;");
        sb.AppendLine("}");
        return sb.ToString();
    }
}
