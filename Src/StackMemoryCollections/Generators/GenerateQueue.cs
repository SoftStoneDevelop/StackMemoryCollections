using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackMemoryCollections
{
    internal class QueueGenerator
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public void GenerateQueue(
            in List<INamedTypeSymbol> typeQueue,
            in GeneratorExecutionContext context,
            in Dictionary<string, Model.TypeInfo> typeInfos
            )
        {
            for (int i = 0; i < typeQueue.Count; i++)
            {
                var currentType = typeQueue[i];
                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"{nameof(GenerateQueue)}: Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                GenerateQueue(in context, in currentType, in typeInfo, "Class");
                GenerateQueue(in context, in currentType, in typeInfo, "Struct");
            }
        }

        private void GenerateQueue(
            in GeneratorExecutionContext context,
            in INamedTypeSymbol currentType,
            in Model.TypeInfo typeInfo,
            in string queueNamespace
            )
        {
            _builder.Clear();

            var sizeOf = typeInfo.IsRuntimeCalculatedSize ? $"{currentType.Name}Helper.SizeOf" : $"{typeInfo.Size}";
            QueueStart(in currentType, in queueNamespace);

            QueueConstructor1(in currentType, in sizeOf);
            QueueConstructor2(in currentType, in sizeOf);
            QueueConstructor3(in currentType, in sizeOf);
            QueueConstructor4(in currentType);

            QueueProperties();

            QueueReducingCapacity(in sizeOf, in currentType, in queueNamespace);
            QueueExpandCapacity(in sizeOf, in queueNamespace);
            QueueTryExpandCapacity(in sizeOf, in queueNamespace);
            QueueTrimExcess();

            QueuePushIn(in currentType, in sizeOf, in queueNamespace);
            QueuePushFuture(in queueNamespace);
            QueuePushInPtr(in currentType, in sizeOf, in queueNamespace);
            QueueTryPushIn(in currentType, in sizeOf, in queueNamespace);
            QueueTryPushInPtr(in currentType, in sizeOf, in queueNamespace);

            QueuePop(in queueNamespace);
            QueueTryPop(in queueNamespace);

            QueueClear(in queueNamespace);

            QueueFront(in typeInfo, in sizeOf);
            QueueBack(in typeInfo, in sizeOf);

            QueueFrontInPtr(in currentType, in sizeOf);
            QueueBackInPtr(in currentType, in sizeOf);

            QueueFrontRefValue(in currentType, in sizeOf);
            QueueBackRefValue(in currentType, in sizeOf);

            QueueFrontPtr(in sizeOf);
            QueueBackPtr(in sizeOf);

            QueueBackFuture(in sizeOf);

            QueueFrontOutValue(in currentType, in sizeOf);
            QueueBackOutValue(in currentType, in sizeOf);

            QueueDispose(in currentType, in queueNamespace, in sizeOf);
            QueueIndexator(in sizeOf);
            QueueCopyCount(in sizeOf);
            QueueCopy(in sizeOf);
            QueueCopyInQueue(in currentType, in sizeOf);

            QueueSetPositions();
            QueueGetPositions();

            if (queueNamespace == "Class")
            {
                QueueIEnumerable(in currentType, in sizeOf);
            }

            QueueEnd();

            context.AddSource($"QueueOf{currentType.Name}{queueNamespace}.g.cs", _builder.ToString());
        }

        private void QueueStart(
            in INamedTypeSymbol currentType,
            in string queueNamespace
            )
        {
            string implements;
            if (queueNamespace == "Class")
            {
                implements = $"IDisposable, System.Collections.Generic.IEnumerable<{currentType.Name}>";
            }
            else
            {
                implements = $"IDisposable";
            }

            _builder.Append($@"
using System;
using System.Collections;
using {currentType.ContainingNamespace};
using System.Runtime.CompilerServices;

namespace {currentType.ContainingNamespace}.{queueNamespace}
{{
    public unsafe {queueNamespace.ToLowerInvariant()} QueueOf{currentType.Name} : {implements}
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private void* _start;
        private readonly bool _memoryOwner = false;
        private nuint _head = 0;
        private nuint _tail = 0;
");
            if (queueNamespace == "Class")
            {
                _builder.Append($@"
        private int _version = 0;
");
            }
        }

        private void QueueConstructor1(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public QueueOf{currentType.Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({sizeOf} * 4);
            _start = _stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void QueueConstructor2(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public QueueOf{currentType.Name}(
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

        private void QueueConstructor3(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public QueueOf{currentType.Name}(
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

        private void QueueConstructor4(
            in INamedTypeSymbol currentType
            )
        {
            _builder.Append($@"
        public QueueOf{currentType.Name}(
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

        private void QueueProperties()
        {
            _builder.Append($@"
        public nuint Capacity {{ get; private set; }}

        public nuint Size {{ get; private set; }} = 0;

        public bool IsEmpty => Size == 0;

        public void* Start => _start;
");
        }

        private void QueueReducingCapacity(
            in string sizeOf,
            in INamedTypeSymbol currentType,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
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
                _start = _stackMemoryC.Start;{incrementVersion}
            }}
            else if (_stackMemoryS != null)
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
                    _stackMemoryS->ShiftLeft(
                        (byte*)_start + (_head * {sizeOf}),
                        (byte*)_start + (Capacity * {sizeOf}),
                        (long)(reducingCount * {sizeOf})
                        );
                    _head -= reducingCount;{incrementVersion}
                }}
                else
                if (_head <= _tail)
                {{
                    if(Size == 1)
                    {{
                        {currentType.Name}Helper.Copy((byte*)_start + (_tail * {sizeOf}), (byte*)_start);
                        _tail = 0;
                        _head = 0;
                        }}
                    else
                    {{
                        var freeCountToEnd = Capacity - (_tail + 1);
                        if(freeCountToEnd == 0 || freeCountToEnd < reducingCount)
                        {{
                            _stackMemoryS->ShiftLeft(
                                (byte*)_start + (_head * {sizeOf}),
                                (byte*)_start + ((_tail + 1) * {sizeOf}),
                                (long)((Capacity - freeCountToEnd - ((_tail + 1) - _head)) * {sizeOf})
                                );
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
                    _stackMemoryC.ShiftLeft(
                        (byte*)_start + (_head * {sizeOf}),
                        (byte*)_start + (Capacity * {sizeOf}),
                        (long)(reducingCount * {sizeOf})
                        );
                    _head -= reducingCount;{incrementVersion}
                }}
                else
                if (_head <= _tail)
                {{
                    if(Size == 1)
                    {{
                        {currentType.Name}Helper.Copy((byte*)_start + (_tail * {sizeOf}), (byte*)_start);
                        _tail = 0;
                        _head = 0;
                    }}
                    else
                    {{
                        var freeCountToEnd = Capacity - (_tail + 1);
                        if(freeCountToEnd == 0 || freeCountToEnd < reducingCount)
                        {{
                            _stackMemoryC.ShiftLeft(
                                (byte*)_start + (_head * {sizeOf}),
                                (byte*)_start + ((_tail + 1) * {sizeOf}),
                                (long)((Capacity - freeCountToEnd - ((_tail + 1) - _head)) * {sizeOf})
                                );
                            _head = 0;
                            _tail = Size - 1;{incrementVersion}
                        }}
                    }}
                }}                    
                _stackMemoryC.FreeMemory(reducingCount * {sizeOf});
            }}

            Capacity -= reducingCount;
        }}
");
        }

        private void QueueExpandCapacity(
            in string sizeOf,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
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
                _start = _stackMemoryC.Start;{incrementVersion}
            }}
            else if (_stackMemoryS != null)
            {{
                if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                {{
                    throw new Exception(""Failed to expand available memory, stack moved further"");
                }}

                _stackMemoryS->AllocateMemory(expandCount * {sizeOf});
                if(Size != 0 && _head != 0 && _head > _tail)
                {{
                    _stackMemoryS->ShiftRight(
                        (byte*)_start + (_head * {sizeOf}),
                        (byte*)_start + ((_head + (Capacity - _head)) * {sizeOf}),
                        (long)(expandCount * {sizeOf})
                        );{incrementVersion}
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
                    _stackMemoryC.ShiftRight(
                        (byte*)_start + (_head * {sizeOf}),
                        (byte*)_start + ((_head + (Capacity - _head)) * {sizeOf}),
                        (long)(expandCount * {sizeOf})
                        );{incrementVersion}
                    _head += expandCount;
                }}
            }}

            Capacity += expandCount;
        }}
");
        }

        private void QueueTryExpandCapacity(
            in string sizeOf,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
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
                _start = _stackMemoryC.Start;{incrementVersion}
            }}
            else if (_stackMemoryS != null)
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
                    _stackMemoryS->ShiftRight(
                        (byte*)_start + (_head * {sizeOf}),
                        (byte*)_start + ((_head + (Capacity - _head)) * {sizeOf}),
                        (long)(expandCount * {sizeOf})
                        );{incrementVersion}
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
                    _stackMemoryC.ShiftRight(
                        (byte*)_start + (_head * {sizeOf}),
                        (byte*)_start + ((_head + (Capacity - _head)) * {sizeOf}),
                        (long)(expandCount * {sizeOf})
                        );{incrementVersion}
                    _head += expandCount;
                }}
            }}

            Capacity += expandCount;
            return true;
        }}
");
        }

        private void QueueTrimExcess()
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

        private void QueuePushIn(
            in INamedTypeSymbol currentType,
            in string sizeOf,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public void Push(in {currentType.Name} item)
        {{
            if(Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            if(Size == 0)
            {{
                {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (_tail * {sizeOf}));
            }}
            else
            {{
                if (++_tail == Capacity)
                {{
                    _tail = 0;
                    {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start);
                }}
                else
                {{
                    {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (_tail * {sizeOf}));
                }}
            }}

            Size += 1;{incrementVersion}
        }}
");
        }

        private void QueuePushFuture(
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
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

        private void QueuePushInPtr(
            in INamedTypeSymbol currentType,
            in string sizeOf,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
    @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public void Push(in void* ptr)
        {{
            if(Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            if(Size == 0)
            {{
                {currentType.Name}Helper.Copy(in ptr, (byte*)_start + (_tail * {sizeOf}));
            }}
            else
            {{
                if (++_tail == Capacity)
                {{
                    _tail = 0;
                    {currentType.Name}Helper.Copy(in ptr, (byte*)_start);
                }}
                else
                {{
                    {currentType.Name}Helper.Copy(in ptr, (byte*)_start + (_tail * {sizeOf}));
                }}
            }}

            Size += 1;{incrementVersion}
        }}
");
        }

        private void QueueTryPushIn(
            in INamedTypeSymbol currentType,
            in string sizeOf,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public bool TryPush(in {currentType.Name} item)
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
                {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (_tail * {sizeOf}));
            }}
            else
            {{
                if (_tail == Capacity - 1)
                {{
                    _tail = 0;
                    {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start);
                }}
                else
                {{
                    _tail += 1;
                    {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (_tail * {sizeOf}));
                }}
            }}

            Size += 1;{incrementVersion}
            return true;
        }}
");
        }

        private void QueueTryPushInPtr(
            in INamedTypeSymbol currentType,
            in string sizeOf,
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public bool TryPush(in void* ptr)
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
                {currentType.Name}Helper.Copy(in ptr, (byte*)_start + (_tail * {sizeOf}));
            }}
            else
            {{
                if (_tail == Capacity - 1)
                {{
                    _tail = 0;
                    {currentType.Name}Helper.Copy(in ptr, (byte*)_start);
                }}
                else
                {{
                    _tail += 1;
                    {currentType.Name}Helper.Copy(in ptr, (byte*)_start + (_tail * {sizeOf}));
                }}
            }}

            Size += 1;{incrementVersion}
            return true;
        }}
");
        }

        private void QueuePop(
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public void Pop()
        {{
            if (Size == 0)
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

        private void QueueTryPop(
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            _builder.Append($@"
        public bool TryPop()
        {{
            if (Size == 0)
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

        private void QueueClear(
            in string queueNamespace
            )
        {
            var incrementVersion = queueNamespace == "Class" ?
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
                Size = 0;
                _tail = 0;
                _head = 0;{incrementVersion}
            }}
        }}
");
        }

        private void QueueFront(
            in Model.TypeInfo typeInfo,
            in string sizeOf
            )
        {
            if(typeInfo.IsValueType)
            {
                _builder.Append($@"
        [SkipLocalsInit]
        public {typeInfo.TypeName} Front()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {typeInfo.TypeName} result;
            Unsafe.SkipInit(out result);
            {typeInfo.TypeName}Helper.CopyToValue((byte*)_start + (_head * {sizeOf}), ref result);

            return result;
        }}
");
            }
            else
            {
                _builder.Append($@"
        public {typeInfo.TypeName} Front()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {typeInfo.TypeName} result = new {typeInfo.TypeName}();
            {typeInfo.TypeName}Helper.CopyToValue((byte*)_start + (_head * {sizeOf}), ref result);
            return result;
        }}
");
            }
        }

        private void QueueBack(
            in Model.TypeInfo typeInfo,
            in string sizeOf
            )
        {
            if (typeInfo.IsValueType)
            {
                _builder.Append($@"
        [SkipLocalsInit]
        public {typeInfo.TypeName} Back()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {typeInfo.TypeName} result;
            Unsafe.SkipInit(out result);
            {typeInfo.TypeName}Helper.CopyToValue((byte*)_start + (_tail * {sizeOf}), ref result);

            return result;
        }}
");
            }
            else
            {
                _builder.Append($@"
        public {typeInfo.TypeName} Back()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {typeInfo.TypeName} result = new {typeInfo.TypeName}();
            {typeInfo.TypeName}Helper.CopyToValue((byte*)_start + (_tail * {sizeOf}), ref result);
            return result;
        }}
");
            }
        }

        private void QueueBackFuture(
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void* BackFuture()
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
                    return (byte*)_start + (tempTail * {sizeOf});
                }}
            }}
            else
            {{
                if(Size == 0)
                {{
                    return (byte*)_start + (_tail * {sizeOf});
                }}
                else
                {{
                    return (byte*)_start + ((_tail + 1) * {sizeOf});
                }}
            }}
        }}
");
        }

        private void QueueFrontInPtr(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void Front(in void* ptr)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {currentType.Name}Helper.Copy((byte*)_start + (_head * {sizeOf}), in ptr);
        }}
");
        }

        private void QueueBackInPtr(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void Back(in void* ptr)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {currentType.Name}Helper.Copy((byte*)_start + (_tail * {sizeOf}), in ptr);
        }}
");
        }

        private void QueueFrontRefValue(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void Front(ref {currentType.Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {currentType.Name}Helper.CopyToValue((byte*)_start + (_head * {sizeOf}), ref item);
        }}
");
        }

        private void QueueBackRefValue(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void Back(ref {currentType.Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {currentType.Name}Helper.CopyToValue((byte*)_start + (_tail * {sizeOf}), ref item);
        }}
");
        }

        private void QueueFrontOutValue(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void FrontOut(out {currentType.Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {currentType.Name}Helper.CopyToValueOut((byte*)_start + (_head * {sizeOf}), out item);
        }}
");
        }

        private void QueueBackOutValue(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void BackOut(out {currentType.Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            {currentType.Name}Helper.CopyToValueOut((byte*)_start + (_tail * {sizeOf}), out item);
        }}
");
        }

        private void QueueFrontPtr(
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void* FrontPtr()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            return (byte*)_start + (_head * {sizeOf});
        }}
");
        }

        private void QueueBackPtr(
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void* BackPtr()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the queue"");
            }}

            return (byte*)_start + (_tail * {sizeOf});
        }}
");
        }

        private void QueueDispose(
            in INamedTypeSymbol currentType,
            in string queueNamespace,
            in string sizeOf
            )
        {
            if(queueNamespace == "Class")
            {
                _builder.Append($@"
        #region IDisposable

        private bool _disposed;

        ~QueueOf{currentType.Name}() => Dispose(false);

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

        private void QueueIEnumerable(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
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
            private readonly Class.QueueOf{currentType.Name} _queue;
            private void* _current;
            private int _currentIndex;
            private int _currentItem = 0;
            private int _version;

            internal Enumerator(Class.QueueOf{currentType.Name} queue)
            {{
                _queue = queue;
                _currentIndex = -1;
                _current = default;
                _version = _queue._version;
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
                    _current = (byte*)_queue._start + (_currentIndex * (int){sizeOf});
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

                _current = (byte*)_queue._start + (_currentIndex * (int){sizeOf});
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

        private void QueueIndexator(
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void* this[nuint index]
        {{
            get
            {{
                if (Size == 0 || Size <= index)
                {{
                    throw new Exception(""Element outside the queue"");
                }}
                
                if(_head > _tail)
                {{
                    return (byte*)_start + ((index - (Capacity - _head)) * {sizeOf});
                }}
                else
                {{
                    return (byte*)_start + ((_head + index) * {sizeOf});
                }}
            }}
        }}
");
        }

        private void QueueCopy(
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void Copy(in void* ptrDest, in nuint count)
        {{
            if(Size < (nuint)count)
            {{
                throw new Exception(""The collection does not have that many elements"");
            }}
            
            if(_head > _tail)
            {{
                var countToEnd = (Capacity - (nuint)_head);
                if(countToEnd <= count)
                {{
                    Buffer.MemoryCopy(
                        (byte*)_start + (_head * {sizeOf}),
                        ptrDest,
                        count * (nuint){sizeOf},
                        (countToEnd) * (nuint){sizeOf}
                        );
                }}
                else
                {{
                    Buffer.MemoryCopy(
                        (byte*)_start + (_head * {sizeOf}),
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
                    (byte*)_start + (_head * {sizeOf}),
                    ptrDest,
                    count * (nuint){sizeOf},
                    count * (nuint){sizeOf}
                    );
            }}
        }}
");
        }

        private void QueueCopyCount(
            in string sizeOf
            )
        {
            _builder.Append($@"
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
                    (byte*)_start + (_head * {sizeOf}),
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
                    (byte*)_start + (_head * {sizeOf}),
                    ptrDest,
                    Size * (nuint){sizeOf},
                    Size * (nuint){sizeOf}
                    );
            }}
        }}
");
        }

        private void QueueCopyInQueue(
            in INamedTypeSymbol currentType,
            in string sizeOf
            )
        {
            _builder.Append($@"
        public void Copy(in Class.QueueOf{currentType.Name} destQueue)
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

        private void QueueSetPositions()
        {
            _builder.Append($@"
        public void SetPositions(in nuint headIndex, in nuint tailIndex, in nuint size)
        {{
            _head = headIndex;
            _tail = tailIndex;
            Size = size;
        }}
");
        }

        private void QueueGetPositions()
        {
            _builder.Append($@"
        public void GetPositions(out nuint headIndex, out nuint tailIndex, out nuint size)
        {{
            headIndex = _head;
            tailIndex = _tail;
            size = Size;
        }}
");
        }

        private void QueueEnd()
        {
            _builder.Append($@"
    }}
}}
");
        }
    }
}