using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StackMemoryCollections
{
    public partial class Generator
    {
        private void GenerateList(
            in List<INamedTypeSymbol> typeList,
            in GeneratorExecutionContext context,
            in Dictionary<string, TypeInfo> typeInfos,
            in StringBuilder builder
            )
        {
            for (int i = 0; i < typeList.Count; i++)
            {
                var currentType = typeList[i];
                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"{nameof(GenerateList)}: Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                GenerateList(in context, in builder, in currentType, in typeInfo, "Class");
                GenerateList(in context, in builder, in currentType, in typeInfo, "Struct");
            }
        }

        private void GenerateList(
            in GeneratorExecutionContext context,
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in TypeInfo typeInfo,
            in string listNamespace
            )
        {
            builder.Clear();
            var sizeOf = typeInfo.IsRuntimeCalculatedSize ? $"{currentType.Name}Helper.SizeOf" : $"{typeInfo.Size}";
            ListStart(in builder, in currentType, in listNamespace);

            ListConstructor1(in builder, in sizeOf, in currentType);
            ListConstructor2(in builder, in sizeOf, in currentType);
            ListConstructor3(in builder, in sizeOf, in currentType);
            ListConstructor4(in builder, in currentType);

            ListProperties(in builder);

            ListReducingCapacity(in builder, in sizeOf, in listNamespace);
            ListExpandCapacity(in builder, in sizeOf, in listNamespace);
            ListTryExpandCapacity(in builder, in sizeOf, in listNamespace);
            ListTrimExcess(in builder);

            ListAddIn(in builder, in listNamespace, in sizeOf, in currentType);
            ListAddFuture(in builder, in listNamespace);
            ListAddInPtr(in builder, in listNamespace, in sizeOf, in currentType);
            ListTryAddIn(in builder, in listNamespace, in sizeOf, in currentType);
            ListTryAddInPtr(in builder, in listNamespace, in sizeOf, in currentType);

            ListRemove(in builder, in sizeOf, in listNamespace);
            ListInsert(in builder, in sizeOf, in listNamespace, in currentType);

            ListClear(in builder, in listNamespace);

            ListGetByIndex(in builder, in typeInfo, in sizeOf);
            ListGetByIndexRef(in builder, in currentType, in sizeOf);
            ListGetOutByIndex(in builder, in currentType, in sizeOf);

            ListGetFuture(in builder, in sizeOf);

            ListDispose(in builder, in listNamespace, in sizeOf, in currentType);
            ListIndexator(in builder, in sizeOf);

            ListCopyCount(in builder, in sizeOf);
            ListCopy(in builder, in sizeOf);
            ListCopyInList(in builder, in sizeOf, in currentType);

            ListSetSize(in builder);

            if (listNamespace == "Class")
            {
                ListIEnumerable(in builder, in currentType, in sizeOf);
            }

            ListEnd(in builder);

            context.AddSource($"ListOf{currentType.Name}{listNamespace}.g.cs", builder.ToString());
        }

        private void ListStart(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string listNamespace
            )
        {
            string implements;
            if (listNamespace == "Class")
            {
                implements = $"IDisposable, System.Collections.Generic.IEnumerable<{currentType.Name}>";
            }
            else
            {
                implements = $"IDisposable";
            }

            builder.Append($@"
/*
{Resource.License}
*/

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using {currentType.ContainingNamespace};

namespace {currentType.ContainingNamespace}.{listNamespace}
{{
    public unsafe {listNamespace.ToLowerInvariant()} ListOf{currentType.Name} : {implements}
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private void* _start;
        private readonly bool _memoryOwner = false;
");
            if (listNamespace == "Class")
            {
                builder.Append($@"
        private int _version = 0;
");
            }
        }

        private void ListConstructor1(
            in StringBuilder builder,
            in string sizeOf,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
        public ListOf{currentType.Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({sizeOf} * 4);
            _start = _stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void ListConstructor2(
            in StringBuilder builder,
            in string sizeOf,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
        public ListOf{currentType.Name}(
            nuint count,
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory->AllocateMemory({sizeOf} * count);
            _stackMemoryS = stackMemory;
            Capacity = count;
        }}
");
        }

        private void ListConstructor3(
            in StringBuilder builder,
            in string sizeOf,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
        public ListOf{currentType.Name}(
            nuint count,
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory.AllocateMemory({sizeOf} * count);
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
            Capacity = count;
        }}
");
        }

        private void ListConstructor4(
            in StringBuilder builder,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
        public ListOf{currentType.Name}(
            nuint count,
            void* memoryStart
            )
        {{
            if (memoryStart == null)
            {{
                throw new ArgumentNullException(nameof(memoryStart));
            }}

            _start = memoryStart;
            _stackMemoryS = null;
            Capacity = count;
        }}
");
        }

        private void ListProperties(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public nuint Capacity {{ get; private set; }}

        public nuint Size {{ get; private set; }} = 0;

        public bool IsEmpty => Size == 0;

        public void* Start => _start;
");
        }

        private void ListReducingCapacity(
            in StringBuilder builder,
            in string sizeOf,
            in string listNamespace
            )
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void ReducingCapacity(in nuint reducingCount)
        {{
            if (reducingCount <= 0)
            {{
                return;
            }}

            if (Size == Capacity || Capacity - Size < reducingCount)
            {{
                throw new Exception(""Can't reduce available memory, it's already in use"");
            }}

            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOf} * (Capacity - reducingCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    Size * {sizeOf}
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;{incrementVersion}
            }}
            else if (_stackMemoryS != null)
            {{
                if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                {{
                    throw new Exception(""Failed to reduce available memory, stack moved further"");
                }}

                _stackMemoryS->FreeMemory(reducingCount * {sizeOf});
            }}
            else if (_stackMemoryC != null)
            {{
                if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                {{
                    throw new Exception(""Failed to reduce available memory, stack moved further"");
                }}

                _stackMemoryC.FreeMemory(reducingCount * {sizeOf});
            }}

            Capacity -= reducingCount;
        }}
");
        }

        private void ListExpandCapacity(
            in StringBuilder builder,
            in string sizeOf,
            in string listNamespace
            )
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void ExpandCapacity(in nuint expandCount)
        {{
            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOf} * (Capacity + expandCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    Size * {sizeOf}
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;{incrementVersion}
            }}
            else if (_stackMemoryS != null)
            {{
                if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                {{
                    throw new Exception(""Failed to expand available memory, stack moved further"");
                }}

                _stackMemoryS->AllocateMemory(expandCount * {sizeOf});
            }}
            else if (_stackMemoryC != null)
            {{
                if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                {{
                    throw new Exception(""Failed to expand available memory, stack moved further"");
                }}

                _stackMemoryC.AllocateMemory(expandCount * {sizeOf});
            }}

            Capacity += expandCount;
        }}
");
        }

        private void ListTryExpandCapacity(
            in StringBuilder builder,
            in string sizeOf,
            in string listNamespace
            )
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public bool TryExpandCapacity(in nuint expandCount)
        {{
            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOf} * (Capacity + expandCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    Size * {sizeOf}
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;{incrementVersion}
            }}
            else if (_stackMemoryS != null)
            {{
                if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                {{
                    return false;
                }}

                if (!_stackMemoryS->TryAllocateMemory(expandCount * {sizeOf}, out _))
                {{
                    return false;
                }}
            }}
            else if (_stackMemoryC != null)
            {{
                if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                {{
                    return false;
                }}

                if (!_stackMemoryC.TryAllocateMemory(expandCount * {sizeOf}, out _))
                {{
                    return false;
                }}
            }}

            Capacity += expandCount;
            return true;
        }}
");
        }

        private void ListTrimExcess(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public void TrimExcess()
        {{
            if (_memoryOwner)
            {{
                ReducingCapacity(
                    Size == 0 ?
                        Capacity > 4 ? (nuint)(-(4 - (long)Capacity))
                            : 0
                        : Capacity - Size
                        );
            }}
            else
            {{
                ReducingCapacity(Capacity - Size);
            }}
        }}
");
        }

        private void ListAddIn(
            in StringBuilder builder,
            in string listNamespace,
            in string sizeOf,
            in INamedTypeSymbol currentType
            )
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void Add(in {currentType.Name} item)
        {{
            if (Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (Size * {sizeOf}));
            Size += 1;{incrementVersion}
        }}
");
        }

        private void ListAddFuture(
            in StringBuilder builder,
            in string listNamespace
            )
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void AddFuture()
        {{
            if(Size == Capacity)
            {{
                throw new Exception(""Not enough memory to allocate list element"");
            }}

            Size += 1;{incrementVersion}
        }}
");
        }

        private void ListAddInPtr(
            in StringBuilder builder,
            in string listNamespace,
            in string sizeOf,
            in INamedTypeSymbol currentType
            )
        {
            var incrementVersion = listNamespace == "Class" ?
    @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void Add(in void* ptr)
        {{
            if (Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            {currentType.Name}Helper.Copy(in ptr, (byte*)_start + (Size * {sizeOf}));
            Size += 1;
{incrementVersion}
        }}
");
        }

        private void ListTryAddIn(
            in StringBuilder builder,
            in string listNamespace,
            in string sizeOf,
            in INamedTypeSymbol currentType
            )
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public bool TryAdd(in {currentType.Name} item)
        {{
            if (Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (Size * {sizeOf}));
            Size += 1;{incrementVersion}
            return true;
        }}
");
        }

        private void ListTryAddInPtr(
            in StringBuilder builder,
            in string listNamespace,
            in string sizeOf,
            in INamedTypeSymbol currentType
            )
        {
            var incrementVersion = listNamespace == "Class" ?
    @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public bool TryAdd(in void* ptr)
        {{
            if (Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            {currentType.Name}Helper.Copy(in ptr, (byte*)_start + (Size * {sizeOf}));
            Size += 1;{incrementVersion}
            return true;
        }}
");
        }

        private void ListRemove(
            in StringBuilder builder,
            in string sizeOf,
            in string listNamespace
            )
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void Remove(nuint index)
        {{
            if(Size == 0 || index >= Size)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            if(index == Size - 1)
            {{
                Size -= 1;{incrementVersion}
                return;
            }}

            if(_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOf} * Capacity);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    Size * {sizeOf}
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;
            }}
            else if (_stackMemoryS != null)
            {{
                _stackMemoryS->ShiftLeft((byte*)_start + ((index + 1) * {sizeOf}), (byte*)_start + (Size * {sizeOf}), (long){sizeOf});
            }}
            else if (_stackMemoryC != null)
            {{
                _stackMemoryC.ShiftLeft((byte*)_start + ((index + 1) * {sizeOf}), (byte*)_start + (Size * {sizeOf}), (long){sizeOf});
            }}

            Size -= 1;{incrementVersion}
        }}
");
        }

        private void ListInsert(
            in StringBuilder builder,
            in string sizeOf,
            in string listNamespace,
            in INamedTypeSymbol currentType
            )
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void Insert(in {currentType.Name} value, nuint index)
        {{
            if(index > Size || Size == Capacity)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            if (index == Size)
            {{
{incrementVersion}
                {currentType.Name}Helper.CopyToPtr(in value, (byte*)_start + (Size * {sizeOf}));
                Size += 1;
                return;
            }}

            var needExtend = Size == Capacity;
            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOf} * (needExtend ? Capacity + 1 : Capacity));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if(index == 0)
                {{
                    {currentType.Name}Helper.CopyToPtr(in value, (byte*)newMemory.Start);
                    Buffer.MemoryCopy(
                        _start,
                        (byte*)newMemory.Start + {sizeOf},
                        newMemory.ByteCount - {sizeOf},
                        Size * {sizeOf}
                        );
                }}
                else
                {{
                    Buffer.MemoryCopy(
                        _start,
                        newMemory.Start,
                        newMemory.ByteCount,
                        index * {sizeOf}
                        );
                    {currentType.Name}Helper.CopyToPtr(in value, (byte*)newMemory.Start + (index * {sizeOf}));
                    Buffer.MemoryCopy(
                        (byte*)_start + (index * {sizeOf}),
                        (byte*)newMemory.Start + ((index + 1) * {sizeOf}),
                        newMemory.ByteCount,
                        (Size - index) * {sizeOf}
                        );
                }}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;
            }}
            else if (_stackMemoryS != null)
            {{
                if(needExtend)
                {{
                    _stackMemoryS->AllocateMemory({sizeOf});
                }}

                _stackMemoryS->ShiftRight((byte*)_start + (index * {sizeOf}), (byte*)_start + (Size * {sizeOf}), (long){sizeOf});
                {currentType.Name}Helper.CopyToPtr(in value, (byte*)_start + (index * {sizeOf}));
            }}
            else if (_stackMemoryC != null)
            {{
                if (needExtend)
                {{
                    _stackMemoryC.AllocateMemory({sizeOf});
                }}

                _stackMemoryC.ShiftRight((byte*)_start + (index * {sizeOf}), (byte*)_start + (Size * {sizeOf}), (long){sizeOf});
                {currentType.Name}Helper.CopyToPtr(in value, (byte*)_start + (index * {sizeOf}));
            }}

            Size += 1;{incrementVersion}
        }}
");
        }

        private void ListClear(
            in StringBuilder builder,
            in string ListNamespace
            )
        {
            var incrementVersion = ListNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void Clear()
        {{
            if (Size != 0)
            {{
                Size = 0;{incrementVersion}
            }}
        }}
");
        }

        private void ListGetByIndex(
            in StringBuilder builder,
            in TypeInfo typeInfo,
            in string sizeOf
            )
        {
            if(typeInfo.IsValueType)
            {
                builder.Append($@"
        [SkipLocalsInit]
        public {typeInfo.TypeName} GetByIndex(nuint index)
        {{
            if (Size == 0 || index >= Size)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            {typeInfo.TypeName} result;
            Unsafe.SkipInit(out result);
            {typeInfo.TypeName}Helper.CopyToValue((byte*)_start + (index * {sizeOf}), ref result);

            return result;
        }}
");
            }
            else
            {
                builder.Append($@"
        public {typeInfo.TypeName} GetByIndex(nuint index)
        {{
            if (Size == 0 || index >= Size)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            {typeInfo.TypeName} result = new {typeInfo.TypeName}();
            {typeInfo.TypeName}Helper.CopyToValue((byte*)_start + (index * {sizeOf}), ref result);

            return result;
        }}
");
            }
        }

        private void ListGetByIndexRef(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            builder.Append($@"
        public void GetByIndex(nuint index, ref {currentType.Name} value)
        {{
            if (Size == 0 || index >= Size)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            {currentType.Name}Helper.CopyToValue((byte*)_start + (index * {sizeOf}), ref value);
        }}
");
        }

        private void ListGetOutByIndex(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            builder.Append($@"
        public void GetOutByIndex(nuint index, out {currentType.Name} value)
        {{
            if (Size == 0 || index >= Size)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            {currentType.Name}Helper.CopyToValueOut((byte*)_start + (index * {sizeOf}), out value);
        }}
");
        }

        private void ListGetFuture(
            in StringBuilder builder,
            in string sizeOf
            )
        {
            builder.Append($@"
        public void* GetFuture()
        {{
            if (Capacity == 0 || Size == Capacity)
            {{
                throw new Exception(""Future element not available"");
            }}

            return (byte*)_start + (Size * {sizeOf});
        }}
");
        }

        private void ListDispose(
            in StringBuilder builder,
            in string listNamespace,
            in string sizeOf,
            in INamedTypeSymbol currentType
            )
        {
            if (listNamespace == "Class")
            {
                builder.Append($@"
        #region IDisposable

        private bool _disposed;

        ~ListOf{currentType.Name}() => Dispose(false);

        public void Dispose()
        {{
            Dispose(true);
            GC.SuppressFinalize(this);
        }}

        protected virtual void Dispose(bool disposing)
        {{
            if (!_disposed)
            {{
                if (!_memoryOwner)
                {{
                    if (disposing)
                    {{
                        if(_stackMemoryS != null)
                        {{
                            _stackMemoryS->FreeMemory(Capacity * {sizeOf});
                        }}
                        else if (_stackMemoryC != null)
                        {{
                            _stackMemoryC.FreeMemory(Capacity * {sizeOf});
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
                if(_stackMemoryS != null)
                {{
                    _stackMemoryS->FreeMemory(Capacity * {sizeOf});
                }}
                else if (_stackMemoryC != null)
                {{
                    _stackMemoryC.FreeMemory(Capacity * {sizeOf});
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

        private void ListIndexator(
            in StringBuilder builder,
            in string sizeOf
            )
        {
            builder.Append($@"
        public void* this[nuint index]
        {{
            get
            {{
                if (Size == 0 || Size <= index)
                {{
                    throw new IndexOutOfRangeException(""Element outside the list"");
                }}

                return (byte*)_start + (index * {sizeOf});
            }}
        }}
");
        }

        private void ListIEnumerable(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            builder.Append($@"
        #region IEnumerable<{currentType.Name}>

        public System.Collections.Generic.IEnumerator<{currentType.Name}> GetEnumerator()
        {{
            return new Enumerator(this);
        }}

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {{
            return new Enumerator(this);
        }}

        public struct Enumerator : System.Collections.Generic.IEnumerator<{currentType.Name}>, System.Collections.IEnumerator
        {{
            private readonly Class.ListOf{currentType.Name} _list;
            private void* _current;
            private int _currentIndex;
            private int _version;

            internal Enumerator(Class.ListOf{currentType.Name} list)
            {{
                _list = list;
                _currentIndex = -1;
                _current = default;
                _version = _list._version;
            }}

            public {currentType.Name} Current 
            {{
                get
                {{
                    {currentType.Name} result = new {currentType.Name}();
                    {currentType.Name}Helper.CopyToValue(_current, ref result);
                    return result;
                }}
            }}

            object System.Collections.IEnumerator.Current => Current;

            public void Dispose()
            {{
                _currentIndex = -1;
            }}

            public bool MoveNext()
            {{
                if (_version != _list._version)
                {{
                    throw new InvalidOperationException(""The stack was changed during the enumeration"");
                }}

                if (_list.Size == 0)
                {{
                    return false;
                }}

                if (_currentIndex == -2)
                {{
                    _currentIndex = 0;
                    _current = _list._start;
                    return true;
                }}

                if (_currentIndex == -1)
                {{
                    return false;
                }}

                if ((nuint)(++_currentIndex) >= _list.Size)
                {{
                    _current = default;
                    return false;
                }}

                _current = (byte*)_list._start + (_currentIndex * (int){sizeOf});
                return true;
            }}

            public void Reset()
            {{
                _currentIndex = -2;
            }}
        }}

        #endregion
");
        }

        private void ListCopy(
            in StringBuilder builder,
            in string sizeOf
            )
        {
            builder.Append($@"
        public void Copy(in void* ptrDest, in nuint count)
        {{
            if (Size < count)
            {{
                throw new Exception(""The collection does not have that many elements"");
            }}

            Buffer.MemoryCopy(
                _start,
                ptrDest,
                count * (nuint){sizeOf},
                count * (nuint){sizeOf}
                );
        }}
");
        }

        private void ListCopyCount(
            in StringBuilder builder,
            in string sizeOf
            )
        {
            builder.Append($@"
        public void Copy(in void* ptrDest)
        {{
            if(Size == 0)
            {{
                return;
            }}

            Buffer.MemoryCopy(
                _start,
                ptrDest,
                Size * (nuint){sizeOf},
                Size * (nuint){sizeOf}
                );
        }}
");
        }

        private void ListCopyInList(
            in StringBuilder builder,
            in string sizeOf,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
        public void Copy(in Class.ListOf{currentType.Name} destList)
        {{
            if(Size == 0)
            {{
                destList.SetSize(0);
                return;
            }}

            if (destList.Capacity < Size)
            {{
                throw new ArgumentException(""Destination list not enough capacity"");
            }}

            Buffer.MemoryCopy(
                _start,
                destList.Start,
                destList.Capacity * {sizeOf},
                Size * {sizeOf}
                );

            destList.SetSize(Size);
        }}
");
        }

        private void ListSetSize(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public void SetSize(in nuint size)
        {{
            Size = size;
        }}
");
        }

        private void ListEnd(
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