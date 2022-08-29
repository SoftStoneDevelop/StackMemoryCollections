using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StackMemoryCollections
{
    public partial class Generator
    {
        private void GenerateWrappers(
            in List<INamedTypeSymbol> typeWrappers,
            in GeneratorExecutionContext context,
            in Dictionary<string, TypeInfo> typeInfos,
            in StringBuilder builder
            )
        {
            for (int i = 0; i < typeWrappers.Count; i++)
            {
                var currentType = typeWrappers[i];
                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                GenerateWrapper(in currentType, in context, in typeInfo, in builder, "Class", in typeInfos);
                GenerateWrapper(in currentType, in context, in typeInfo, in builder, "Struct", in typeInfos);
            }
        }

        private void GenerateWrapper(
            in INamedTypeSymbol currentType,
            in GeneratorExecutionContext context,
            in TypeInfo typeInfo,
            in StringBuilder builder,
            in string wrapperNamespace,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            builder.Clear();
            WrapperStart(in currentType, in builder, in wrapperNamespace);
            WrapperConstructor1(in currentType, in typeInfo, in builder);
            WrapperConstructor2(in currentType, in typeInfo, in builder);
            WrapperConstructor3(in currentType, in typeInfo, in builder);
            WrapperConstructor4(in currentType, in builder);

            WrapperProperties(in typeInfo, in builder, in typeInfos);

            WrapperDispose(in currentType, in typeInfo, in builder, in wrapperNamespace);


            WrapperEnd(in builder);

            context.AddSource($"{currentType.Name}Wrapper{wrapperNamespace}.g.cs", builder.ToString());
        }

        private void WrapperStart(
            in INamedTypeSymbol currentType,
            in StringBuilder builder,
            in string wrapperNamespace
            )
        {
            builder.Append($@"
/*
{Resource.License}
*/

using System;
using {currentType.ContainingNamespace};
using System.Runtime.CompilerServices;

namespace {currentType.ContainingNamespace}.{wrapperNamespace}
{{
    public unsafe {wrapperNamespace.ToLowerInvariant()} {currentType.Name}Wrapper : IDisposable
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private readonly StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private void* _start;
        private readonly bool _memoryOwner = false;
");
        }

        private void WrapperConstructor1(
            in INamedTypeSymbol currentType,
            in TypeInfo typeInfo,
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public {currentType.Name}Wrapper()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({typeInfo.Members.Sum(s => s.Size)});
            _start = _stackMemoryC.Start;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void WrapperConstructor2(
            in INamedTypeSymbol currentType,
            in TypeInfo typeInfo,
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public {currentType.Name}Wrapper(
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory->AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
            _stackMemoryS = stackMemory;
        }}
");
        }

        private void WrapperConstructor3(
            in INamedTypeSymbol currentType,
            in TypeInfo typeInfo,
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public {currentType.Name}Wrapper(
            StackMemoryCollections.Class.StackMemory stackMemory
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
");
        }

        private void WrapperConstructor4(
            in INamedTypeSymbol currentType,
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public {currentType.Name}Wrapper(
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
");
        }

        private void WrapperDispose(
            in INamedTypeSymbol currentType,
            in TypeInfo typeInfo,
            in StringBuilder builder,
            in string wrapperNamespace
            )
        {
            if(wrapperNamespace == "Class")
            {
                builder.Append($@"
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
                if(!_memoryOwner)
                {{
                    if (disposing)
                    {{
                        if(_stackMemoryC != null)
                        {{
                            _stackMemoryC?.FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                        }}
                        else if (_stackMemoryS != null)
                        {{
                            _stackMemoryS->FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                        }}
                    }}
                }}
                else
                {{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    _stackMemoryC.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }}

                _disposed = true;
            }}
        }}

        #endregion
");
            }
            else
            {
                builder.Append($@"
        public void Dispose()
        {{
            if(!_memoryOwner)
            {{
                if(_stackMemoryC != null)
                {{
                    _stackMemoryC?.FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                }}
                else if (_stackMemoryS != null)
                {{
                    _stackMemoryS->FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                }}
            }}
            else
            {{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }}
            
        }}
");
            }
        }

        private void WrapperProperties(
            in TypeInfo typeInfo,
            in StringBuilder builder,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            WrapperPtr(in builder);
            PrimitiveWrapperChangePtr(in builder);
            WrapperGetValue(in typeInfo, in builder);
            WrapperFillValue(in typeInfo, in builder);

            for (int i = 0; i < typeInfo.Members.Count; i++)
            {
                var currentMember = typeInfo.Members[i];
                if (currentMember.IsPrimitive)
                {
                    WrapperPrimitiveGetSet(in builder, in currentMember, in typeInfo);
                    WrapperPrimitiveSetIn(in builder, in currentMember, in typeInfo);
                    WrapperPrimitiveSetPtr(in builder, in currentMember, in typeInfo);

                    WrapperPrimitiveGetOut(in builder, in currentMember, in typeInfo);
                }
                else
                {
                    if (!typeInfos.TryGetValue(currentMember.TypeName, out var memberTypeInfo))
                    {
                        throw new Exception($"Type information not found, types filling error. Type name: {currentMember.TypeName}");
                    }

                    WrapperСompositeGetSet(in builder, in currentMember, in memberTypeInfo, in typeInfo);
                    WrapperСompositeSetIn(in builder, in currentMember, in memberTypeInfo, in typeInfo);
                    WrapperСompositeSetPtr(in builder, in currentMember, in memberTypeInfo, in typeInfo);
                    WrapperСompositeGetOut(in builder, in currentMember, in memberTypeInfo, in typeInfo);
                }
            }
        }

        private void WrapperPtr(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public void* Ptr => _start;
");
        }

        private void WrapperGetValue(
            in TypeInfo typeInfo,
            in StringBuilder builder
            )
        {
            if(typeInfo.IsValueType)
            {
                builder.Append($@"
        [SkipLocalsInit]
        public {typeInfo.TypeName} GetValue()
        {{
            {typeInfo.ContainingNamespace}.{typeInfo.TypeName} result;
            Unsafe.SkipInit(out result);
            {typeInfo.ContainingNamespace}.{typeInfo.TypeName}Helper.CopyToValue(in _start, ref result);

            return result;
        }}

            ");
            }
            else
            {
                builder.Append($@"
        public {typeInfo.TypeName} GetValue()
        {{
            {typeInfo.ContainingNamespace}.{typeInfo.TypeName} result = new {typeInfo.TypeName}();
            {typeInfo.ContainingNamespace}.{typeInfo.TypeName}Helper.CopyToValue(in _start, ref result);
            return result;
        }}
            ");
            }
        }

        private void WrapperFillValue(
            in TypeInfo typeInfo,
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public void Fill(in {typeInfo.TypeName} value)
        {{
            {typeInfo.TypeName}Helper.CopyToPtr(in value, in _start);
        }}

            ");
        }

        private void WrapperPrimitiveGetSet(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in TypeInfo containingType
            )
        {
            builder.Append($@"
        public {memberInfo.TypeName} {memberInfo.MemberName} 
        {{
            get
            {{
                return {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Value(in _start);
            }}

            set
            {{
                {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Set{memberInfo.MemberName}Value(in _start, in value);
            }}
        }}
");
        }

        private void WrapperPrimitiveSetIn(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in TypeInfo containingType
            )
        {
            builder.Append($@"
        public void Set{memberInfo.MemberName}(in {memberInfo.TypeName} item)
        {{
            {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Set{memberInfo.MemberName}Value(in _start, in item);
        }}
");
        }

        private void WrapperPrimitiveSetPtr(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in TypeInfo containingType
            )
        {
            builder.Append($@"
        public void Set{memberInfo.MemberName}(in {memberInfo.TypeName}* valuePtr)
        {{
            {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Set{memberInfo.MemberName}Value(in _start, in valuePtr);
        }}
");
        }

        private void WrapperPrimitiveGetOut(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in TypeInfo containingType
            )
        {
            builder.Append($@"
        public void GetOut{memberInfo.MemberName}(out {memberInfo.TypeName} item)
        {{
            {containingType.ContainingNamespace}.{containingType.TypeName}Helper.GetOut{memberInfo.MemberName}Value(in _start, out item);
        }}
");
        }

        private void WrapperСompositeGetSet(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in TypeInfo memberTypeInfo,
            in TypeInfo containingType
            )
        {
            builder.Append($@"
        public {memberInfo.TypeName} {memberInfo.MemberName} 
        {{
            get
            {{
                var ptr = {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
                var result = new {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}();
                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToValue(in ptr, ref result);
                return result;
            }}

            set
            {{
                var ptr = {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToPtr(in value, in ptr);
            }}
        }}
");
        }

        private void WrapperСompositeSetPtr(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in TypeInfo memberTypeInfo,
            in TypeInfo containingType
            )
        {
            builder.Append($@"
        public void Set{memberInfo.MemberName} (in void* valuePtr)
        {{
            var ptr = {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
            {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.Copy(in valuePtr, in ptr);
        }}
");
        }

        private void WrapperСompositeGetOut(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in TypeInfo memberTypeInfo,
            in TypeInfo containingType
            )
        {
            builder.Append($@"
        public void GetOut{memberInfo.MemberName}(out {memberInfo.TypeName} item)
        {{
            var ptr = {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
            {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToValueOut(in ptr, out item);
        }}
");
        }

        private void WrapperСompositeSetIn(
            in StringBuilder builder,
            in MemberInfo memberInfo,
            in TypeInfo memberTypeInfo,
            in TypeInfo containingType
            )
        {
            builder.Append($@"
        public void Set{memberInfo.MemberName}(in {memberInfo.TypeName} item) 
        {{
            var ptr = {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
            {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToPtr(in item, in ptr);
        }}
");
        }

        private void PrimitiveWrapperChangePtr(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public void ChangePtr(in void* newPtr)
        {{
            if(_stackMemoryC != null || _stackMemoryS != null)
            {{
                throw new Exception(""Only an instance created via a void* constructor can change the pointer."");
            }}

            _start = newPtr;
        }}
");
        }

        private void WrapperEnd(
            in StringBuilder builder
            )
        {
            builder.Append($@"
    }}
}}
");
        }
    }
}