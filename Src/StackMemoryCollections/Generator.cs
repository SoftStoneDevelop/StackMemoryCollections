using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackMemoryCollections
{
    [Generator]
    public partial class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var c = (CSharpCompilation)context.Compilation;
            var typeHelpers = new List<INamedTypeSymbol>();
            var typeWrappers = new List<INamedTypeSymbol>();
            var typeGeneratedStack = new List<INamedTypeSymbol>();
            var typeGeneratedList = new List<INamedTypeSymbol>();
            var typeGeneratedQueue = new List<INamedTypeSymbol>();
            var typeGeneratedDictionary = new List<INamedTypeSymbol>();

            FillAllTypes(
                in typeHelpers,
                in typeWrappers,
                in typeGeneratedStack,
                in typeGeneratedList,
                in typeGeneratedQueue,
                in typeGeneratedDictionary,
                c.Assembly.GlobalNamespace,
                out var containCollections
                );
            // Build up the source code

            var infos = new Dictionary<string, TypeInfo>();
            FillTypeInfos(in typeHelpers, in infos);

            var builder = new StringBuilder();
            if (!containCollections)
            {
                GeneratePrimitiveWrappers(in context, in builder);
                GeneratePrimitiveStack(in context, in builder);
                GeneratePrimitiveQueue(in context, in builder);
                GeneratePrimitiveList(in context, in builder);
                GenerateMemory(in context, in builder);
            }

            GenerateCommonHelpers(in typeHelpers, in context, in builder);
            GenerateHelpers(in typeHelpers, in context, in infos, in builder);
            GenerateWrappers(in typeWrappers, in context, in infos, in builder);
            GenerateStack(in typeGeneratedStack, in context, in infos, in builder);
            GenerateQueue(in typeGeneratedQueue, in context, in infos, in builder);
            GenerateList(in typeGeneratedList, in context, in infos, in builder);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

        private void FillAllTypes(
            in List<INamedTypeSymbol> typesHelpers,
            in List<INamedTypeSymbol> typesWrappers,
            in List<INamedTypeSymbol> typesGeneratedStack,
            in List<INamedTypeSymbol> typesGeneratedList,
            in List<INamedTypeSymbol> typesGeneratedQueue,
            in List<INamedTypeSymbol> typesGeneratedDictionary,
            INamespaceOrTypeSymbol symbol,
            out bool containCollections
            )
        {
            containCollections = false;
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
                    var hasWrapper = attributes.Any(wh => wh.AttributeClass.Name == "GenerateWrapperAttribute");

                    if (hasHelper || hasStack || hasQueue || hasList || hasDictionary || hasWrapper)
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

                        if(hasWrapper)
                        {
                            typesWrappers.Add(type);
                        }
                    }
                }
                else if(current is INamespaceSymbol namespaceSymbol)
                {
                    if (!containCollections && namespaceSymbol.Name == "StackMemoryCollections")
                    {
                        containCollections = true;
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
                    if(!currentType.Constructors.Any(an => an.Parameters.Length == 0))
                    {
                        throw new Exception($"The type '{currentType.Name}' must have a parameterless constructor");
                    }

                    var info = new TypeInfo();
                    info.IsValueType = currentType.IsValueType;
                    info.IsPrimitive = TypeInfoHelper.IsPrimitive(currentType.Name);
                    info.IsUnmanagedType = currentType.IsUnmanagedType;
                    info.ContainingNamespace = currentType.ContainingNamespace.Name;
                    info.TypeName = currentType.Name;

                    var needSkip = false;

                    foreach (var member in currentType.GetMembers())
                    {
                        if (!member.Kind.HasFlag(SymbolKind.Property) && !member.Kind.HasFlag(SymbolKind.Field))
                        {
                            continue;
                        }

                        var mustBeIgnored = member.GetAttributes().Any(wh => wh.AttributeClass.Name == "GeneratorIgnoreAttribute");
                        info.HasIgnoredMembers |= mustBeIgnored;
                        if (mustBeIgnored)
                        {
                            continue;
                        }

                        if (member is Microsoft.CodeAnalysis.IPropertySymbol propertySymbol)
                        {
                            if(!ProcessProperty(in info, in currentType, in typeInfos, in propertySymbol, in stackCurrentTypes))
                            {
                                needSkip = true;
                                break;
                            }

                            continue;
                        }

                        if (member is Microsoft.CodeAnalysis.IFieldSymbol fieldSymbol)
                        {
                            if (!ProcessField(in info, in currentType, in typeInfos, in fieldSymbol, in stackCurrentTypes))
                            {
                                needSkip = true;
                                break;
                            }

                            continue;
                        }
                    }

                    if(!needSkip)
                    {
                        TypeInfoHelper.CalculateSize(info, in typeInfos);
                        if (!typeInfos.ContainsKey($"{currentType.ContainingNamespace}.{currentType.Name}"))
                        {
                            typeInfos.Add($"{currentType.ContainingNamespace}.{currentType.Name}", info);
                        }
                        stackCurrentTypes.Pop();
                    }
                }
            }
        }

        private bool ProcessProperty(
            in TypeInfo info,
            in INamedTypeSymbol currentType,
            in Dictionary<string, TypeInfo> typeInfos,
            in Microsoft.CodeAnalysis.IPropertySymbol propertySymbol,
            in Stack<INamedTypeSymbol> stackCurrentTypes
            )
        {
            if (!(propertySymbol.Type is INamedTypeSymbol namedTypeSymbol))
            {
                throw new Exception($"A type '{propertySymbol.Type.Name}' nested in a type '{currentType.Name}' is not 'INamedTypeSymbol'");
            }

            if (namedTypeSymbol.IsGenericType)
            {
                throw new Exception($"Generic property not supported. Property name: '{propertySymbol.Name}'");
            }

            if (!propertySymbol.DeclaredAccessibility.HasFlag(Accessibility.Public))
            {
                throw new Exception($"Generation is possible only for the public property. Property name: '{propertySymbol.Name}'");
            }

            if (propertySymbol.GetMethod == null)
            {
                throw new Exception($"The property must have GetMethod. Property name: '{propertySymbol.Name}'");
            }

            if (!propertySymbol.GetMethod.DeclaredAccessibility.HasFlag(Accessibility.Public))
            {
                throw new Exception($"GetMethod must be public. Property name: '{propertySymbol.Name}'");
            }

            if (propertySymbol.SetMethod == null)
            {
                throw new Exception($"The property must have SetMethod. Property name: '{propertySymbol.Name}'");
            }

            if (!propertySymbol.SetMethod.DeclaredAccessibility.HasFlag(Accessibility.Public))
            {
                throw new Exception($"SetMethod must be public. Property name: '{propertySymbol.Name}'");
            }

            if (TypeInfoHelper.IsPrimitive(propertySymbol.Type.Name))
            {
                var memberInfo = new MemberInfo();
                memberInfo.TypeName = propertySymbol.Type.Name;
                memberInfo.MemberName = propertySymbol.Name;
                memberInfo.IsPrimitive = true;
                memberInfo.IsValueType = true;
                memberInfo.IsUnmanagedType = propertySymbol.Type.IsUnmanagedType;
                memberInfo.AsPointer = false;
                TypeInfoHelper.CalculateOffset(memberInfo, in info, in typeInfos);

                info.Members.Add(memberInfo);
                return true;
            }

            if (propertySymbol.Type.IsAbstract)
            {
                throw new Exception($"Abstract type are not supported, property name: '{propertySymbol.Name}'");
            }

            if (propertySymbol.Type.IsRecord)
            {
                throw new Exception($"Record type are not supported, property name: '{propertySymbol.Name}'");
            }

            var asPointer =
                    !propertySymbol.Type.IsValueType &&
                    propertySymbol.GetAttributes().Any(wh => wh.AttributeClass.Name == "AsPointerAttribute")
                    ;

            if (asPointer)
            {
                var memberInfo = new MemberInfo();
                memberInfo.TypeName = $"{propertySymbol.Type.ContainingNamespace}.{propertySymbol.Type.Name}";
                memberInfo.MemberName = propertySymbol.Name;
                memberInfo.IsUnmanagedType = propertySymbol.Type.IsUnmanagedType;
                memberInfo.IsValueType = propertySymbol.Type.IsValueType;
                memberInfo.AsPointer = asPointer;
                TypeInfoHelper.CalculateOffset(memberInfo, in info, in typeInfos);

                info.Members.Add(memberInfo);
                return true;
            }
            
            if (typeInfos.TryGetValue($"{propertySymbol.Type.ContainingNamespace}.{propertySymbol.Type.Name}", out var tInfo))
            {
                var memberInfo = new MemberInfo();
                memberInfo.TypeName = $"{propertySymbol.Type.ContainingNamespace}.{propertySymbol.Type.Name}";
                memberInfo.MemberName = propertySymbol.Name;
                memberInfo.IsUnmanagedType = propertySymbol.Type.IsUnmanagedType;
                memberInfo.IsValueType = propertySymbol.Type.IsValueType;
                memberInfo.AsPointer = asPointer;
                TypeInfoHelper.CalculateOffset(memberInfo, in info, in typeInfos);

                info.Members.Add(memberInfo);
                return true;
            }

            var attributes = propertySymbol.Type.GetAttributes();
            if (!HelperMustGenerated(attributes))
            {
                throw new Exception($"A type '{propertySymbol.Type.Name}' nested in a type '{currentType.Name}' is not marked with an generate attribute");
            }

            if (stackCurrentTypes.Contains(namedTypeSymbol, SymbolEqualityComparer.Default))
            {
                throw new Exception($"Cyclic dependency detected: type '{propertySymbol.Type.Name}'; property '{propertySymbol.Name}'");
            }

            stackCurrentTypes.Push(namedTypeSymbol);
            return false;
        }

        private bool ProcessField(
            in TypeInfo info,
            in INamedTypeSymbol currentType,
            in Dictionary<string, TypeInfo> typeInfos,
            in Microsoft.CodeAnalysis.IFieldSymbol fieldSymbol,
            in Stack<INamedTypeSymbol> stackCurrentTypes
            )
        {
            if (!fieldSymbol.DeclaredAccessibility.HasFlag(Accessibility.Public))
            {
                if (fieldSymbol.IsImplicitlyDeclared)
                {
                    return true;
                }

                throw new Exception($"Generation is possible only for the public field. Field name: '{fieldSymbol.Name}'");
            }

            if (!(fieldSymbol.Type is INamedTypeSymbol namedTypeSymbol))
            {
                throw new Exception($"A type '{fieldSymbol.Type.Name}' nested in a type '{currentType.Name}' is not 'INamedTypeSymbol'");
            }

            if (namedTypeSymbol.IsGenericType)
            {
                throw new Exception($"Generic field not supported. Field name: '{fieldSymbol.Name}'");
            }

            if (TypeInfoHelper.IsPrimitive(fieldSymbol.Type.Name))
            {
                var memberInfo = new MemberInfo();
                memberInfo.TypeName = fieldSymbol.Type.Name;
                memberInfo.MemberName = fieldSymbol.Name;
                memberInfo.IsUnmanagedType = fieldSymbol.Type.IsUnmanagedType;
                memberInfo.IsPrimitive = true;
                memberInfo.IsValueType = true;
                memberInfo.AsPointer = false;
                TypeInfoHelper.CalculateOffset(memberInfo, in info, in typeInfos);

                info.Members.Add(memberInfo);
                return true;
            }

            if (fieldSymbol.Type.IsAbstract)
            {
                throw new Exception($"Abstract type are not supported, field name: '{fieldSymbol.Name}'");
            }

            if (fieldSymbol.Type.IsRecord)
            {
                throw new Exception($"Record type are not supported, field name: '{fieldSymbol.Name}'");
            }

            var asPointer =
                    !fieldSymbol.Type.IsValueType &&
                    fieldSymbol.GetAttributes().Any(wh => wh.AttributeClass.Name == "AsPointerAttribute")
                    ;

            if (asPointer)
            {
                var memberInfo = new MemberInfo();
                memberInfo.TypeName = $"{fieldSymbol.Type.ContainingNamespace}.{fieldSymbol.Type.Name}";
                memberInfo.MemberName = fieldSymbol.Name;
                memberInfo.IsUnmanagedType = fieldSymbol.Type.IsUnmanagedType;
                memberInfo.IsValueType = fieldSymbol.Type.IsValueType;
                memberInfo.AsPointer = asPointer;
                TypeInfoHelper.CalculateOffset(memberInfo, in info, in typeInfos);

                info.Members.Add(memberInfo);
                return true;
            }
            else
            if (typeInfos.TryGetValue($"{fieldSymbol.Type.ContainingNamespace}.{fieldSymbol.Type.Name}", out var tInfo))
            {
                var memberInfo = new MemberInfo();
                memberInfo.TypeName = $"{fieldSymbol.Type.ContainingNamespace}.{fieldSymbol.Type.Name}";
                memberInfo.MemberName = fieldSymbol.Name;
                memberInfo.IsUnmanagedType = fieldSymbol.Type.IsUnmanagedType;
                memberInfo.IsValueType = fieldSymbol.Type.IsValueType;
                memberInfo.AsPointer = asPointer;
                TypeInfoHelper.CalculateOffset(memberInfo, in info, in typeInfos);

                info.Members.Add(memberInfo);
                return true;
            }
            else
            {
                var attributes = fieldSymbol.Type.GetAttributes();
                if (!HelperMustGenerated(attributes))
                {
                    throw new Exception($"A type '{fieldSymbol.Type.Name}' nested in a type '{currentType.Name}' is not marked with an generate attribute");
                }

                if (stackCurrentTypes.Contains(namedTypeSymbol, SymbolEqualityComparer.Default))
                {
                    throw new Exception($"Cyclic dependency detected: type '{fieldSymbol.Type.Name}'; field '{fieldSymbol.Name}'");
                }

                stackCurrentTypes.Push(namedTypeSymbol);
                return false;
            }
        }

        private bool HelperMustGenerated(in System.Collections.Immutable.ImmutableArray<AttributeData> attributes)
        {
            return 
                attributes.Any(wh => 
                wh.AttributeClass.Name == "GenerateHelperAttribute" ||
                wh.AttributeClass.Name == "GenerateStackAttribute" ||
                wh.AttributeClass.Name == "GenerateQueueAttribute" ||
                wh.AttributeClass.Name == "GenerateListAttribute" ||
                wh.AttributeClass.Name == "GenerateDictionaryAttribute" ||
                wh.AttributeClass.Name == "GenerateWrapperAttribute"
                );
        }
    }
}