using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StackMemoryCollections.Helpers;
using System.Collections.Immutable;
using System.Linq;

namespace StackMemoryCollections
{
    [Generator]
    public partial class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //System.Diagnostics.Debugger.Launch();
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                predicate: (s, _) => IsSyntaxTargetForGeneration(s),
                transform: (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(m => m != null);

            var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndClasses,
                (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        {
            if(!(node is ClassDeclarationSyntax) && !(node is StructDeclarationSyntax))
            {
                return false;
            }

            var typeDeclar = (TypeDeclarationSyntax)node;
            if (typeDeclar.Modifiers.Any(wh => wh.IsKind(SyntaxKind.StaticKeyword)))
            {
                return false;
            }

            if (typeDeclar.TypeParameterList?.Parameters.Any() == true)
            {
                return false;
            }

            if (typeDeclar.Modifiers.Any(wh => wh.IsKind(SyntaxKind.AbstractKeyword)))
            {
                return false;
            }

            if (typeDeclar.AttributeLists.Count == 0)
            {
                return false;
            }

            return true;
        }

        static TypeDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            return GetSemanticClassOrStruct(context);
        }

        static TypeDeclarationSyntax GetSemanticClassOrStruct(GeneratorSyntaxContext context)
        {
            if (!(context.Node is ClassDeclarationSyntax) && !(context.Node is StructDeclarationSyntax))
            {
                return null;
            }

            var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;
            foreach (var attributeListSyntax in typeDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    IMethodSymbol attributeSymbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;
                    if (attributeSymbol == null)
                    {
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;

                    if (attributeContainingTypeSymbol.ContainingNamespace.GetFullNamespace().StartsWith("StackMemoryCollections."))
                    {
                        return typeDeclarationSyntax;
                    }
                }
            }

            return null;
        }

        private void Execute(Compilation compilation, ImmutableArray<TypeDeclarationSyntax> types, SourceProductionContext context)
        {
            //System.Diagnostics.Debugger.Launch();
            var processor = new AttributeProcessor();
            processor.FillAllTypes(compilation, types);
            processor.Generate(context);
        }
    }
}