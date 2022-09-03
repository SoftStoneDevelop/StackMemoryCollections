using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Text;

namespace StackMemoryCollections
{
    public partial class Generator
    {
        private void GeneratePrimitiveQueue(
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            GeneratePrimitiveQueue<IntPtr>(in context, in builder, "Class", 0, true);
            GeneratePrimitiveQueue<IntPtr>(in context, in builder, "Struct", 0, true);

            GeneratePrimitiveQueue<Int32>(in context, in builder, "Class", 4, false);
            GeneratePrimitiveQueue<Int32>(in context, in builder, "Struct", 4, false);

            GeneratePrimitiveQueue<UInt32>(in context, in builder, "Class", 4, false);
            GeneratePrimitiveQueue<UInt32>(in context, in builder, "Struct", 4, false);

            GeneratePrimitiveQueue<Int64>(in context, in builder, "Class", 8, false);
            GeneratePrimitiveQueue<Int64>(in context, in builder, "Struct", 8, false);

            GeneratePrimitiveQueue<UInt64>(in context, in builder, "Class", 8, false);
            GeneratePrimitiveQueue<UInt64>(in context, in builder, "Struct", 8, false);

            GeneratePrimitiveQueue<SByte>(in context, in builder, "Class", 1, false);
            GeneratePrimitiveQueue<SByte>(in context, in builder, "Struct", 1, false);

            GeneratePrimitiveQueue<Byte>(in context, in builder, "Class", 1, false);
            GeneratePrimitiveQueue<Byte>(in context, in builder, "Struct", 1, false);

            GeneratePrimitiveQueue<Int16>(in context, in builder, "Class", 2, false);
            GeneratePrimitiveQueue<Int16>(in context, in builder, "Struct", 2, false);

            GeneratePrimitiveQueue<UInt16>(in context, in builder, "Class", 2, false);
            GeneratePrimitiveQueue<UInt16>(in context, in builder, "Struct", 2, false);

            GeneratePrimitiveQueue<Char>(in context, in builder, "Class", 2, false);
            GeneratePrimitiveQueue<Char>(in context, in builder, "Struct", 2, false);

            GeneratePrimitiveQueue<Decimal>(in context, in builder, "Class", 16, false);
            GeneratePrimitiveQueue<Decimal>(in context, in builder, "Struct", 16, false);

            GeneratePrimitiveQueue<Double>(in context, in builder, "Class", 8, false);
            GeneratePrimitiveQueue<Double>(in context, in builder, "Struct", 8, false);

            GeneratePrimitiveQueue<Boolean>(in context, in builder, "Class", 1, false);//1 byte is not optimal
            GeneratePrimitiveQueue<Boolean>(in context, in builder, "Struct", 1, false);//1 byte is not optimal

            GeneratePrimitiveQueue<Single>(in context, in builder, "Class", 4, false);
            GeneratePrimitiveQueue<Single>(in context, in builder, "Struct", 4, false);
        }

        private void GeneratePrimitiveQueue<T>(
            in GeneratorExecutionContext context,
            in StringBuilder builder,
            in string queueNamespace,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            var sizeOfStr = calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : sizeOf.ToString();
            builder.Clear();
            QueuePrimitiveStart<T>(in builder, in queueNamespace);

            QueuePrimitiveConstructor1<T>(in builder, in sizeOf, in sizeOfStr, calculateSize);
            QueuePrimitiveConstructor2<T>(in builder, in sizeOfStr);
            QueuePrimitiveConstructor3<T>(in builder, in sizeOfStr);
            QueuePrimitiveConstructor4<T>(in builder);

            QueuePrimitiveProperties<T>(in builder);

            QueuePrimitiveReducingCapacity<T>(in builder, in sizeOfStr, in queueNamespace);
            QueuePrimitiveExpandCapacity<T>(in builder, in sizeOfStr, in queueNamespace);
            QueuePrimitiveTryExpandCapacity<T>(in builder, in sizeOfStr, in queueNamespace);
            QueuePrimitiveTrimExcess(in builder);

            QueuePrimitivePushIn<T>(in builder, in queueNamespace);
            QueuePrimitivePushFuture(in builder, in queueNamespace);
            QueuePrimitivePushInPtr<T>(in builder, in queueNamespace);
            QueuePrimitiveTryPushIn<T>(in builder, in queueNamespace);
            QueuePrimitiveTryPushInPtr<T>(in builder, in queueNamespace);

            QueuePrimitivePop(in builder, in queueNamespace);
            QueuePrimitiveTryPop(in builder, in queueNamespace);

            QueuePrimitiveClear(in builder, in queueNamespace);

            QueuePrimitiveFront<T>(in builder);
            QueuePrimitiveBack<T>(in builder);

            QueuePrimitiveFrontInPtr<T>(in builder);
            QueuePrimitiveBackInPtr<T>(in builder);

            QueuePrimitiveFrontRefValue<T>(in builder);
            QueuePrimitiveBackRefValue<T>(in builder);

            QueuePrimitiveFrontPtr<T>(in builder);
            QueuePrimitiveBackPtr<T>(in builder);

            QueuePrimitiveBackFuture<T>(in builder);

            QueuePrimitiveFrontOutValue<T>(in builder);
            QueuePrimitiveBackOutValue<T>(in builder);

            QueuePrimitiveDispose<T>(in builder, in queueNamespace, in sizeOfStr);
            QueuePrimitiveIndexator<T>(in builder);
            QueuePrimitiveCopyCount<T>(in builder, in sizeOfStr);
            QueuePrimitiveCopy<T>(in builder, in sizeOfStr);
            QueuePrimitiveCopyInQueue<T>(in builder, in sizeOfStr);

            QueuePrimitiveSetPositions(in builder);
            QueuePrimitiveGetPositions(in builder);

            if (queueNamespace == "Class")
            {
                QueuePrimitiveIEnumerable<T>(in builder);
            }

            QueuePrimitiveEnd(in builder);

            context.AddSource($"QueueOf{typeof(T).Name}{queueNamespace}.g.cs", builder.ToString());
        }

        private void QueuePrimitiveStart<T>(
            in StringBuilder builder,
            in string queueNamespace
            ) where T : unmanaged
        {
            string implements;
            if (queueNamespace == "Class")
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

namespace StackMemoryCollections.{queueNamespace}
{{
    public unsafe {queueNamespace.ToLowerInvariant()} QueueOf{typeof(T).Name} : {implements}
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private {typeof(T).Name}* _start;
        private readonly bool _memoryOwner = false;
        private nuint _head = 0;
        private nuint _tail = 0;
");
            if (queueNamespace == "Class")
            {
                builder.Append($@"
        private int _version = 0;
");
            }
        }

        private void QueuePrimitiveConstructor1<T>(
            in StringBuilder builder,
            in int sizeOf,
            in string sizeOfStr,
            in bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public QueueOf{typeof(T).Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({(calculateSize ? sizeOfStr : (sizeOf * 4).ToString())});
            _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void QueuePrimitiveConstructor2<T>(
            in StringBuilder builder,
            in string sizeOf
            ) where T : unmanaged
        {
            builder.Append($@"
        public QueueOf{typeof(T).Name}(
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

        private void QueuePrimitiveConstructor3<T>(
            in StringBuilder builder,
            in string sizeOf
            ) where T: unmanaged
        {
            builder.Append($@"
        public QueueOf{typeof(T).Name}(
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

        private void QueuePrimitiveConstructor4<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public QueueOf{typeof(T).Name}(
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

        private void QueuePrimitiveProperties<T>(
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

        private void QueuePrimitiveReducingCapacity<T>(
            in StringBuilder builder,
            in string sizeOf,
            in string queueNamespace
            ) where T :unmanaged
        {
            var incrementVersion = queueNamespace == "Class" ?
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
                if (Size == 0)
                {{
                    _tail = 0;
                    _head = 0;
                }}
                else if (_tail > _head)
                {{
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * {sizeOf}),
                        newMemory.Start,
                        newMemory.ByteCount,
                        Size * {sizeOf}
                        );
                    _tail = Size - 1;
                    _head = 0;
                }}
                else
                {{
                    var headToEndByteCount = (Capacity - _head) * {sizeOf};
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * {sizeOf}),
                        newMemory.Start,
                        newMemory.ByteCount,
                        headToEndByteCount
                        );
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start,
                        (byte*)newMemory.Start + headToEndByteCount,
                        newMemory.ByteCount - headToEndByteCount,
                        (_tail + 1) * {sizeOf}
                        );
                    _tail = Size - 1;
                    _head = 0;
                }}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;{incrementVersion}
            }}
            else
            {{
                if (_stackMemoryS != null)
                {{
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                    {{
                        throw new Exception(""Failed to reduce available memory, stack moved further"");
                    }}

                    if(Size == 0)
                    {{
                        _head = 0;
                        _tail = 0;
                    }}
                    else
                    if(_head > _tail)
                    {{
                        _stackMemoryS->ShiftLeft((byte*)(_start + _head), (byte*)(_start + (Capacity)), (long)(reducingCount * {sizeOf}));
                        _head -= reducingCount;{incrementVersion}
                    }}
                    else
                    if (_head <= _tail)
                    {{
                        if(Size == 1)
                        {{
                            *(_start) = *(_start + _tail);
                            _tail = 0;
                            _head = 0;
                        }}
                        else
                        {{
                            var freeCountToEnd = Capacity - (_tail + 1);
                            if(freeCountToEnd == 0 || freeCountToEnd < reducingCount)
                            {{
                                _stackMemoryS->ShiftLeft((byte*)(_start + _head), (byte*)(_start + (_tail + 1)), (long)((Capacity - freeCountToEnd - ((_tail + 1) - _head)) * {sizeOf}));
                                _head = 0;
                                _tail = Size - 1;{incrementVersion}
                            }}
                        }}
                    }}
                    
                    _stackMemoryS->FreeMemory(reducingCount * {sizeOf});
                }}
                else if (_stackMemoryC != null)
                {{
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                    {{
                        throw new Exception(""Failed to reduce available memory, stack moved further"");
                    }}

                    if(Size == 0)
                    {{
                        _head = 0;
                        _tail = 0;
                    }}
                    else
                    if(_head > _tail)
                    {{
                        _stackMemoryC.ShiftLeft((byte*)(_start + _head), (byte*)(_start + (Capacity * {sizeOf})), (long)(reducingCount * {sizeOf}));
                        _head -= reducingCount;{incrementVersion}
                    }}
                    else
                    if (_head <= _tail)
                    {{
                        if(Size == 1)
                        {{
                            *(_start) = *(_start + _tail);
                            _tail = 0;
                            _head = 0;
                        }}
                        else
                        {{
                            var freeCountToEnd = Capacity - (_tail + 1);
                            if(freeCountToEnd == 0 || freeCountToEnd < reducingCount)
                            {{
                                _stackMemoryC.ShiftLeft((byte*)(_start + _head), (byte*)(_start + (_tail + 1)), (long)((Capacity - freeCountToEnd - ((_tail + 1) - _head)) * {sizeOf}));
                                _head = 0;
                                _tail = Size - 1;{incrementVersion}
                            }}
                        }}
                    }}                    

                    _stackMemoryC.FreeMemory(reducingCount * {sizeOf});
                }}
            }}

            Capacity -= reducingCount;
        }}
");
        }

        private void QueuePrimitiveExpandCapacity<T>(
            in StringBuilder builder,
            in string sizeOf,
            in string queueNamespace
            ) where T : unmanaged
        {
            var incrementVersion = queueNamespace == "Class" ?
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
                if(Size == 0)
                {{
                    _tail = 0;
                    _head = 0;
                }}
                else if(_tail > _head)
                {{
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * {sizeOf}),
                        newMemory.Start,
                        newMemory.ByteCount,
                        Size * {sizeOf}
                        );
                    _tail = Size - 1;
                    _head = 0;
                }}
                else
                {{
                    var headToEndByteCount = (Capacity - _head) * {sizeOf};
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * {sizeOf}),
                        newMemory.Start,
                        newMemory.ByteCount,
                        headToEndByteCount
                        );
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start,
                        (byte*)newMemory.Start + headToEndByteCount,
                        newMemory.ByteCount - headToEndByteCount,
                        (_tail + 1) * {sizeOf}
                        );
                    _tail = Size - 1;
                    _head = 0;
                }}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;{incrementVersion}
            }}
            else
            {{
                if (_stackMemoryS != null)
                {{
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                    {{
                        throw new Exception(""Failed to expand available memory, stack moved further"");
                    }}

                    _stackMemoryS->AllocateMemory(expandCount * {sizeOf});
                    if(Size != 0 && _head != 0 && _head > _tail)
                    {{
                        _stackMemoryS->ShiftRight((byte*)(_start + _head), (byte*)(_start + (_head + (Capacity - _head))), (long)(expandCount * {sizeOf}));{incrementVersion}
                        _head += expandCount;
                    }}
                }}
                else if (_stackMemoryC != null)
                {{
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                    {{
                        throw new Exception(""Failed to expand available memory, stack moved further"");
                    }}

                    _stackMemoryC.AllocateMemory(expandCount * {sizeOf});
                    if (Size != 0 && _head != 0 && _head > _tail)
                    {{
                        _stackMemoryC.ShiftRight((byte*)(_start + _head), (byte*)(_start + (_head + (Capacity - _head))), (long)(expandCount * {sizeOf}));{incrementVersion}
                        _head += expandCount;
                    }}
                }}
            }}

            Capacity += expandCount;
        }}
");
        }

        private void QueuePrimitiveTryExpandCapacity<T>(
            in StringBuilder builder,
            in string sizeOf,
            in string queueNamespace
            ) where T : unmanaged
        {
            var incrementVersion = queueNamespace == "Class" ?
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
                if(Size == 0)
                {{
                    _tail = 0;
                    _head = 0;
                }}
                else if(_tail > _head)
                {{
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * {sizeOf}),
                        newMemory.Start,
                        newMemory.ByteCount,
                        Size * {sizeOf}
                        );
                    _tail = Size - 1;
                    _head = 0;
                }}
                else
                {{
                    var headToEndByteCount = (Capacity - _head) * {sizeOf};
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * {sizeOf}),
                        newMemory.Start,
                        newMemory.ByteCount,
                        headToEndByteCount
                        );
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start,
                        (byte*)newMemory.Start + headToEndByteCount,
                        newMemory.ByteCount - headToEndByteCount,
                        (_tail + 1) * {sizeOf}
                        );
                    _tail = Size - 1;
                    _head = 0;
                }}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;{incrementVersion}
            }}
            else
            {{
                if (_stackMemoryS != null)
                {{
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                    {{
                        return false;
                    }}

                    if(!_stackMemoryS->TryAllocateMemory(expandCount * {sizeOf}, out _))
                    {{
                        return false;
                    }}

                    if(Size != 0 && _head != 0 && _head > _tail)
                    {{
                        _stackMemoryS->ShiftRight((byte*)(_start + _head), (byte*)(_start + (_head + (Capacity - _head))), (long)(expandCount * {sizeOf}));{incrementVersion}
                        _head += expandCount;
                    }}
                }}
                else if (_stackMemoryC != null)
                {{
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                    {{
                        return false;
                    }}

                    if(!_stackMemoryC.TryAllocateMemory(expandCount * {sizeOf}, out _))
                    {{
                        return false;
                    }}
                    
                    if (Size != 0 && _head != 0 && _head > _tail)
                    {{
                        _stackMemoryC.ShiftRight((byte*)(_start + _head), (byte*)(_start + (_head + (Capacity - _head))), (long)(expandCount * {sizeOf}));{incrementVersion}
                        _head += expandCount;
                    }}
                }}
            }}

            Capacity += expandCount;
            return true;
        }}
");
        }

        private void QueuePrimitiveTrimExcess(
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

        private void QueuePrimitivePushIn<T>(
            in StringBuilder builder,
            in string queueNamespace
            ) where T : unmanaged
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void Push(in {typeof(T).Name} item)
        {{
            if(Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            if(Size == 0)
            {{
                *(_start + _tail) = item;
            }}
            else
            {{
                if (++_tail == Capacity)
                {{
                    _tail = 0;
                    *(_start) = item;
                }}
                else
                {{
                    *(_start + _tail) = item;
                }}
            }}

            Size += 1;{incrementVersion}
        }}
");
        }

        private void QueuePrimitivePushFuture(
            in StringBuilder builder,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void PushFuture()
        {{
            if(Size == Capacity)
            {{
                throw new Exception(""Not enough memory to allocate queue element"");
            }}

            if(Size != 0)
            {{
                if (++_tail == Capacity)
                {{
                    _tail = 0;
                }}
            }}

            Size += 1;{incrementVersion}
        }}
");
        }

        private void QueuePrimitivePushInPtr<T>(
            in StringBuilder builder,
            in string queueNamespace
            ) where T : unmanaged
        {
            var incrementVersion = queueNamespace == "Class" ?
    @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void Push(in {typeof(T).Name}* ptr)
        {{
            if(Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            if(Size == 0)
            {{
                *(_start + _tail) = *ptr;
            }}
            else
            {{
                if (++_tail == Capacity)
                {{
                    _tail = 0;
                    *(_start) = *ptr;
                }}
                else
                {{
                    *(_start + _tail) = *ptr;
                }}
            }}

            Size += 1;{incrementVersion}
        }}
");
        }

        private void QueuePrimitiveTryPushIn<T>(
            in StringBuilder builder,
            in string queueNamespace
            ) where T : unmanaged
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public bool TryPush(in {typeof(T).Name} item)
        {{
            if(Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            if(Size == 0)
            {{
                *(_start + _tail) = item;
            }}
            else
            {{
                if (_tail == Capacity - 1)
                {{
                    _tail = 0;
                    *(_start) = item;
                }}
                else
                {{
                    _tail += 1;
                    *(_start + _tail) = item;
                }}
            }}

            Size += 1;{incrementVersion}
            return true;
        }}
");
        }

        private void QueuePrimitiveTryPushInPtr<T>(
            in StringBuilder builder,
            in string queueNamespace
            ) where T : unmanaged
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public bool TryPush(in {typeof(T).Name}* ptr)
        {{
            if(Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            if(Size == 0)
            {{
                *(_start + _tail) = *ptr;
            }}
            else
            {{
                if (_tail == Capacity - 1)
                {{
                    _tail = 0;
                    *(_start) = *ptr;
                }}
                else
                {{
                    _tail += 1;
                    *(_start + _tail) = *ptr;
                }}
            }}

            Size += 1;{incrementVersion}
            return true;
        }}
");
        }

        private void QueuePrimitivePop(
            in StringBuilder builder,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void Pop()
        {{
            if (Size <= 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            Size -= 1;
            if(_head++ == _tail)
            {{
                _head = 0;
                _tail = 0;
            }}
            else if(_head == Capacity)
            {{
                _head = 0;
            }}{incrementVersion}
        }}
");
        }

        private void QueuePrimitiveTryPop(
            in StringBuilder builder,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public bool TryPop()
        {{
            if (Size <= 0)
            {{
                return false;
            }}

            Size -= 1;
            if(_head++ == _tail)
            {{
                _head = 0;
                _tail = 0;
            }}
            else if(_head == Capacity)
            {{
                _head = 0;
            }}{incrementVersion}

            return true;
        }}
");
        }

        private void QueuePrimitiveClear(
            in StringBuilder builder,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
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
                Size = 0;
                _tail = 0;
                _head = 0;{incrementVersion}
            }}
        }}
");
        }

        private void QueuePrimitiveFront<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name} Front()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            return *(_start + _head);
        }}
");
        }

        private void QueuePrimitiveBack<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name} Back()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            return *(_start + _tail);
        }}
");
        }

        private void QueuePrimitiveBackFuture<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name}* BackFuture()
        {{
            if (Capacity == 0 || Size == Capacity)
            {{
                throw new Exception(""Future element not available"");
            }}

            if(_tail > _head)
            {{
                var tempTail = _tail + 1;
                if(tempTail == Capacity)
                {{
                    return _start;
                }}
                else
                {{
                    return _start + tempTail;
                }}
            }}
            else
            {{
                if(Size == 0)
                {{
                    return _start + _tail;
                }}
                else
                {{
                    return _start + _tail + 1;
                }}
            }}
        }}
");
        }

        private void QueuePrimitiveFrontInPtr<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Front(in {typeof(T).Name}* ptr)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            *ptr = *(_start + _head);
        }}
");
        }

        private void QueuePrimitiveBackInPtr<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Back(in {typeof(T).Name}* ptr)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            *ptr = *(_start + _tail);
        }}
");
        }

        private void QueuePrimitiveFrontRefValue<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Front(ref {typeof(T).Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            item = *(_start + _head);
        }}
");
        }

        private void QueuePrimitiveBackRefValue<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Back(ref {typeof(T).Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            item = *(_start + _tail);
        }}
");
        }

        private void QueuePrimitiveFrontOutValue<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void FrontOut(out {typeof(T).Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            item = *(_start + _head);
        }}
");
        }

        private void QueuePrimitiveBackOutValue<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void BackOut(out {typeof(T).Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            item = *(_start + _tail);
        }}
");
        }

        private void QueuePrimitiveFrontPtr<T>(
            in StringBuilder builder
            ) where T :unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name}* FrontPtr()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            return _start + _head;
        }}
");
        }

        private void QueuePrimitiveBackPtr<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name}* BackPtr()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            return _start + _tail;
        }}
");
        }

        private void QueuePrimitiveDispose<T>(
            in StringBuilder builder,
            in string queueNamespace,
            in string sizeOf
            ) where T : unmanaged
        {
            if(queueNamespace == "Class")
            {
                builder.Append($@"
        #region IDisposable

        private bool _disposed;

        ~QueueOf{typeof(T).Name}() => Dispose(false);

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

        private void QueuePrimitiveIEnumerable<T>(
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
            private readonly Class.QueueOf{typeof(T).Name} _queue;
            private {typeof(T).Name}* _current;
            private int _currentIndex;
            private int _currentItem = 0;
            private int _version;

            internal Enumerator(Class.QueueOf{typeof(T).Name} queue)
            {{
                _queue = queue;
                _currentIndex = -1;
                _current = default;
                _version = _queue._version;
            }}

            public {typeof(T).Name} Current => *_current;

            object System.Collections.IEnumerator.Current => Current;

            public void Dispose()
            {{
                _currentIndex = -1;
            }}

            public bool MoveNext()
            {{
                if (_version != _queue._version)
                {{
                    throw new InvalidOperationException(""The stack was changed during the enumeration"");
                }}

                if (_queue.Size == 0)
                {{
                    return false;
                }}

                if (_currentIndex == -2)
                {{
                    _currentIndex = (int)_queue._head;
                    _current = _queue._start + _currentIndex;
                    _currentItem = 1;
                    return true;
                }}

                if (_currentIndex == -1)
                {{
                    return false;
                }}

                if ((nuint)(++_currentItem) == _queue.Size)
                {{
                    _current = default;
                    return false;
                }}

                if ((nuint)(_currentItem) == _queue.Capacity)
                {{
                    _currentIndex = 0;
                }}

                _current = _queue._start + _currentIndex;
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

        private void QueuePrimitiveIndexator<T>(
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
                    throw new Exception(""Element outside the queue"");
                }}
                
                if(_head > _tail)
                {{
                    return _start + (index - (Capacity - _head));
                }}
                else
                {{
                    return _start + _head + index;
                }}
            }}
        }}
");
        }

        private void QueuePrimitiveCopy<T>(
            in StringBuilder builder,
            in string sizeOf
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Copy(in void* ptrDest, in nuint count)
        {{
            if(Size < (nuint)count)
            {{
                throw new Exception(""The collection does not have that many elements"");
            }}
            
            if(_head > _tail)
            {{
                if(Capacity - _head <= count)
                {{
                    Buffer.MemoryCopy(
                        _start + _head,
                        ptrDest,
                        count * (nuint){sizeOf},
                        (Capacity - _head) * (nuint){sizeOf}
                        );
                }}
                else
                {{
                    var countToEnd = (Capacity - (nuint)_head);
                    Buffer.MemoryCopy(
                        _start + _head,
                        ptrDest,
                        count * (nuint){sizeOf},
                        countToEnd * (nuint){sizeOf}
                        );

                    Buffer.MemoryCopy(
                        _start,
                        (byte*)ptrDest + ((Capacity - _head) * (nuint){sizeOf}),
                        (Size - countToEnd) * (nuint){sizeOf},
                        (_tail + 1) * (nuint){sizeOf}
                        );
                }}
            }}
            else
            {{
                Buffer.MemoryCopy(
                    _start + _head,
                    ptrDest,
                    count * (nuint){sizeOf},
                    count * (nuint){sizeOf}
                    );
            }}
        }}
");
        }

        private void QueuePrimitiveCopyCount<T>(
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
            
            if(_head > _tail)
            {{
                var countToEnd = (Capacity - (nuint)_head);
                Buffer.MemoryCopy(
                    _start + _head,
                    ptrDest,
                    Size * (nuint){sizeOf},
                    countToEnd * (nuint){sizeOf}
                    );

                Buffer.MemoryCopy(
                    _start,
                    (byte*)ptrDest + ((Capacity - _head) * (nuint){sizeOf}),
                    (Size - countToEnd) * (nuint){sizeOf},
                    (_tail + 1) * (nuint){sizeOf}
                    );
            }}
            else
            {{
                Buffer.MemoryCopy(
                    _start + _head,
                    ptrDest,
                    Size * (nuint){sizeOf},
                    Size * (nuint){sizeOf}
                    );
            }}
        }}
");
        }

        private void QueuePrimitiveCopyInQueue<T>(
            in StringBuilder builder,
            in string sizeOf
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Copy(in Class.QueueOf{typeof(T).Name} destQueue)
        {{
            if(Size == 0)
            {{
                destQueue.SetPositions(0, 0, 0);
                return;
            }}

            if (destQueue.Capacity < Capacity)
            {{
                throw new ArgumentException(""Destination queue not enough capacity"");
            }}

            Buffer.MemoryCopy(
                _start,
                destQueue.Start,
                destQueue.Capacity * {sizeOf},
                Capacity * {sizeOf}
                );

            destQueue.SetPositions(in _head, in _tail, Size);
        }}
");
        }

        private void QueuePrimitiveSetPositions(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public void SetPositions(in nuint headIndex, in nuint tailIndex, in nuint size)
        {{
            _head = headIndex;
            _tail = tailIndex;
            Size = size;
        }}
");
        }

        private void QueuePrimitiveGetPositions(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public void GetPositions(out nuint headIndex, out nuint tailIndex, out nuint size)
        {{
            headIndex = _head;
            tailIndex = _tail;
            size = Size;
        }}
");
        }

        private void QueuePrimitiveEnd(
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