﻿using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace StackMemoryCollections
{
    public partial class Generator
    {
        private void GeneratePrimitiveList(
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            GeneratePrimitiveList<IntPtr>(in context, in builder, "Class", 0, true);
            GeneratePrimitiveList<IntPtr>(in context, in builder, "Struct", 0, true);

            GeneratePrimitiveList<Int32>(in context, in builder, "Class", 4, false);
            GeneratePrimitiveList<Int32>(in context, in builder, "Struct", 4, false);

            GeneratePrimitiveList<UInt32>(in context, in builder, "Class", 4, false);
            GeneratePrimitiveList<UInt32>(in context, in builder, "Struct", 4, false);

            GeneratePrimitiveList<Int64>(in context, in builder, "Class", 8, false);
            GeneratePrimitiveList<Int64>(in context, in builder, "Struct", 8, false);

            GeneratePrimitiveList<UInt64>(in context, in builder, "Class", 8, false);
            GeneratePrimitiveList<UInt64>(in context, in builder, "Struct", 8, false);

            GeneratePrimitiveList<SByte>(in context, in builder, "Class", 1, false);
            GeneratePrimitiveList<SByte>(in context, in builder, "Struct", 1, false);

            GeneratePrimitiveList<Byte>(in context, in builder, "Class", 1, false);
            GeneratePrimitiveList<Byte>(in context, in builder, "Struct", 1, false);

            GeneratePrimitiveList<Int16>(in context, in builder, "Class", 2, false);
            GeneratePrimitiveList<Int16>(in context, in builder, "Struct", 2, false);

            GeneratePrimitiveList<UInt16>(in context, in builder, "Class", 2, false);
            GeneratePrimitiveList<UInt16>(in context, in builder, "Struct", 2, false);

            GeneratePrimitiveList<Char>(in context, in builder, "Class", 2, false);
            GeneratePrimitiveList<Char>(in context, in builder, "Struct", 2, false);

            GeneratePrimitiveList<Decimal>(in context, in builder, "Class", 16, false);
            GeneratePrimitiveList<Decimal>(in context, in builder, "Struct", 16, false);

            GeneratePrimitiveList<Double>(in context, in builder, "Class", 8, false);
            GeneratePrimitiveList<Double>(in context, in builder, "Struct", 8, false);

            GeneratePrimitiveList<Boolean>(in context, in builder, "Class", 1, false);//1 byte is not optimal
            GeneratePrimitiveList<Boolean>(in context, in builder, "Struct", 1, false);//1 byte is not optimal

            GeneratePrimitiveList<Single>(in context, in builder, "Class", 4, false);
            GeneratePrimitiveList<Single>(in context, in builder, "Struct", 4, false);
        }

        private void GeneratePrimitiveList<T>(
            in GeneratorExecutionContext context,
            in StringBuilder builder,
            in string ListNamespace,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            var sizeOfStr = calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : sizeOf.ToString();
            builder.Clear();
            ListPrimitiveStart<T>(in builder, in ListNamespace);

            ListPrimitiveConstructor1<T>(in builder, in sizeOf, in sizeOfStr, calculateSize);
            ListPrimitiveConstructor2<T>(in builder, in sizeOfStr);
            ListPrimitiveConstructor3<T>(in builder, in sizeOfStr);
            ListPrimitiveConstructor4<T>(in builder);

            ListPrimitiveProperties<T>(in builder);

            ListPrimitiveReducingCapacity<T>(in builder, in sizeOfStr, in ListNamespace);
            ListPrimitiveExpandCapacity<T>(in builder, in sizeOfStr, in ListNamespace);
            ListPrimitiveTryExpandCapacity<T>(in builder, in sizeOfStr, in ListNamespace);
            ListPrimitiveTrimExcess(in builder);

            ListPrimitiveAddIn<T>(in builder, in ListNamespace);
            ListPrimitiveAddFuture(in builder, in ListNamespace);
            ListPrimitiveAddInPtr<T>(in builder, in ListNamespace);
            ListPrimitiveTryAddIn<T>(in builder, in ListNamespace);
            ListPrimitiveTryAddInPtr<T>(in builder, in ListNamespace);

            ListPrimitiveRemove<T>(in builder, in sizeOfStr, in ListNamespace);
            ListPrimitiveInsert<T>(in builder, in sizeOfStr, in ListNamespace);

            ListPrimitiveClear(in builder, in ListNamespace);

            ListPrimitiveGetByIndex<T>(in builder);
            ListPrimitiveGetByIndexRef<T>(in builder);
            ListPrimitiveGetOutByIndex<T>(in builder);

            ListPrimitiveGetFuture<T>(in builder);

            ListPrimitiveDispose<T>(in builder, in ListNamespace, in sizeOfStr);
            ListPrimitiveIndexator<T>(in builder);

            ListPrimitiveCopyCount<T>(in builder, in sizeOfStr);
            ListPrimitiveCopy<T>(in builder, in sizeOfStr);
            ListPrimitiveCopyInList<T>(in builder, in sizeOfStr);

            ListPrimitiveSetSize(in builder);

            if (ListNamespace == "Class")
            {
                ListPrimitiveIEnumerable<T>(in builder);
            }

            ListPrimitiveEnd(in builder);

            context.AddSource($"ListOf{typeof(T).Name}{ListNamespace}.g.cs", builder.ToString());
        }

        private void ListPrimitiveStart<T>(
            in StringBuilder builder,
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

            builder.Append($@"
/*
{Resource.License}
*/

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
                builder.Append($@"
        private int _version = 0;
");
            }
        }

        private void ListPrimitiveConstructor1<T>(
            in StringBuilder builder,
            in int sizeOf,
            in string sizeOfStr,
            in bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public ListOf{typeof(T).Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({(calculateSize ? sizeOfStr : (sizeOf * 4).ToString())});
            _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void ListPrimitiveConstructor2<T>(
            in StringBuilder builder,
            in string sizeOf
            ) where T : unmanaged
        {
            builder.Append($@"
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
            in StringBuilder builder,
            in string sizeOf
            ) where T: unmanaged
        {
            builder.Append($@"
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

        private void ListPrimitiveConstructor4<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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

        private void ListPrimitiveProperties<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public nuint Capacity {{ get; private set; }}

        public nuint Size {{ get; private set; }} = 0;

        public bool IsEmpty => Size == 0;

        public {typeof(T).Name}* Start => _start;
");
        }

        private void ListPrimitiveReducingCapacity<T>(
            in StringBuilder builder,
            in string sizeOf,
            in string listNamespace
            ) where T :unmanaged
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
            in StringBuilder builder,
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
            in StringBuilder builder,
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

        private void ListPrimitiveTrimExcess(
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

        private void ListPrimitiveAddIn<T>(
            in StringBuilder builder,
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
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

        private void ListPrimitiveAddInPtr<T>(
            in StringBuilder builder,
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
    @"
                _version += 1;
"
:
"";
            builder.Append($@"
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
            in StringBuilder builder,
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
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
            in StringBuilder builder,
            in string listNamespace
            ) where T : unmanaged
        {
            var incrementVersion = listNamespace == "Class" ?
    @"
                _version += 1;
"
:
"";
            builder.Append($@"
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
            in StringBuilder builder,
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
            in StringBuilder builder,
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
            builder.Append($@"
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

        private void ListPrimitiveGetByIndex<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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

        private void ListPrimitiveGetByIndexRef<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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

        private void ListPrimitiveGetOutByIndex<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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

        private void ListPrimitiveGetFuture<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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
            in StringBuilder builder,
            in string listNamespace,
            in string sizeOf
            ) where T : unmanaged
        {
            if(listNamespace == "Class")
            {
                builder.Append($@"
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

        private void ListPrimitiveIndexator<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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

        private void ListPrimitiveIEnumerable<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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
            in StringBuilder builder,
            in string sizeOf
            ) where T : unmanaged
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

        private void ListPrimitiveCopyCount<T>(
            in StringBuilder builder,
            in string sizeOf
            ) where T : unmanaged
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

        private void ListPrimitiveCopyInList<T>(
            in StringBuilder builder,
            in string sizeOf
            ) where T : unmanaged
        {
            builder.Append($@"
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

        private void ListPrimitiveSetSize(
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

        private void ListPrimitiveEnd(
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