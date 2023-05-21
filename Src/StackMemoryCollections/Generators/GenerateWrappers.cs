using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackMemoryCollections
{
    internal class WrappersGenerator
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public void GenerateWrappers(
            in List<INamedTypeSymbol> typeWrappers,
            in SourceProductionContext context,
            in Dictionary<string, Model.TypeInfo> typeInfos
            )
        {
            for (int i = 0; i < typeWrappers.Count; i++)
            {
                var currentType = typeWrappers[i];
                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"{nameof(GenerateWrappers)}: Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                GenerateWrapper(in currentType, in context, in typeInfo, "Class", in typeInfos);
                GenerateWrapper(in currentType, in context, in typeInfo, "Struct", in typeInfos);
            }
        }

        private void GenerateWrapper(
            in INamedTypeSymbol currentType,
            in SourceProductionContext context,
            in Model.TypeInfo typeInfo,
            in string wrapperNamespace,
            in Dictionary<string, Model.TypeInfo> typeInfos
            )
        {
            _builder.Clear();

            var sizeOfStr = typeInfo.IsRuntimeCalculatedSize ? $"{currentType.Name}Helper.SizeOf" : $"{typeInfo.Size}";
            WrapperStart(in currentType, in wrapperNamespace);
            WrapperConstructor1(in currentType, in typeInfo, in sizeOfStr);
            WrapperConstructor2(in currentType, in typeInfo, in sizeOfStr);
            WrapperConstructor3(in currentType, in typeInfo, in sizeOfStr);
            WrapperConstructor4(in currentType, in typeInfo);
            WrapperCreateInstance(in typeInfo);

            WrapperProperties(in typeInfo, in typeInfos);

            WrapperDispose(in currentType, in wrapperNamespace, in sizeOfStr);


            WrapperEnd();

            context.AddSource($"{currentType.Name}Wrapper{wrapperNamespace}.g.cs", _builder.ToString());
        }

        private void WrapperStart(
            in INamedTypeSymbol currentType,
            in string wrapperNamespace
            )
        {
            _builder.Append($@"
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
            in Model.TypeInfo typeInfo,
            in string sizeOfStr
            )
        {
            _builder.Append($@"
        public {currentType.Name}Wrapper()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({sizeOfStr});
            _start = _stackMemoryC.Start;
            _memoryOwner = true;
            _stackMemoryS = null;
");
            if (!currentType.IsValueType)
            {
                _builder.Append($@"
            *((byte*)_start) = 1;
");
            }

            ResetPointerAndReference(in typeInfo, "            ");

            _builder.Append($@"
        }}
");
        }

        private void WrapperConstructor2(
            in INamedTypeSymbol currentType,
            in Model.TypeInfo typeInfo,
            in string sizeOfStr
            )
        {
            _builder.Append($@"
        public {currentType.Name}Wrapper(
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory->AllocateMemory({sizeOfStr});
            _stackMemoryS = stackMemory;
");
            if (!currentType.IsValueType)
            {
                _builder.Append($@"
            *((byte*)_start) = 1;
");
            }

            ResetPointerAndReference(in typeInfo, "            ");

            _builder.Append($@"
        }}
");
        }

        private void WrapperConstructor3(
            in INamedTypeSymbol currentType,
            in Model.TypeInfo typeInfo,
            in string sizeOfStr
            )
        {
            _builder.Append($@"
        public {currentType.Name}Wrapper(
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory.AllocateMemory({sizeOfStr});
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
");
            if (!currentType.IsValueType)
            {
                _builder.Append($@"
            *((byte*)_start) = 1;
");
            }

            ResetPointerAndReference(in typeInfo, "            ");

            _builder.Append($@"
        }}
");
        }

        private void WrapperConstructor4(
            in INamedTypeSymbol currentType,
            in Model.TypeInfo typeInfo
            )
        {
            _builder.Append($@"
        public {currentType.Name}Wrapper(
            void* start,
            bool createInstance
            )
        {{
            if (start == null)
            {{
                throw new ArgumentNullException(nameof(start));
            }}

            _start = start;
            _stackMemoryC = null;
            _stackMemoryS = null;
");
            _builder.Append($@"
            if(createInstance)
            {{
                
");

            if (!currentType.IsValueType)
            {
                _builder.Append($@"
            *((byte*)_start) = 1;
");
            }

            ResetPointerAndReference(in typeInfo, "                ");
            _builder.Append($@"
            }}
        }}
");
        }

        private void ResetPointerAndReference(
            in Model.TypeInfo typeInfo,
            in string paddings
            )
        {
            for (int i = 0; i < typeInfo.Members.Count; i++)
            {
                var currentMember = typeInfo.Members[i];
                if (!currentMember.IsValueType && !currentMember.AsPointer)
                {
                    _builder.Append($@"
{paddings}//set null marker {currentMember.MemberName}
{paddings}*((byte*)_start + {(currentMember.IsRuntimeOffsetCalculated ? $"{typeInfo.TypeName}Helper.{currentMember.MemberName}Offset" : $"{currentMember.Offset}")}) = 0;
");
                    continue;
                }

                if (currentMember.AsPointer)
                {
                    _builder.Append($@"
{paddings}//IntPtr must be a valid structure, so write it down. Member: {currentMember.MemberName}
{paddings}*(IntPtr*)((byte*)_start + {(currentMember.IsRuntimeOffsetCalculated ? $"{typeInfo.TypeName}Helper.{currentMember.MemberName}Offset" : $"{currentMember.Offset}")}) = IntPtr.Zero;
");
                    continue;
                }
            }
        }

        private void WrapperCreateInstance(
            in Model.TypeInfo typeInfo
            )
        {
            if (!typeInfo.IsValueType)
            {
                _builder.Append($@"
        public void CreateInstance()
        {{
            *((byte*)_start) = 1;
");
                ResetPointerAndReference(in typeInfo, "            ");
                _builder.Append($@"
        }}
");
            }
        }

        private void WrapperDispose(
            in INamedTypeSymbol currentType,
            in string wrapperNamespace,
            in string sizeOfStr
            )
        {
            if(wrapperNamespace == "Class")
            {
                _builder.Append($@"
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
                            _stackMemoryC?.FreeMemory({sizeOfStr});
                        }}
                        else if (_stackMemoryS != null)
                        {{
                            _stackMemoryS->FreeMemory({sizeOfStr});
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
                _builder.Append($@"
        public void Dispose()
        {{
            if(!_memoryOwner)
            {{
                if(_stackMemoryC != null)
                {{
                    _stackMemoryC?.FreeMemory({sizeOfStr});
                }}
                else if (_stackMemoryS != null)
                {{
                    _stackMemoryS->FreeMemory({sizeOfStr});
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
            in Model.TypeInfo typeInfo,
            in Dictionary<string, Model.TypeInfo> typeInfos
            )
        {
            WrapperPtr();
            WrapperIsNull(in typeInfo);
            PrimitiveWrapperChangePtr();
            WrapperGetValue(in typeInfo);
            WrapperFillValue(in typeInfo);

            for (int i = 0; i < typeInfo.Members.Count; i++)
            {
                var currentMember = typeInfo.Members[i];
                if (currentMember.IsPrimitive || currentMember.AsPointer)
                {
                    var memberType = currentMember.TypeName;
                    if (currentMember.AsPointer)
                        currentMember.TypeName = typeof(IntPtr).Name;

                    WrapperPrimitiveGetPtr(in currentMember, in typeInfo);
                    WrapperPrimitiveGetSet(in currentMember, in typeInfo);
                    WrapperPrimitiveSetIn(in currentMember, in typeInfo);
                    WrapperPrimitiveSetPtr(in currentMember, in typeInfo);

                    WrapperPrimitiveGetOut(in currentMember, in typeInfo);

                    currentMember.TypeName = memberType;
                    if(currentMember.AsPointer)
                    {
                        WrapperGetValueInPtr(in typeInfo, in currentMember, in typeInfos);
                    }
                }
                else
                {
                    if (!typeInfos.TryGetValue(currentMember.TypeName, out var memberTypeInfo))
                    {
                        throw new Exception($"{nameof(WrapperProperties)}: Type information not found, types filling error. Type name: {currentMember.TypeName}");
                    }

                    WrapperСompositeGetPtr(in currentMember, in typeInfo);
                    WrapperСompositeGetSet(in currentMember, in memberTypeInfo, in typeInfo);
                    WrapperСompositeSetIn(in currentMember, in memberTypeInfo, in typeInfo);
                    WrapperСompositeSetPtr(in currentMember, in memberTypeInfo, in typeInfo);
                    WrapperСompositeGetOut(in currentMember, in memberTypeInfo, in typeInfo);
                }
            }
        }

        private void WrapperPtr()
        {
            _builder.Append($@"
        public void* Ptr => _start;
");
        }

        private void WrapperIsNull(
            in Model.TypeInfo typeInfo
            )
        {
            if(!typeInfo.IsValueType)
            {
                _builder.Append($@"
        public bool IsNull => *((byte*)_start) == 0;
");
            }
        }

        private void WrapperGetValue(
            in Model.TypeInfo typeInfo
            )
        {
            if(typeInfo.IsValueType)
            {
                _builder.Append($@"
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
                _builder.Append($@"
        public {typeInfo.TypeName} GetValue()
        {{
            {typeInfo.ContainingNamespace}.{typeInfo.TypeName} result = new {typeInfo.TypeName}();
            {typeInfo.ContainingNamespace}.{typeInfo.TypeName}Helper.CopyToValue(in _start, ref result);
            return result;
        }}
");
            }
        }

        private void WrapperGetValueInPtr(
            in Model.TypeInfo containType,
            in Model.MemberInfo memberInfo,
            in Dictionary<string, Model.TypeInfo> typeInfos
            )
        {
            if (!typeInfos.TryGetValue($"{memberInfo.TypeName}", out var memberTypeInfo))
            {
                throw new Exception($"{nameof(WrapperGetValueInPtr)}: Type information not found, types filling error. Type name: {memberInfo.TypeName}");
            }

            _builder.Append($@"
        public {memberInfo.TypeName} {memberInfo.MemberName}ValueInPtr
        {{
            get
            {{
");
            if(!containType.IsValueType)
            {
                _builder.Append($@"
                if (*((byte*)_start) == 0)
                {{
                    throw new NullReferenceException(""ptr is null value"");
                }}
");
            }

            _builder.Append($@"
                var intPtr = {memberInfo.MemberName};
                if(intPtr == IntPtr.Zero)
                {{
                    return null;
                }}

                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName} result = new {memberTypeInfo.TypeName}();
                {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToValue(intPtr.ToPointer(), ref result);
                return result;
            }}
        }}
");
        }

        private void WrapperFillValue(
            in Model.TypeInfo typeInfo
            )
        {
            _builder.Append($@"
        public void Fill(in {typeInfo.TypeName} value)
        {{
            {typeInfo.TypeName}Helper.CopyToPtr(in value, in _start);
        }}

            ");
        }

        private void WrapperPrimitiveGetSet(
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
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

        private void WrapperPrimitiveGetPtr(
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
        public {memberInfo.TypeName}* {memberInfo.MemberName}Ptr
        {{
            get
            {{
                return ({memberInfo.TypeName}*){containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
            }}
        }}
");
        }

        private void WrapperPrimitiveSetIn(
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
        public void Set{memberInfo.MemberName}(in {memberInfo.TypeName} item)
        {{
            {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Set{memberInfo.MemberName}Value(in _start, in item);
        }}
");
        }

        private void WrapperPrimitiveSetPtr(
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
        public void Set{memberInfo.MemberName}(in {memberInfo.TypeName}* valuePtr)
        {{
            {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Set{memberInfo.MemberName}Value(in _start, in valuePtr);
        }}
");
        }

        private void WrapperPrimitiveGetOut(
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
        public void GetOut{memberInfo.MemberName}(out {memberInfo.TypeName} item)
        {{
            {containingType.ContainingNamespace}.{containingType.TypeName}Helper.GetOut{memberInfo.MemberName}Value(in _start, out item);
        }}
");
        }

        private void WrapperСompositeGetPtr(
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
        public void* {memberInfo.MemberName}Ptr
        {{
            get
            {{
                return {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
            }}
        }}
");
        }

        private void WrapperСompositeGetSet(
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo memberTypeInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
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
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo memberTypeInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
        public void Set{memberInfo.MemberName} (in void* valuePtr)
        {{
            var ptr = {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
            {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.Copy(in valuePtr, in ptr);
        }}
");
        }

        private void WrapperСompositeGetOut(
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo memberTypeInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
        public void GetOut{memberInfo.MemberName}(out {memberInfo.TypeName} item)
        {{
            var ptr = {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
            {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToValueOut(in ptr, out item);
        }}
");
        }

        private void WrapperСompositeSetIn(
            in Model.MemberInfo memberInfo,
            in Model.TypeInfo memberTypeInfo,
            in Model.TypeInfo containingType
            )
        {
            _builder.Append($@"
        public void Set{memberInfo.MemberName}(in {memberInfo.TypeName} item) 
        {{
            var ptr = {containingType.ContainingNamespace}.{containingType.TypeName}Helper.Get{memberInfo.MemberName}Ptr(in _start);
            {memberTypeInfo.ContainingNamespace}.{memberTypeInfo.TypeName}Helper.CopyToPtr(in item, in ptr);
        }}
");
        }

        private void PrimitiveWrapperChangePtr()
        {
            _builder.Append($@"
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

        private void WrapperEnd()
        {
            _builder.Append($@"
    }}
}}
");
        }
    }
}