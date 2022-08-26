using Microsoft.CodeAnalysis;
using System.Text;

namespace TestGenerator
{
    [Generator]
    public partial class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var builder = new StringBuilder();
            GenerateWrapPrimitiveTest(
                in context,
                in builder
                );
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            
        }
    }
}