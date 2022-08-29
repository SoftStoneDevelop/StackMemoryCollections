using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    throw new Exception($"Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                GenerateStart(in builder, in currentType);
                GenerateSize(in builder, in typeInfo);
                
                for (int j = 0; j < typeInfo.Members.Count; j++)
                {
                    MemberInfo memberInfo = typeInfo.Members[j];
                    GenerateGetPtr(in builder, in memberInfo);
                    
                    if (memberInfo.IsPrimitive)
                    {
                        GenerateGetPrimitiveValue(in builder, in memberInfo);
                        GenerateGetPrimitiveValueRef(in builder, in memberInfo);
                        GenerateGetPrimitiveValueOut(in builder, in memberInfo);

                        GenerateSetPrimitiveValue(in builder, in memberInfo);
                        GenerateSetPrimitiveValueFromPtr(in builder, in memberInfo);
                        GenerateSetPrimitiveValueFrom(in builder, in memberInfo, in currentType);
                    }
                    else
                    {
                        GenerateGetСompositeValue(in builder, in memberInfo);
                        GenerateSetСompositeValueFrom(in builder, in memberInfo, in currentType);
                    }
                }

                GenerateCopyToPtr(in builder, in typeInfo, in currentType);
                GenerateCopyToValue(in builder, in typeInfo, in currentType);
                GenerateCopyToValueOut(in builder, in typeInfo, in currentType);
                GenerateCopy(in builder, in typeInfo);
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

        private void GenerateSize(
            in StringBuilder builder,
            in TypeInfo typeInfo
            )
        {
            builder.Append($@"
        public static nuint GetSize()
        {{
            return {typeInfo.Members.Sum(s => s.Size)};
        }}

");
        }

        private void GenerateGetPtr(
            in StringBuilder builder,
            in MemberInfo memberInfo
            )
        {
            builder.Append($@"
        public static void* Get{memberInfo.MemberName}Ptr(in void* ptr)
        {{
            return (byte*)ptr + {memberInfo.Offset};
        }}

");
        }

        private void GenerateGetPrimitiveValue(
            in StringBuilder builder,
            in MemberInfo memberInfo
            )
        {
            builder.Append($@"
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
            return *({memberInfo.TypeName}*)((byte*)ptr + {memberInfo.Offset});
        }}
");
        }

        private void GenerateGetPrimitiveValueRef(
            in StringBuilder builder,
            in MemberInfo memberInfo
            )
        {
            builder.Append($@"
        public static void GetRef{memberInfo.MemberName}Value(in void* ptr, ref {memberInfo.TypeName} item)
        {{
            item = *({memberInfo.TypeName}*)((byte*)ptr + {memberInfo.Offset});
        }}
");
        }

        private void GenerateGetPrimitiveValueOut(
            in StringBuilder builder,
            in MemberInfo memberInfo
            )
        {
            builder.Append($@"
        public static void GetOut{memberInfo.MemberName}Value(in void* ptr, out {memberInfo.TypeName} item)
        {{
            item = *({memberInfo.TypeName}*)((byte*)ptr + {memberInfo.Offset});
        }}
");
        }

        private void GenerateSetPrimitiveValue(
            in StringBuilder builder,
            in MemberInfo memberInfo
            )
        {
            builder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {memberInfo.TypeName} value)
        {{
            *({memberInfo.TypeName}*)((byte*)ptr + {memberInfo.Offset}) = value;
        }}

");
        }

        private void GenerateSetPrimitiveValueFrom(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {currentType.Name} value)
        {{
            *({memberInfo.TypeName}*)((byte*)ptr + {memberInfo.Offset}) = value.{memberInfo.MemberName};
        }}

");
        }

        private void GenerateSetPrimitiveValueFromPtr(
            in StringBuilder builder,
            in MemberInfo memberInfo
            )
        {
            builder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {memberInfo.TypeName}* valuePtr)
        {{
            *({memberInfo.TypeName}*)((byte*)ptr + {memberInfo.Offset}) = *valuePtr;
        }}

");
        }


        private void GenerateGetСompositeValue(
            in StringBuilder builder,
            in MemberInfo memberInfo
            )
        {
            if (memberInfo.IsValueType)
            {
                builder.Append($@"
        [SkipLocalsInit]
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
            {memberInfo.TypeName} result;
            Unsafe.SkipInit(out result);
            {memberInfo.TypeName}Helper.CopyToValue((byte*)ptr + {memberInfo.Offset}, ref result);

            return result;
        }}

");
                return;
            }
            else
            {
                builder.Append($@"
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
            {memberInfo.TypeName} result = new {memberInfo.TypeName}();
            {memberInfo.TypeName}Helper.CopyToValue((byte*)ptr + {memberInfo.Offset}, ref result);
            return result;
        }}
");
            }
        }

        private void GenerateSetСompositeValueFrom(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
        public static void Set{memberInfo.MemberName}Value(in void* ptr, in {currentType.Name} value)
        {{
            {memberInfo.TypeName}Helper.CopyToPtr(value.{memberInfo.MemberName}, (byte*)ptr + {memberInfo.Offset});
        }}
");
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
            foreach (var memberInfo in typeInfo.Members)
            {
                builder.Append($@"
            Set{memberInfo.MemberName}Value(in ptr, in value);
");
            }

            builder.Append($@"

        }}
");
        }

        private void GenerateCopyToValue(
            in StringBuilder builder,
            in TypeInfo typeInfo,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
        public static void CopyToValue(in void* ptr, ref {currentType.Name} value)
        {{

");
            foreach (var memberInfo in typeInfo.Members)
            {
                builder.Append($@"
            value.{memberInfo.MemberName} = Get{memberInfo.MemberName}Value(in ptr);
");
            }

            builder.Append($@"
        }}

");
        }

        private void GenerateCopyToValueOut(
            in StringBuilder builder,
            in TypeInfo typeInfo,
            in INamedTypeSymbol currentType
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
                    builder.Append($@"
            value.{memberInfo.MemberName} = Get{memberInfo.MemberName}Value(in ptr);
");
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
            value = new {currentType.Name}();
");
                foreach (var memberInfo in typeInfo.Members)
                {
                    builder.Append($@"
            value.{memberInfo.MemberName} = Get{memberInfo.MemberName}Value(in ptr);
");
                }

                builder.Append($@"
        }}
");
            }
        }

        private void GenerateCopy(
            in StringBuilder builder,
            in TypeInfo typeInfo
            )
        {
            builder.Append($@"
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