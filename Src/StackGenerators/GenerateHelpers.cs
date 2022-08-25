﻿using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace StackGenerators
{
    public partial class Generator
    {
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
    }
}