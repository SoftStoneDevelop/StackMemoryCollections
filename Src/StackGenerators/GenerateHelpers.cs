using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackGenerators
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
                        GenerateSetPrimitiveValue(in builder, in memberInfo);
                        GenerateSetPrimitiveValueFrom(in builder, in memberInfo, in currentType);
                    }
                    else
                    {
                        GenerateGetСompositeValue(in builder, in memberInfo, in typeInfos);
                        GenerateSetСompositeValueFrom(in builder, in memberInfo, in currentType);
                    }
                }

                GenerateCopyToPtr(in builder, in typeInfo, in currentType);
                GenerateCopyToValue(in builder, in typeInfo, in currentType);
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
MIT License

Copyright (c) 2022 Brevnov Vyacheslav Sergeevich

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
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


        private void GenerateGetСompositeValue(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            if (memberInfo.IsValueType)
            {
                if (!typeInfos.TryGetValue(memberInfo.TypeName, out var typeInfo))
                {
                    throw new Exception($"Type information not found, types filling error. Type name: {memberInfo.TypeName}");
                }

                if (typeInfo.IsStructLayoutSequential &&
                    typeInfo.Members.All(all => all.IsUnmanagedType) &&
                    typeInfo.AllIsStructLayoutSequential(in typeInfos)
                    )
                {
                    builder.Append($@"
        [SkipLocalsInit]
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
            {memberInfo.TypeName} result;
            {memberInfo.TypeName}Helper.Copy((byte*)ptr + {memberInfo.Offset}, &result);
            return result;
        }}

");
                    return;
                }
            }

            builder.Append($@"
        public static {memberInfo.TypeName} Get{memberInfo.MemberName}Value(in void* ptr)
        {{
            {memberInfo.TypeName} result = default;
            {memberInfo.TypeName}Helper.CopyToValue((byte*)ptr + {memberInfo.Offset}, ref result);
            return result;
        }}

");
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