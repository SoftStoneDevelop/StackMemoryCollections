using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace StackMemoryCollections
{
    [Generator]
    public partial class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var c = (CSharpCompilation)context.Compilation;
            var processor = new AttributeProcessor();
            processor.FillAllTypes(c.Assembly.GlobalNamespace);
            processor.Generate(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}