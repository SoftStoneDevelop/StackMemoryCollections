using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;

namespace StackMemoryCollections.Helpers
{
    internal static class TypeHelper
    {
        internal static string GetFullNamespace(
            this INamespaceSymbol namespaceSymbol
        )
        {
            var builder = new StringBuilder();
            var nestedStack = new Stack<INamespaceSymbol>();
            var currentNamespace = namespaceSymbol;
            while (currentNamespace != null)
            {
                nestedStack.Push(currentNamespace);
                currentNamespace = currentNamespace.ContainingNamespace;
            }

            while (nestedStack.Count != 0)
            {
                currentNamespace = nestedStack.Pop();
                if (currentNamespace.IsGlobalNamespace)
                {
                    continue;
                }

                builder.Append(currentNamespace.Name);
                if (nestedStack.Count != 0)
                {
                    builder.Append(".");
                }
            }

            return builder.ToString();
        }
    }
}
