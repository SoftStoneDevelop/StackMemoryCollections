using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;

namespace StackMemoryCollections
{
    public partial class Generator
    {
        private void GenerateCommonHelpers(
            in List<INamedTypeSymbol> typeHelpers,
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            if (typeHelpers.Count == 0)
            {
                return;
            }
            builder.Clear();
            builder.Append($@"
/*
{Resource.License}
*/

using System;
using System.Runtime.CompilerServices;

namespace StackMemoryCollections
{{
    public unsafe static class CommonHelper
    {{
        public static bool IsNull(in void* ptr)
        {{
            return *((byte*)ptr) == 0;
        }}
    }}
}}
");

            context.AddSource($"CommonHelper.g.cs", builder.ToString());
        }
    }
}