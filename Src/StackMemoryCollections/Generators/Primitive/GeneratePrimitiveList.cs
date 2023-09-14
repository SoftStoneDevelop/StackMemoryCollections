using Microsoft.CodeAnalysis;
using System;
using System.Text;
using System.Threading;

namespace StackMemoryCollections.Generators.Primitive
{
    internal class PrimitiveListGenerator
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public void GeneratePrimitiveList(
            in SourceProductionContext context,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            GenerateList<IntPtr>(context, 0, true);

            cancellationToken.ThrowIfCancellationRequested();
            GenerateList<int>(context, 4, false);
            GenerateList<uint>(context, 4, false);

            cancellationToken.ThrowIfCancellationRequested();
            GenerateList<long>(context, 8, false);
            GenerateList<ulong>(context, 8, false);

            cancellationToken.ThrowIfCancellationRequested();
            GenerateList<sbyte>(context, 1, false);
            GenerateList<byte>(context, 1, false);

            cancellationToken.ThrowIfCancellationRequested();
            GenerateList<short>(context, 2, false);
            GenerateList<ushort>(context, 2, false);

            cancellationToken.ThrowIfCancellationRequested();
            GenerateList<char>(context, 2, false);
            GenerateList<decimal>(context, 16, false);
            GenerateList<double>(context, 8, false);
            GenerateList<bool>(context, 1, false);//1 byte is not optimal
            GenerateList<float>(context, 4, false);
        }

        private void GenerateList<T>(
            in SourceProductionContext context,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            var sizeOfStr = calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : sizeOf.ToString();

            GeneratePrimitiveList<T>(in context, "Class", sizeOf, sizeOfStr, false);
            GeneratePrimitiveList<T>(in context, "Struct", sizeOf, sizeOfStr, false);
        }

        private void GeneratePrimitiveList<T>(
            in SourceProductionContext context,
            in string ListNamespace,
            in int sizeOf,
            in string sizeOfStr,
            bool calculateSize
            ) where T : unmanaged
        {
            _builder.Clear();
            ListPrimitiveStart<T>(in ListNamespace);

            ListPrimitiveConstructor1<T>(in sizeOf, in sizeOfStr, calculateSize);
            ListPrimitiveConstructor2<T>(in sizeOfStr);
            ListPrimitiveConstructor3<T>(in sizeOfStr);
            ListPrimitiveConstructor4<T>();

            ListPrimitiveProperties<T>();

            ListPrimitiveReducingCapacity<T>(in sizeOfStr, in ListNamespace);
            ListPrimitiveExpandCapacity<T>(in sizeOfStr, in ListNamespace);
            ListPrimitiveTryExpandCapacity<T>(in sizeOfStr, in ListNamespace);
            ListPrimitiveTrimExcess();

            ListPrimitiveAddIn<T>(in ListNamespace);
            ListPrimitiveAddFuture(in ListNamespace);
            ListPrimitiveAddInPtr<T>(in ListNamespace);
            ListPrimitiveTryAddIn<T>(in ListNamespace);
            ListPrimitiveTryAddInPtr<T>(in ListNamespace);

            ListPrimitiveRemove<T>(in sizeOfStr, in ListNamespace);
            ListPrimitiveInsert<T>(in sizeOfStr, in ListNamespace);

            ListPrimitiveClear(in ListNamespace);

            ListPrimitiveGetByIndex<T>();
            ListPrimitiveGetByIndexRef<T>();
            ListPrimitiveGetOutByIndex<T>();

            ListPrimitiveGetFuture<T>();

            ListPrimitiveDispose<T>(in ListNamespace, in sizeOfStr);
            ListPrimitiveIndexator<T>();

            ListPrimitiveCopyCount<T>(in sizeOfStr);
            ListPrimitiveCopy<T>(in sizeOfStr);
            ListPrimitiveCopyInList<T>(in sizeOfStr);

            ListPrimitiveSetSize();

            if (ListNamespace == "Class")
            {
                ListPrimitiveIEnumerable<T>();
            }

            ListPrimitiveEnd();

            context.AddSource($"ListOf{typeof(T).Name}{ListNamespace}.g.cs", _builder.ToString());
        }

        private void ListPrimitiveStart<T>(
            in string listNamespace
            ) where T : unmanaged
        {
            string implements;
            if (listNamespace == "Class")
            {
                implements = $"IDisposable, System.Collections.Generic.IEnumerable<{typeof(T).Name}>";
            }
            else
            {
                implements = $"IDisposable";
            }

            _builder.Append($@"
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace StackMemoryCollections.{listNamespace}
{{
    public unsafe {listNamespace.ToLowerInvariant()} ListOf{typeof(T).Name} : {implements}
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private {typeof(T).Name}* _start;
        private readonly bool _memoryOwner = false;
");
            if (listNamespace == "Class")
            {
                _builder.Append($@"
        private int _version = 0;
");
            }
        }

        private void ListPrimitiveConstructor1<T>(
            in int sizeOf,
            in string sizeOfStr,
            in bool calculateSize
            ) where T : unmanaged
        {
            _builder.Append($@"
        public ListOf{typeof(T).Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({(calculateSize ? $"{sizeOfStr} * 4" : (sizeOf * 4).ToString())});
            _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void ListPrimitiveConstructor2<T>(
            in string sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public ListOf{typeof(T).Name}(
            nuint count,
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = ({typeof(T).Name}*)stackMemory->AllocateMemory({sizeOf} * count);
            _stackMemoryS = stackMemory;
            Capacity = count;
        }}
");
        }

        private void ListPrimitiveConstructor3<T>(
            in string sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public ListOf{typeof(T).Name}(
            nuint count,
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = ({typeof(T).Name}*)stackMemory.AllocateMemory({sizeOf} * count);
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
            Capacity = count;
        }}
");
        }

        private void ListPrimitiveConstructor4<T>() where T : unmanaged
        {
            _builder.Append($@"
        public ListOf{typeof(T).Name}(
            nuint count,
            {typeof(T).Name}* memoryStart
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

        private void ListPrimitiveProperties<T>() where T : unmanaged
        {
            _builder.Append($@"
        public nuint Capacity {{ get; private set; }}

        public nuint Size {{ get; private set; }} = 0;

        public bool IsEmpty => Size == 0;

        public {typeof(T).Name}* Start => _start;
");
        }

        private void ListPrimitiveReducingCapacity<T>(
            in string sizeOf,
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
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
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;{incrementVersion}
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

        private void ListPrimitiveExpandCapacity<T>(
            in string sizeOf,
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
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
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;{incrementVersion}
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

        private void ListPrimitiveTryExpandCapacity<T>(
            in string sizeOf,
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
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
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;{incrementVersion}
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

        private void ListPrimitiveTrimExcess()
        {
            _builder.Append($@"
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

        private void ListPrimitiveAddIn<T>(
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public void Add(in {typeof(T).Name} item)
        {{
            if (Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            *(_start + Size++) = item;{incrementVersion}
        }}
");
        }

        private void ListPrimitiveAddFuture(
            in string listNamespace
            )
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
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

        private void ListPrimitiveAddInPtr<T>(
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
    @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public void Add(in {typeof(T).Name}* ptr)
        {{
            if (Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            *(_start + Size++) = *ptr;{incrementVersion}
        }}
");
        }

        private void ListPrimitiveTryAddIn<T>(
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public bool TryAdd(in {typeof(T).Name} item)
        {{
            if (Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            *(_start + Size++) = item;{incrementVersion}
            return true;
        }}
");
        }

        private void ListPrimitiveTryAddInPtr<T>(
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
    @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public bool TryAdd(in {typeof(T).Name}* ptr)
        {{
            if (Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            *(_start + Size++) = *ptr;{incrementVersion}
            return true;
        }}
");
        }

        private void ListPrimitiveRemove<T>(
            in string sizeOf,
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
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
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            }}
            else if (_stackMemoryS != null)
            {{
                _stackMemoryS->ShiftLeft((byte*)(_start + (index + 1)), (byte*)(_start + Size), (long){sizeOf});
            }}
            else if (_stackMemoryC != null)
            {{
                _stackMemoryC.ShiftLeft((byte*)(_start + (index + 1)), (byte*)(_start + Size), (long){sizeOf});
            }}

            Size -= 1;{incrementVersion}
        }}
");
        }

        private void ListPrimitiveInsert<T>(
            in string sizeOf,
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public void Insert(in {typeof(T).Name} value, nuint index)
        {{
            if(index > Size || Size == Capacity)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            if (index == Size)
            {{
{incrementVersion}
                *(_start + Size++) = value;
                return;
            }}

            var needExtend = Size == Capacity;
            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOf} * (needExtend ? Capacity + 1 : Capacity));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if(index == 0)
                {{
                    *(({typeof(T).Name}*)newMemory.Start) = value;
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
                    *(({typeof(T).Name}*)newMemory.Start + index) = value;
                    Buffer.MemoryCopy(
                        _start + index,
                        ({typeof(T).Name}*)newMemory.Start + index + 1,
                        newMemory.ByteCount,
                        (Size - index) * {sizeOf}
                        );
                }}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            }}
            else if (_stackMemoryS != null)
            {{
                if(needExtend)
                {{
                    _stackMemoryS->AllocateMemory({sizeOf});
                }}

                _stackMemoryS->ShiftRight((byte*)(_start + index), (byte*)(_start + Size), (long){sizeOf});
                *(_start + index) = value;
            }}
            else if (_stackMemoryC != null)
            {{
                if (needExtend)
                {{
                    _stackMemoryC.AllocateMemory({sizeOf});
                }}

                _stackMemoryC.ShiftRight((byte*)(_start + index), (byte*)(_start + Size), (long){sizeOf});
                *(_start + index) = value;
            }}

            Size += 1;{incrementVersion}
        }}
");
        }

        private void ListPrimitiveClear(
            in string ListNamespace
            )
        {
            var incrementVersion = ListNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public void Clear()
        {{
            if (Size != 0)
            {{
                Size = 0;{incrementVersion}
            }}
        }}
");
        }

        private void ListPrimitiveGetByIndex<T>() where T : unmanaged
        {
            _builder.Append($@"
        public {typeof(T).Name} GetByIndex(nuint index)
        {{
            if (Size == 0 || index >= Size)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            return *(_start + index);
        }}
");
        }

        private void ListPrimitiveGetByIndexRef<T>() where T : unmanaged
        {
            _builder.Append($@"
        public void GetByIndex(nuint index, ref {typeof(T).Name} value)
        {{
            if (Size == 0 || index >= Size)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            value = *(_start + index);
        }}
");
        }

        private void ListPrimitiveGetOutByIndex<T>() where T : unmanaged
        {
            _builder.Append($@"
        public void GetOutByIndex(nuint index, out {typeof(T).Name} value)
        {{
            if (Size == 0 || index >= Size)
            {{
                throw new IndexOutOfRangeException(""Element outside the list"");
            }}

            value = *(_start + index);
        }}
");
        }

        private void ListPrimitiveGetFuture<T>() where T : unmanaged
        {
            _builder.Append($@"
        public {typeof(T).Name}* GetFuture()
        {{
            if (Capacity == 0 || Size == Capacity)
            {{
                throw new Exception(""Future element not available"");
            }}

            return _start + Size;
        }}
");
        }

        private void ListPrimitiveDispose<T>(
            in string listNamespace,
            in string sizeOf
            ) where T : unmanaged
        {
            if (listNamespace == "Class")
            {
                _builder.Append($@"
        #region IDisposable

        private bool _disposed;

        ~ListOf{typeof(T).Name}() => Dispose(false);

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
                _builder.Append($@"
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

        private void ListPrimitiveIndexator<T>() where T : unmanaged
        {
            _builder.Append($@"
        public {typeof(T).Name}* this[nuint index]
        {{
            get
            {{
                if (Size == 0 || Size <= index)
                {{
                    throw new IndexOutOfRangeException(""Element outside the list"");
                }}

                return _start + index;
            }}
        }}
");
        }

        private void ListPrimitiveIEnumerable<T>() where T : unmanaged
        {
            _builder.Append($@"
        #region IEnumerable<{typeof(T).Name}>

        public System.Collections.Generic.IEnumerator<{typeof(T).Name}> GetEnumerator()
        {{
            return new Enumerator(this);
        }}

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {{
            return new Enumerator(this);
        }}

        public struct Enumerator : System.Collections.Generic.IEnumerator<{typeof(T).Name}>, System.Collections.IEnumerator
        {{
            private readonly Class.ListOf{typeof(T).Name} _list;
            private {typeof(T).Name}* _current;
            private int _currentIndex;
            private int _version;

            internal Enumerator(Class.ListOf{typeof(T).Name} list)
            {{
                _list = list;
                _currentIndex = -1;
                _current = default;
                _version = _list._version;
            }}

            public {typeof(T).Name} Current => *_current;

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

                _current = _list._start + _currentIndex;
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

        private void ListPrimitiveCopy<T>(
            in string sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
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

        private void ListPrimitiveCopyCount<T>(
            in string sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
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

        private void ListPrimitiveCopyInList<T>(
            in string sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public void Copy(in Class.ListOf{typeof(T).Name} destList)
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

        private void ListPrimitiveSetSize()
        {
            _builder.Append($@"
        public void SetSize(in nuint size)
        {{
            Size = size;
        }}
");
        }

        private void ListPrimitiveEnd()
        {
            _builder.Append($@"
    }}
}}
");
        }
    }
}