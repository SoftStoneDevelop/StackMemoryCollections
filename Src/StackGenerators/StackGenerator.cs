using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
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
            var typeHelpers = new List<INamedTypeSymbol>();
            var typeGeneratedStack = new List<INamedTypeSymbol>();
            var typeGeneratedList = new List<INamedTypeSymbol>();
            var typeGeneratedQueue = new List<INamedTypeSymbol>();
            var typeGeneratedDictionary = new List<INamedTypeSymbol>();

            FillAllTypes(
                in typeHelpers,
                in typeGeneratedStack,
                in typeGeneratedList,
                in typeGeneratedQueue,
                in typeGeneratedDictionary,
                c.Assembly.GlobalNamespace
                );
            // Build up the source code

            var infos = new Dictionary<string, TypeInfo>();
            FillTypeInfos(in typeHelpers, in infos);
            GenerateHelpers(in typeHelpers, in context, in infos);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

        private void FillAllTypes(
            in List<INamedTypeSymbol> typesHelpers,
            in List<INamedTypeSymbol> typesGeneratedStack,
            in List<INamedTypeSymbol> typesGeneratedList,
            in List<INamedTypeSymbol> typesGeneratedQueue,
            in List<INamedTypeSymbol> typesGeneratedDictionary,
            INamespaceOrTypeSymbol symbol
            )
        {
            var queue = new Queue<INamespaceOrTypeSymbol>();
            queue.Enqueue(symbol);

            while(queue.Count != 0)
            {
                var current = queue.Dequeue();
                if (current is INamedTypeSymbol type && 
                    !type.IsAbstract &&
                    !type.IsGenericType &&
                    !type.IsStatic &&
                    (type.IsValueType || type.IsReferenceType))
                {
                    var attributes = type.GetAttributes();
                    var hasHelper = attributes.Any(wh => wh.AttributeClass.Name == "GenerateHelperAttribute");
                    var hasStack = attributes.Any(wh => wh.AttributeClass.Name == "GenerateStackAttribute");
                    var hasQueue = attributes.Any(wh => wh.AttributeClass.Name == "GenerateQueueAttribute");
                    var hasList = attributes.Any(wh => wh.AttributeClass.Name == "GenerateListAttribute");
                    var hasDictionary = attributes.Any(wh => wh.AttributeClass.Name == "GenerateDictionaryAttribute");

                    if (hasHelper || hasStack || hasQueue || hasList || hasDictionary)
                    {
                        typesHelpers.Add(type);

                        if (hasStack)
                        {
                            typesGeneratedStack.Add(type);
                        }

                        if (hasList)
                        {
                            typesGeneratedList.Add(type);
                        }

                        if (hasQueue)
                        {
                            typesGeneratedQueue.Add(type);
                        }

                        if (hasDictionary)
                        {
                            typesGeneratedDictionary.Add(type);
                        }
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

        private void FillTypeInfos(
            in List<INamedTypeSymbol> typeHelpers,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            for (int i = 0; i < typeHelpers.Count; i++)
            {
                var stackCurrentTypes = new Stack<INamedTypeSymbol>();
                stackCurrentTypes.Push(typeHelpers[i]);

                while(stackCurrentTypes.Count != 0)
                {
                    var currentType = stackCurrentTypes.Peek();
                    var info = new TypeInfo();
                    var offset = 0;

                    var needSkip = false;
                    foreach (var member in currentType.GetMembers())
                    {
                        if (member is Microsoft.CodeAnalysis.IPropertySymbol propertySymbol)
                        {
                            if (!member.DeclaredAccessibility.HasFlag(Accessibility.Public))
                            {
                                throw new Exception($"Generation is possible only for the public property. Property name: '{member.Name}'");
                            }

                            if (propertySymbol.GetMethod == null)
                            {
                                throw new Exception($"The property must have GetMethod. Property name: '{member.Name}'");
                            }

                            if (propertySymbol.GetMethod.DeclaredAccessibility.HasFlag(Accessibility.Public))
                            {
                                throw new Exception($"GetMethod must be public. Property name: '{member.Name}'");
                            }

                            if (propertySymbol.SetMethod == null)
                            {
                                throw new Exception($"The property must have SetMethod. Property name: '{member.Name}'");
                            }

                            if (propertySymbol.SetMethod.DeclaredAccessibility.HasFlag(Accessibility.Public))
                            {
                                throw new Exception($"SetMethod must be public. Property name: '{member.Name}'");
                            }

                            if (propertySymbol.Type.IsUnmanagedType)
                            {
                                var memberInfo = new MemberInfo();
                                memberInfo.Size = TypeToSize(propertySymbol.Type.Name);
                                memberInfo.TypeName = propertySymbol.Type.Name;
                                memberInfo.MemberName = propertySymbol.Name;
                                memberInfo.Offset = offset;
                                memberInfo.IsUnmanaged = true;
                                offset += memberInfo.Size;
                                info.Members.Add(memberInfo);
                                continue;
                            }

                            if (propertySymbol.Type.IsReferenceType)
                            {
                                throw new Exception($"Nested reference properties are not supported, property name: '{member.Name}'");
                            }

                            if (propertySymbol.Type.IsValueType)
                            {
                                if(typeInfos.TryGetValue($"{propertySymbol.Type.ContainingNamespace}.{propertySymbol.Type.Name}", out var tInfo))
                                {
                                    var memberInfo = new MemberInfo();
                                    memberInfo.Size = tInfo.Members.Sum(s => s.Size);
                                    memberInfo.TypeName = $"{propertySymbol.Type.ContainingNamespace}.{propertySymbol.Type.Name}";
                                    memberInfo.MemberName = propertySymbol.Name;
                                    memberInfo.Offset = offset;
                                    offset += memberInfo.Size;
                                    info.Members.Add(memberInfo);
                                    continue;
                                }
                                else
                                {
                                    var attributes = propertySymbol.Type.GetAttributes();
                                    var hasHelper = attributes.Any(wh => wh.AttributeClass.Name == "GenerateHelperAttribute");
                                    if (!hasHelper)
                                    {
                                        throw new Exception($"A type '{propertySymbol.Type.Name}' nested in a type '{currentType.Name}' is not marked with an attribute 'GenerateHelperAttribute'");
                                    }

                                    if(!(propertySymbol.Type is INamedTypeSymbol namedTypeSymbol))
                                    {
                                        throw new Exception($"A type '{propertySymbol.Type.Name}' nested in a type '{currentType.Name}' is not 'INamedTypeSymbol'");
                                    }
                                    stackCurrentTypes.Push(namedTypeSymbol);
                                    needSkip = true;
                                    break;
                                }
                            }

                            throw new Exception($"The property must be a struct or unmanaged type, property name: '{member.Name}'");
                        }
                        else
                        if (member is Microsoft.CodeAnalysis.IFieldSymbol fieldSymbol)
                        {
                            if (!member.DeclaredAccessibility.HasFlag(Accessibility.Public))
                            {
                                throw new Exception($"Generation is possible only for the public field. Field name: '{member.Name}'");
                            }

                            if (fieldSymbol.Type.IsUnmanagedType)
                            {
                                var memberInfo = new MemberInfo();
                                memberInfo.Size = TypeToSize(fieldSymbol.Type.Name);
                                memberInfo.TypeName = fieldSymbol.Type.Name;
                                memberInfo.MemberName = fieldSymbol.Name;
                                memberInfo.Offset = offset;
                                memberInfo.IsUnmanaged = true;
                                offset += memberInfo.Size;
                                info.Members.Add(memberInfo);
                                continue;
                            }

                            if (fieldSymbol.Type.IsReferenceType)
                            {
                                throw new Exception($"Nested reference field are not supported, field name: '{member.Name}'");
                            }

                            if (fieldSymbol.Type.IsValueType)
                            {
                                if (typeInfos.TryGetValue($"{fieldSymbol.Type.ContainingNamespace}.{fieldSymbol.Type.Name}", out var tInfo))
                                {
                                    var memberInfo = new MemberInfo();
                                    memberInfo.Size = tInfo.Members.Sum(s => s.Size);
                                    memberInfo.TypeName = $"{fieldSymbol.Type.ContainingNamespace}.{fieldSymbol.Type.Name}";
                                    memberInfo.MemberName = fieldSymbol.Name;
                                    memberInfo.Offset = offset;
                                    offset += memberInfo.Size;
                                    info.Members.Add(memberInfo);
                                    continue;
                                }
                                else
                                {
                                    var attributes = fieldSymbol.Type.GetAttributes();
                                    var hasHelper = attributes.Any(wh => wh.AttributeClass.Name == "GenerateHelperAttribute");
                                    if (!hasHelper)
                                    {
                                        throw new Exception($"A type '{fieldSymbol.Type.Name}' nested in a type '{currentType.Name}' is not marked with an attribute 'GenerateHelperAttribute'");
                                    }

                                    if (!(fieldSymbol.Type is INamedTypeSymbol namedTypeSymbol))
                                    {
                                        throw new Exception($"A type '{fieldSymbol.Type.Name}' nested in a type '{currentType.Name}' is not 'INamedTypeSymbol'");
                                    }
                                    stackCurrentTypes.Push(namedTypeSymbol);
                                    needSkip = true;
                                    break;
                                }
                            }
                        }
                    }

                    if(!needSkip)
                    {
                        typeInfos.Add($"{currentType.ContainingNamespace}.{currentType.Name}", info);
                        stackCurrentTypes.Pop();
                    }
                }
            }
        }

        private void GenerateHelpers(
            in List<INamedTypeSymbol> typeHelpers,
            in GeneratorExecutionContext context,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            var helperBuilder = new StringBuilder();
            for (int i = 0; i < typeHelpers.Count; i++)
            {
                var currentType = typeHelpers[i];
                helperBuilder.Clear();
                helperBuilder.Append($@"// <auto-generated by Brevnov Vyacheslav Sergeevich/>
using System;

namespace {currentType.ContainingNamespace}
{{
    public unsafe static class {currentType.Name}Helper
    {{

");
                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                helperBuilder.Append($@"
        public static nuint GetSize()
        {{
            return {typeInfo.Members.Sum(s => s.Size)};
        }}

");
                for (int j = 0; j < typeInfo.Members.Count; j++)
                {
                    MemberInfo memberInfo = typeInfo.Members[j];
                    helperBuilder.Append($@"
        public static void* Get{memberInfo.MemberName}Ptr(in void* ptr)
        {{
            return (byte*)ptr + {memberInfo.Offset};
        }}

");

                    if (memberInfo.IsUnmanaged)
                    {
                        helperBuilder.Append($@"
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
            return *({memberInfo.TypeName}*)((byte*)ptr + {memberInfo.Offset});
        }}

");
                        helperBuilder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {memberInfo.TypeName} value)
        {{
            *({memberInfo.TypeName}*)((byte*)ptr + {memberInfo.Offset}) = value;
        }}

");
                        helperBuilder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {currentType.Name} value)
        {{
            *({memberInfo.TypeName}*)((byte*)ptr + {memberInfo.Offset}) = value.{memberInfo.MemberName};
        }}

");
                    }
                    else
                    {
                        helperBuilder.Append($@"
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
            {memberInfo.TypeName} result = default;
            {currentType.Name}Helper.CopyToStruct((byte*)ptr + {memberInfo.Offset}, ref result);
            return result;
        }}

");
                        helperBuilder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {memberInfo.TypeName} value)
        {{
            {memberInfo.TypeName}Helper.CopyToPtr(in value, in ptr);
        }}

");
                    }

                }

                //--CopyToPtr
                helperBuilder.Append($@"
        public static void CopyToPtr(in {currentType.Name} value, in void* ptr)
        {{

");
                foreach (var memberInfo in typeInfo.Members)
                {
                    helperBuilder.Append($@"
            Set{memberInfo.MemberName}Value(in ptr, in value);
");
                }

                helperBuilder.Append($@"

        }}

");
                //--CopyToPtr--

                //--CopyToValue
                helperBuilder.Append($@"
        public static void CopyToValue(in void* ptr, ref {currentType.Name} value)
        {{

");
                foreach (var memberInfo in typeInfo.Members)
                {
                    helperBuilder.Append($@"
            value.{memberInfo.MemberName} = Get{memberInfo.MemberName}Value(in ptr);
");
                }

                helperBuilder.Append($@"
        }}

");
                //--CopyToValue--

                helperBuilder.Append($@"

    }}
}}
");
                context.AddSource($"{currentType.Name}Helper.g.cs", helperBuilder.ToString());
            }
        }

        private int TypeToSize(string typeName)
        {
            switch(typeName)
            {
                case "Int32":
                {
                    return sizeof(Int32);
                }

                case "Int64":
                {
                    return sizeof(Int64);
                }

                default:
                {
                    throw new Exception($"TypeToSize: unknown type {typeName}");
                }
            }
        }
    }
}