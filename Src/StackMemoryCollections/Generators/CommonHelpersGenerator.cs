using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace StackMemoryCollections
{
    public class CommonHelpersGenerator
    {
        public void GenerateCommonHelpers(
            in List<INamedTypeSymbol> typeHelpers,
            in GeneratorExecutionContext context
            )
        {
            if (typeHelpers.Count == 0)
            {
                return;
            }

            var code = $@"
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
";

            context.AddSource($"CommonHelper.g.cs", code);
        }
    }
}