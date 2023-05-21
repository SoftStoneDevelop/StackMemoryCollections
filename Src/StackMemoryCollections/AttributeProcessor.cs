using Microsoft.CodeAnalysis;
using StackMemoryCollections.Generators.Primitive;
using StackMemoryCollections.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StackMemoryCollections
{
    internal class AttributeProcessor
    {
        private readonly CommonHelpersGenerator _commonHelpersHenerator = new CommonHelpersGenerator();
        private readonly HelpersGenerator _helpersGenerator = new HelpersGenerator();
        private readonly MemoryGenerator _memoryGenerator = new MemoryGenerator();

        private readonly PrimitiveWrappersGenerator _primitiveWrappersGenerator = new PrimitiveWrappersGenerator();
        private readonly PrimitiveStackGenerator _primitiveStackGenerator = new PrimitiveStackGenerator();
        private readonly PrimitiveQueueGenerator _primitiveQueueGenerator = new PrimitiveQueueGenerator();
        private readonly PrimitiveListGenerator _primitiveListGenerator = new PrimitiveListGenerator();

        private readonly WrappersGenerator _wrappersGenerator = new WrappersGenerator();
        private readonly StackGenerator _stackGenerator = new StackGenerator();
        private readonly QueueGenerator _queueGenerator = new QueueGenerator();
        private readonly ListGenerator _listGenerator = new ListGenerator();

        private readonly List<INamedTypeSymbol> _typeHelpers = new List<INamedTypeSymbol>();
        private readonly List<INamedTypeSymbol> _typeWrappers = new List<INamedTypeSymbol>();
        private readonly List<INamedTypeSymbol> _typeGeneratedStack = new List<INamedTypeSymbol>();
        private readonly List<INamedTypeSymbol> _typeGeneratedList = new List<INamedTypeSymbol>();
        private readonly List<INamedTypeSymbol> _typeGeneratedQueue = new List<INamedTypeSymbol>();
        private readonly List<INamedTypeSymbol> _typeGeneratedDictionary = new List<INamedTypeSymbol>();
        private bool _containCollections = false;

        private readonly Dictionary<string, Model.TypeInfo> _typeInfos = new Dictionary<string, Model.TypeInfo>();

        public void FillAllTypes(
            INamespaceOrTypeSymbol symbol
            )
        {
            var queue = new Queue<INamespaceOrTypeSymbol>();
            queue.Enqueue(symbol);

            while (queue.Count != 0)
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
                        _typeHelpers.Add(type);

                        if (hasStack)
                        {
                            _typeGeneratedStack.Add(type);
                        }

                        if (hasList)
                        {
                            _typeGeneratedList.Add(type);
                        }

                        if (hasQueue)
                        {
                            _typeGeneratedQueue.Add(type);
                        }

                        if (hasDictionary)
                        {
                            _typeGeneratedDictionary.Add(type);
                        }

                        if (hasWrapper)
                        {
                            _typeWrappers.Add(type);
                        }
                    }
                }
                else if (current is INamespaceSymbol namespaceSymbol)
                {
                    if (!_containCollections && namespaceSymbol.Name == "StackMemoryCollections")
                    {
                        _containCollections = true;
                    }
                }

                foreach (var child in current.GetMembers())
                {
                    if (child is INamespaceOrTypeSymbol symbolChild)
                    {
                        queue.Enqueue(symbolChild);
                    }
                }
            }

            FillInfos();
        }

        private void FillInfos()
        {
            for (int i = 0; i < _typeHelpers.Count; i++)
            {
                var stackCurrentTypes = new Stack<INamedTypeSymbol>();
                stackCurrentTypes.Push(_typeHelpers[i]);

                while (stackCurrentTypes.Count != 0)
                {
                    var currentType = stackCurrentTypes.Peek();
                    if (!currentType.Constructors.Any(an => an.Parameters.Length == 0))
                    {
                        throw new Exception($"The type '{currentType.Name}' must have a parameterless constructor");
                    }

                    var info = new Model.TypeInfo();
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
                            if (!ProcessProperty(in info, in currentType, in _typeInfos, in propertySymbol, in stackCurrentTypes))
                            {
                                needSkip = true;
                                break;
                            }

                            continue;
                        }

                        if (member is Microsoft.CodeAnalysis.IFieldSymbol fieldSymbol)
                        {
                            if (!ProcessField(in info, in currentType, in _typeInfos, in fieldSymbol, in stackCurrentTypes))
                            {
                                needSkip = true;
                                break;
                            }

                            continue;
                        }
                    }

                    if (!needSkip)
                    {
                        TypeInfoHelper.CalculateSize(info, in _typeInfos);
                        if (!_typeInfos.ContainsKey($"{currentType.ContainingNamespace}.{currentType.Name}"))
                        {
                            _typeInfos.Add($"{currentType.ContainingNamespace}.{currentType.Name}", info);
                        }
                        stackCurrentTypes.Pop();
                    }
                }
            }
        }

        private bool ProcessProperty(
            in Model.TypeInfo info,
            in INamedTypeSymbol currentType,
            in Dictionary<string, Model.TypeInfo> typeInfos,
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
                var memberInfo = new Model.MemberInfo();
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
                var memberInfo = new Model.MemberInfo();
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
                var memberInfo = new Model.MemberInfo();
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
            in Model.TypeInfo info,
            in INamedTypeSymbol currentType,
            in Dictionary<string, Model.TypeInfo> typeInfos,
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
                var memberInfo = new Model.MemberInfo();
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
                var memberInfo = new Model.MemberInfo();
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
                var memberInfo = new Model.MemberInfo();
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

        public void Generate(GeneratorExecutionContext context)
        {
            if (!_containCollections)
            {
                _primitiveWrappersGenerator.GeneratePrimitiveWrappers(context);
                _primitiveStackGenerator.GeneratePrimitiveStack(context);
                _primitiveQueueGenerator.GeneratePrimitiveQueue(context);
                _primitiveListGenerator.GeneratePrimitiveList(context);
                _memoryGenerator.GenerateMemory(context);
            }

            _commonHelpersHenerator.GenerateCommonHelpers(_typeHelpers, context);
            _helpersGenerator.GenerateHelpers(in _typeHelpers, in context, in _typeInfos);

            _wrappersGenerator.GenerateWrappers(in _typeWrappers, in context, in _typeInfos);
            _stackGenerator.GenerateStack(in _typeGeneratedStack, in context, _typeInfos);
            _queueGenerator.GenerateQueue(in _typeGeneratedQueue, in context, _typeInfos);
            _listGenerator.GenerateList(in _typeGeneratedList, in context, _typeInfos);
        }
    }
}