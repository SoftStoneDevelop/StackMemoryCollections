﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackGenerators
{
    [Generator]
    public class StackGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var c = (CSharpCompilation)context.Compilation;
            var types = new List<INamedTypeSymbol>();
            GetAllTypes(types, c.Assembly.GlobalNamespace);
            // Build up the source code

            var builder = new StringBuilder();
            builder.Append($@"// <auto-generated/>
using System;
using System.Diagnostics;

namespace GenerSpaces
{{
    public static class PClass
    {{
        public static void HelloFrom(string name) 
        {{
            
");
            foreach (var type in types)
            {
                builder.Append($@"Debug.WriteLine($""{type.Name}"");
");
            }

            builder.Append($@"
            
        }}
    }}
}}
");

            // Add the source code to the compilation
            context.AddSource($"PClass.g.cs", builder.ToString());
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

        public void GetAllTypes(List<INamedTypeSymbol> result, INamespaceOrTypeSymbol symbol)
        {
            var queue = new Queue<INamespaceOrTypeSymbol>();
            queue.Enqueue(symbol);

            while(queue.Count != 0)
            {
                var current = queue.Dequeue();
                if (current is INamedTypeSymbol type && (type.IsValueType || type.IsReferenceType))
                {
                    var hasGeneratedAttribute = 
                        type.GetAttributes()
                        .Where(wh => 
                        wh.AttributeClass.Name == "GenerateDictionaryAttribute" ||
                        wh.AttributeClass.Name == "GenerateListAttribute" ||
                        wh.AttributeClass.Name == "GenerateQueueAttribute" ||
                        wh.AttributeClass.Name == "GenerateStackAttribute"
                        )
                        .Any();

                    if (hasGeneratedAttribute)
                    {
                        result.Add(type);
                    }
                }

                foreach (var child in current.GetMembers())
                {
                    if(child is INamespaceOrTypeSymbol symbolChild)
                    {
                        queue.Enqueue(symbolChild);
                    }
                }
            }
        }
    }
}