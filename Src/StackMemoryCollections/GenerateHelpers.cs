using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackMemoryCollections
{
    public partial class Generator
    {
        private void GenerateHelpers(
            in List<INamedTypeSymbol> typeHelpers,
            in GeneratorExecutionContext context,
            in Dictionary<string, TypeInfo> typeInfos,
            in StringBuilder builder
            )
        {
            for (int i = 0; i < typeHelpers.Count; i++)
            {
                var currentType = typeHelpers[i];
                builder.Clear();

                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"{nameof(GenerateHelpers)}: Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                GenerateStart(in builder, in currentType);
                GenerateOffsetsAndSize(in builder, in typeInfo);
                GenerateIsNullable(in builder, in typeInfo);

                for (int j = 0; j < typeInfo.Members.Count; j++)
                {
                    MemberInfo memberInfo = typeInfo.Members[j];
                    var offsetStr = memberInfo.IsRuntimeOffsetCalculated ? $"{currentType.Name}Helper.{memberInfo.MemberName}Offset" : $"{memberInfo.Offset}";
                    GenerateGetPtr(in builder, in memberInfo, in offsetStr);
                    
                    if (memberInfo.IsPrimitive || memberInfo.AsPointer)
                    {
                        var memberType = memberInfo.TypeName;
                        if(memberInfo.AsPointer)
                        {
                            memberInfo.TypeName = nameof(IntPtr);
                        }
                        GenerateGetPrimitiveValue(in builder, in memberInfo, in offsetStr);
                        GenerateGetPrimitiveValueRef(in builder, in memberInfo, in offsetStr);
                        GenerateGetPrimitiveValueOut(in builder, in memberInfo, in offsetStr);

                        GenerateSetPrimitiveValue(in builder, in memberInfo, in offsetStr);
                        GenerateSetPrimitiveValueFromPtr(in builder, in memberInfo, in offsetStr);

                        if (!memberInfo.AsPointer)
                            GenerateSetPrimitiveValueFrom(in builder, in memberInfo, in currentType, in offsetStr);

                        memberInfo.TypeName = memberType;
                    }
                    else
                    {
                        GenerateGetСompositeValue(in builder, in memberInfo, in currentType, in offsetStr);
                        GenerateSetСompositeValueFrom(in builder, in memberInfo, in currentType, in offsetStr);
                    }
                }

                GenerateCopyToPtr(in builder, in typeInfo, in currentType);
                GenerateCopyToValue(in builder, in typeInfo, in currentType, in typeInfos);
                GenerateCopyToValueOut(in builder, in typeInfo, in currentType, in typeInfos);

                var sizeOfStr = typeInfo.IsRuntimeCalculatedSize ? $"{currentType.Name}Helper.SizeOf" : $"{typeInfo.Size}";
                GenerateCopy(in builder, in typeInfo, in sizeOfStr);

                GenerateEnd(in builder);

                context.AddSource($"{currentType.Name}Helper.g.cs", builder.ToString());
            }
        }

        private void GenerateStart(
            in StringBuilder builder,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
/*
{Resource.License}
*/

using System;
using System.Runtime.CompilerServices;

namespace {currentType.ContainingNamespace}
{{
    public unsafe static class {currentType.Name}Helper
    {{

");
        }

        private void GenerateOffsetsAndSize(
            in StringBuilder builder,
            in TypeInfo typeInfo
            )
        {
            builder.Append($@"
        public static readonly nuint SizeOf = (nuint){typeInfo.SizeOf};
");

            foreach (var memberInfo in typeInfo.Members)
            {
                if(memberInfo.IsRuntimeOffsetCalculated)
                {
                    builder.Append($@"
        public static readonly nuint {memberInfo.MemberName}Offset = {memberInfo.OffsetStr};
");
                }
            }
        }

        private void GenerateIsNullable(
            in StringBuilder builder,
            in TypeInfo typeInfo
            )
        {
            builder.Append($@"
        public static bool IsNullable()
        {{
            return {(!typeInfo.IsValueType).ToString().ToLowerInvariant()};
        }}
");
        }

        private void GenerateGetPtr(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in string offsetStr
            )
        {
            builder.Append($@"
        public static void* Get{memberInfo.MemberName}Ptr(in void* ptr)
        {{
            return (byte*)ptr + {offsetStr};
        }}
");
        }

        private void GenerateGetPrimitiveValue(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in string offsetStr
            )
        {
            builder.Append($@"
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
            return *({memberInfo.TypeName}*)((byte*)ptr + {offsetStr});
        }}
");
        }

        private void GenerateGetPrimitiveValueRef(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in string offsetStr
            )
        {
            builder.Append($@"
        public static void GetRef{memberInfo.MemberName}Value(in void* ptr, ref {memberInfo.TypeName} item)
        {{
            item = *({memberInfo.TypeName}*)((byte*)ptr + {offsetStr});
        }}
");
        }

        private void GenerateGetPrimitiveValueOut(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in string offsetStr
            )
        {
            builder.Append($@"
        public static void GetOut{memberInfo.MemberName}Value(in void* ptr, out {memberInfo.TypeName} item)
        {{
            item = *({memberInfo.TypeName}*)((byte*)ptr + {offsetStr});
        }}
");
        }

        private void GenerateSetPrimitiveValue(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in string offsetStr
            )
        {
            builder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {memberInfo.TypeName} value)
        {{
            *({memberInfo.TypeName}*)((byte*)ptr + {offsetStr}) = value;
        }}
");
        }

        private void GenerateSetPrimitiveValueFrom(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in INamedTypeSymbol currentType,
            in string offsetStr
            )
        {
            builder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {currentType.Name} value)
        {{
            *({memberInfo.TypeName}*)((byte*)ptr + {offsetStr}) = value.{memberInfo.MemberName};
        }}
");
        }

        private void GenerateSetPrimitiveValueFromPtr(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in string offsetStr
            )
        {
            builder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {memberInfo.TypeName}* valuePtr)
        {{
            *({memberInfo.TypeName}*)((byte*)ptr + {offsetStr}) = *valuePtr;
        }}
");
        }


        private void GenerateGetСompositeValue(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in INamedTypeSymbol currentType,
            in string offsetStr
            )
        {
            if (memberInfo.IsValueType)
            {
                builder.Append($@"
        [SkipLocalsInit]
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
");
                if(!currentType.IsValueType)
                {
                    builder.Append($@"
            if(*((byte*)ptr) == 0)
            {{
                throw new NullReferenceException(""ptr is null value"");
            }}
");
                }

                builder.Append($@"
            {memberInfo.TypeName} result;
            Unsafe.SkipInit(out result);
            {memberInfo.TypeName}Helper.CopyToValue((byte*)ptr + {offsetStr}, ref result);

            return result;
        }}

");
            }
            else
            {
                builder.Append($@"
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
");
                if (!currentType.IsValueType)
                {
                    builder.Append($@"
            if(*((byte*)ptr) == 0)
            {{
                throw new NullReferenceException(""ptr is null value"");
            }}
");
                }

                builder.Append($@"
            if(*((byte*)ptr + {offsetStr}) == 0)
            {{
                return null;
            }}
            
            {memberInfo.TypeName} result = new {memberInfo.TypeName}();
            {memberInfo.TypeName}Helper.CopyToValue((byte*)ptr + {offsetStr}, ref result);
            return result;
        }}
");
            }
        }

        private void GenerateSetСompositeValueFrom(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in INamedTypeSymbol currentType,
            in string offsetStr
            )
        {
            if(memberInfo.IsValueType)
            {
                builder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {currentType.Name} value)
        {{
");
                if (!currentType.IsValueType)
                {
                    builder.Append($@"
            if(*((byte*)ptr) == 0)
            {{
                throw new NullReferenceException(""ptr is null value"");
            }}
");
                }

                builder.Append($@"
            {memberInfo.TypeName}Helper.CopyToPtr(value.{memberInfo.MemberName}, (byte*)ptr + {offsetStr});
        }}
");
            }
            else
            {
                builder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {currentType.Name} value)
        {{
");
                if (!currentType.IsValueType)
                {
                    builder.Append($@"
            if(*((byte*)ptr) == 0)
            {{
                throw new NullReferenceException(""ptr is null value"");
            }}
");
                }

                builder.Append($@"
            if(value.{memberInfo.MemberName} == null)
            {{
                *((byte*)ptr + {offsetStr}) = 0;
                return;
            }}
            
            {memberInfo.TypeName}Helper.CopyToPtr(value.{memberInfo.MemberName}, (byte*)ptr + {offsetStr});
        }}
");
            }
        }

        private void GenerateCopyToPtr(
            in StringBuilder builder,
            in TypeInfo typeInfo,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
        public static void CopyToPtr(in {currentType.Name} value, in void* ptr)
        {{
");
            if(!currentType.IsValueType)
            {
                builder.Append($@"
            if(value == null)
            {{
                *((byte*)ptr) = 0;
                return;
            }}
            else
            {{
                *((byte*)ptr) = 1;
            }}
");
            }

            foreach (var memberInfo in typeInfo.Members)
            {
                if(memberInfo.AsPointer)
                {
                    builder.Append($@"
            Set{memberInfo.MemberName}Value(in ptr, in IntPtr.Zero);
");
                }
                else
                {
                    builder.Append($@"
            Set{memberInfo.MemberName}Value(in ptr, in value);
");
                }
            }

            builder.Append($@"
        }}
");
        }

        private void GenerateCopyToValue(
            in StringBuilder builder,
            in TypeInfo typeInfo,
            in INamedTypeSymbol currentType,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            builder.Append($@"
        public static void CopyToValue(in void* ptr, ref {currentType.Name} value)
        {{
");
            if (!currentType.IsValueType)
            {
                builder.Append($@"
            if(*((byte*)ptr) == 0)
            {{
                value = null;
                return;
            }}
");
            }

            foreach (var memberInfo in typeInfo.Members)
            {
                if(memberInfo.AsPointer)
                {
                    if (!typeInfos.TryGetValue($"{memberInfo.TypeName}", out var memberTypeInfo))
                    {
                        throw new Exception($"{nameof(GenerateCopyToValue)}: Type information not found, types filling error. Type name: {memberInfo.TypeName}");
                    }

                    builder.Append($@"
            var intPtr = Get{memberInfo.MemberName}Value(in ptr);
            if(intPtr == IntPtr.Zero)
            {{
                value.{memberInfo.MemberName} = null;
            }}
            else
            {{
                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName} {memberInfo.MemberName.ToLowerInvariant()} = new {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}();
                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToValue(intPtr.ToPointer(), ref {memberInfo.MemberName.ToLowerInvariant()});
                value.{memberInfo.MemberName} = {memberInfo.MemberName.ToLowerInvariant()};
            }}
");
                }
                else
                {
                    builder.Append($@"
            value.{memberInfo.MemberName} = Get{memberInfo.MemberName}Value(in ptr);
");
                }
            }

            builder.Append($@"
        }}

");
        }

        private void GenerateCopyToValueOut(
            in StringBuilder builder,
            in TypeInfo typeInfo,
            in INamedTypeSymbol currentType,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            if (typeInfo.IsValueType)
            {
                builder.Append($@"
        [SkipLocalsInit]
        public static void CopyToValueOut(in void* ptr, out {currentType.Name} value)
        {{
            Unsafe.SkipInit(out value);
");
                foreach (var memberInfo in typeInfo.Members)
                {
                    if (memberInfo.AsPointer)
                    {
                        if (!typeInfos.TryGetValue($"{memberInfo.TypeName}", out var memberTypeInfo))
                        {
                            throw new Exception($"{nameof(GenerateCopyToValue)}: Type information not found, types filling error. Type name: {memberInfo.TypeName}");
                        }

                        builder.Append($@"
            var intPtr = Get{memberInfo.MemberName}Value(in ptr);
            if(intPtr == IntPtr.Zero)
            {{
                value.{memberInfo.MemberName} = null;
            }}
            else
            {{
                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName} {memberInfo.MemberName.ToLowerInvariant()} = new {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}();
                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToValue(intPtr.ToPointer(), ref {memberInfo.MemberName.ToLowerInvariant()});
                value.{memberInfo.MemberName} = {memberInfo.MemberName.ToLowerInvariant()};
            }}
");
                    }
                    else
                    {
                        builder.Append($@"
            value.{memberInfo.MemberName} = Get{memberInfo.MemberName}Value(in ptr);
");
                    }
                }

                builder.Append($@"
        }}
");
            }
            else
            {
                builder.Append($@"
        public static void CopyToValueOut(in void* ptr, out {currentType.Name} value)
        {{
");
                if (!currentType.IsValueType)
                {
                    builder.Append($@"
            if(*((byte*)ptr) == 0)
            {{
                value = null;
                return;
            }}
");
                }

                builder.Append($@"
            value = new {currentType.Name}();
");

                foreach (var memberInfo in typeInfo.Members)
                {
                    if (memberInfo.AsPointer)
                    {
                        if (!typeInfos.TryGetValue($"{memberInfo.TypeName}", out var memberTypeInfo))
                        {
                            throw new Exception($"{nameof(GenerateCopyToValue)}: Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                        }

                        builder.Append($@"
            var intPtr = Get{memberInfo.MemberName}Value(in ptr);
            if(intPtr == IntPtr.Zero)
            {{
                value.{memberInfo.MemberName} = null;
            }}
            else
            {{
                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName} {memberInfo.MemberName.ToLowerInvariant()} = new {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}();
                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToValue(intPtr.ToPointer(), ref {memberInfo.MemberName.ToLowerInvariant()});
                value.{memberInfo.MemberName} = {memberInfo.MemberName.ToLowerInvariant()};
            }}
");
                    }
                    else
                    {
                        builder.Append($@"
            value.{memberInfo.MemberName} = Get{memberInfo.MemberName}Value(in ptr);
");
                    }
                }

                builder.Append($@"
        }}
");
            }
        }

        private void GenerateCopy(
            in StringBuilder builder,
            in TypeInfo typeInfo,
            in string sizeOfStr
            )
        {
            if (typeInfo.IsValueType)
            {
                builder.Append($@"
        public static void Copy(in void* ptrSource, in void* ptrDest)
        {{
            Buffer.MemoryCopy(
                ptrSource,
                ptrDest,
                {sizeOfStr},
                {sizeOfStr}
                );
        }}
");
            }
            else
            {
                builder.Append($@"
        public static void Copy(in void* ptrSource, in void* ptrDest)
        {{
            if(*((byte*)ptrSource) == 0)
            {{
                *((byte*)ptrDest) = 0;
                return;
            }}

            Buffer.MemoryCopy(
                ptrSource,
                ptrDest,
                {sizeOfStr},
                {sizeOfStr}
                );
        }}
");
            }
        }

        private void GenerateEnd(in StringBuilder builder)
        {
            builder.Append($@"

    }}
}}
");
        }
    }
}