﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                c.Assembly.GlobalNamespace
                );
            // Build up the source code

            var infos = new Dictionary<string, TypeInfo>();
            FillTypeInfos(in typeHelpers, in infos);
            GenerateHelpers(in typeHelpers, in context, in infos);
            GenerateWrappers(in typeWrappers, in context, in infos);
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

                            if (!propertySymbol.GetMethod.DeclaredAccessibility.HasFlag(Accessibility.Public))
                            {
                                throw new Exception($"GetMethod must be public. Property name: '{member.Name}'");
                            }

                            if (propertySymbol.SetMethod == null)
                            {
                                throw new Exception($"The property must have SetMethod. Property name: '{member.Name}'");
                            }

                            if (!propertySymbol.SetMethod.DeclaredAccessibility.HasFlag(Accessibility.Public))
                            {
                                throw new Exception($"SetMethod must be public. Property name: '{member.Name}'");
                            }

                            if (IsPrimitive(propertySymbol.Type.Name))
                            {
                                var memberInfo = new MemberInfo();
                                memberInfo.Size = TypeToSize(propertySymbol.Type.Name);
                                memberInfo.TypeName = propertySymbol.Type.Name;
                                memberInfo.MemberName = propertySymbol.Name;
                                memberInfo.Offset = offset;
                                memberInfo.IsPrimitive = true;
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
                                if (!member.IsImplicitlyDeclared)
                                {
                                    throw new Exception($"Generation is possible only for the public field. Field name: '{member.Name}'");
                                }
                                continue;
                            }

                            if (IsPrimitive(fieldSymbol.Type.Name))
                            {
                                var memberInfo = new MemberInfo();
                                memberInfo.Size = TypeToSize(fieldSymbol.Type.Name);
                                memberInfo.TypeName = fieldSymbol.Type.Name;
                                memberInfo.MemberName = fieldSymbol.Name;
                                memberInfo.Offset = offset;
                                memberInfo.IsPrimitive = true;
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

                    if (memberInfo.IsPrimitive)
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
            {memberInfo.TypeName}Helper.CopyToValue((byte*)ptr + {memberInfo.Offset}, ref result);
            return result;
        }}

");
                        helperBuilder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {currentType.Name} value)
        {{
            {memberInfo.TypeName}Helper.CopyToPtr(value.{memberInfo.MemberName}, (byte*)ptr + {memberInfo.Offset});
        }}

");
                    }

                }

                #region CopyToPtr

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

                #endregion

                #region CopyToValue

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

                #endregion

                #region CopyFromPtrToPtr

                helperBuilder.Append($@"
        public static void Copy(in void* ptrSource, in void* ptrDest)
        {{
            Buffer.MemoryCopy(
                ptrSource,
                ptrDest,
                {typeInfo.Members.Sum(s => s.Size)},
                {typeInfo.Members.Sum(s => s.Size)}
                );
        }}
");

                #endregion

                helperBuilder.Append($@"

    }}
}}
");
                context.AddSource($"{currentType.Name}Helper.g.cs", helperBuilder.ToString());
            }
        }

        private void GenerateWrappers(
            in List<INamedTypeSymbol> typeWrappers,
            in GeneratorExecutionContext context,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            var helperBuilder = new StringBuilder();
            for (int i = 0; i < typeWrappers.Count; i++)
            {
                var currentType = typeWrappers[i];
                helperBuilder.Clear();
                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                helperBuilder.Append($@"// <auto-generated by Brevnov Vyacheslav Sergeevich/>
using System;
using StackMemoryCollections;

namespace {currentType.ContainingNamespace}.Struct
{{
    public unsafe struct {currentType.Name}Wrapper : IDisposable
    {{
        private readonly Struct.StackMemory* _stackMemoryS;
        private readonly Class.StackMemory? _stackMemoryC = null;
        private readonly void* _start;

        public {currentType.Name}Wrapper()
        {{
            throw new Exception(""Default constructor not supported"");
        }}

        public {currentType.Name}Wrapper(
            Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = (*stackMemory).AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
            _stackMemoryS = stackMemory;
        }}

        public SimpleStructWrapper(
            Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory.AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
        }}

        public SimpleStructWrapper(
            void* start
            )
        {{
            if (start == null)
            {{
                throw new ArgumentNullException(nameof(start));
            }}

            _start = start;
            _stackMemoryC = null;
            _stackMemoryS = null;
        }}

        public void* Ptr => _start;

        public void Dispose()
        {{
            if(_stackMemoryC != null)
            {{
                _stackMemoryC?.FreeMemory({typeInfo.Members.Sum(s => s.Size)});
            }}
            else if (_stackMemoryS != null)
            {{
                (*_stackMemoryS).FreeMemory({typeInfo.Members.Sum(s => s.Size)});
            }}
        }}
    }}
}}
");
                context.AddSource($"{currentType.Name}WrapperStruct.g.cs", helperBuilder.ToString());

                helperBuilder.Clear();
                helperBuilder.Append($@"// <auto-generated by Brevnov Vyacheslav Sergeevich/>
using System;
using StackMemoryCollections;

namespace {currentType.ContainingNamespace}.Class
{{
    public unsafe class {currentType.Name}Wrapper : IDisposable
    {{
        private readonly Struct.StackMemory* _stackMemoryS;
        private readonly Class.StackMemory? _stackMemoryC = null;
        private readonly void* _start;

        public {currentType.Name}Wrapper()
        {{
            throw new Exception(""Default constructor not supported"");
        }}

        public {currentType.Name}Wrapper(
            Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = (*stackMemory).AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
            _stackMemoryS = stackMemory;
        }}

        public SimpleStructWrapper(
            Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory.AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
        }}

        public SimpleStructWrapper(
            void* start
            )
        {{
            if (start == null)
            {{
                throw new ArgumentNullException(nameof(start));
            }}

            _start = start;
            _stackMemoryC = null;
            _stackMemoryS = null;
        }}

        public void* Ptr => _start;

        #region IDisposable

        private bool _disposed;

        ~{currentType.Name}Wrapper() => Dispose(false);

        public void Dispose()
        {{
            Dispose(true);
            GC.SuppressFinalize(this);
        }}

        protected virtual void Dispose(bool disposing)
        {{
            if (!_disposed)
            {{
                if (disposing)
                {{
                    if(_stackMemoryC != null)
                    {{
                        _stackMemoryC?.FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                    }}
                    else if (_stackMemoryS != null)
                    {{
                        (*_stackMemoryS).FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                    }}
                }}

                _disposed = true;
            }}
        }}

        #endregion
    }}
}}
");
                context.AddSource($"{currentType.Name}WrapperClass.g.cs", helperBuilder.ToString());
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

                case "UInt32":
                {
                    return sizeof(UInt32);
                }

                case "Int64":
                {
                    return sizeof(Int64);
                }

                case "UInt64":
                {
                    return sizeof(UInt64);
                }

                case "SByte":
                {
                    return sizeof(SByte);
                }

                case "Byte":
                {
                    return sizeof(Byte);
                }

                case "Int16":
                {
                    return sizeof(Int16);
                }

                case "UInt16":
                {
                    return sizeof(UInt16);
                }

                case "System.Char":
                {
                    return sizeof(Char);
                }

                case "System.Double":
                {
                    return sizeof(Double);
                }

                case "System.Boolean":
                {
                    return sizeof(Boolean);
                }

                default:
                {
                    throw new Exception($"TypeToSize: unknown type {typeName}");
                }
            }
        }

        private bool IsPrimitive(string typeName)
        {
            switch (typeName)
            {
                case "Int32":
                {
                    return true;
                }

                case "UInt32":
                {
                    return true;
                }

                case "Int64":
                {
                    return true;
                }

                case "UInt64":
                {
                    return true;
                }

                case "SByte":
                {
                    return true;
                }

                case "Byte":
                {
                    return true;
                }

                case "Int16":
                {
                    return true;
                }

                case "UInt16":
                {
                    return true;
                }

                case "System.Char":
                {
                    return true;
                }

                case "System.Double":
                {
                    return true;
                }

                case "System.Boolean":
                {
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}