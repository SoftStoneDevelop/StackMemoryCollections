using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace StackMemoryCollections
{
    public partial class Generator
    {
        private void GeneratePrimitiveStack(
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            GeneratePrimitiveStack<IntPtr>(in context, in builder, "Class", 4, true);
            GeneratePrimitiveStack<IntPtr>(in context, in builder, "Struct", 4, true);

            GeneratePrimitiveStack<Int32>(in context, in builder, "Class", 4, false);
            GeneratePrimitiveStack<Int32>(in context, in builder, "Struct", 4, false);

            GeneratePrimitiveStack<UInt32>(in context, in builder, "Class", 4, false);
            GeneratePrimitiveStack<UInt32>(in context, in builder, "Struct", 4, false);

            GeneratePrimitiveStack<Int64>(in context, in builder, "Class", 8, false);
            GeneratePrimitiveStack<Int64>(in context, in builder, "Struct", 8, false);

            GeneratePrimitiveStack<UInt64>(in context, in builder, "Class", 8, false);
            GeneratePrimitiveStack<UInt64>(in context, in builder, "Struct", 8, false);

            GeneratePrimitiveStack<SByte>(in context, in builder, "Class", 1, false);
            GeneratePrimitiveStack<SByte>(in context, in builder, "Struct", 1, false);

            GeneratePrimitiveStack<Byte>(in context, in builder, "Class", 1, false);
            GeneratePrimitiveStack<Byte>(in context, in builder, "Struct", 1, false);

            GeneratePrimitiveStack<Int16>(in context, in builder, "Class", 2, false);
            GeneratePrimitiveStack<Int16>(in context, in builder, "Struct", 2, false);

            GeneratePrimitiveStack<UInt16>(in context, in builder, "Class", 2, false);
            GeneratePrimitiveStack<UInt16>(in context, in builder, "Struct", 2, false);

            GeneratePrimitiveStack<Char>(in context, in builder, "Class", 2, false);
            GeneratePrimitiveStack<Char>(in context, in builder, "Struct", 2, false);

            GeneratePrimitiveStack<Decimal>(in context, in builder, "Class", 16, false);
            GeneratePrimitiveStack<Decimal>(in context, in builder, "Struct", 16, false);

            GeneratePrimitiveStack<Double>(in context, in builder, "Class", 8, false);
            GeneratePrimitiveStack<Double>(in context, in builder, "Struct", 8, false);

            GeneratePrimitiveStack<Boolean>(in context, in builder, "Class", 1, false);//1 byte is not optimal
            GeneratePrimitiveStack<Boolean>(in context, in builder, "Struct", 1, false);//1 byte is not optimal

            GeneratePrimitiveStack<Single>(in context, in builder, "Class", 4, false);
            GeneratePrimitiveStack<Single>(in context, in builder, "Struct", 4, false);
        }

        private void GeneratePrimitiveStack<T>(
            in GeneratorExecutionContext context,
            in StringBuilder builder,
            in string stackNamespace,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Clear();
            StackPrimitiveStart<T>(in builder, in stackNamespace);

            StackPrimitiveConstructor1<T>(in builder, in sizeOf, calculateSize);
            StackPrimitiveConstructor2<T>(in builder, in sizeOf, calculateSize);
            StackPrimitiveConstructor3<T>(in builder, in sizeOf, calculateSize);
            StackPrimitiveConstructor4<T>(in builder);

            StackPrimitiveProperties<T>(in builder);

            StackPrimitiveReducingCapacity<T>(in builder, in sizeOf, calculateSize);
            StackPrimitiveExpandCapacity<T>(in builder, in sizeOf, calculateSize);
            StackPrimitiveTrimExcess(in builder);
            StackPrimitivePushIn<T>(in builder, in stackNamespace, in sizeOf, calculateSize);
            StackPrimitivePushFuture(in builder, in stackNamespace);
            StackPrimitivePushInPtr<T>(in builder, in stackNamespace, in sizeOf, calculateSize);
            StackPrimitiveTryPushIn<T>(in builder, in stackNamespace, in sizeOf, calculateSize);
            StackPrimitiveTryPushInPtr<T>(in builder, in stackNamespace, in sizeOf, calculateSize);
            StackPrimitivePop(in builder, in stackNamespace);
            StackPrimitiveTryPop(in builder, in stackNamespace);
            StackPrimitiveClear(in builder, in stackNamespace);
            StackPrimitiveTop<T>(in builder);
            StackPrimitiveTopInPtr<T>(in builder);
            StackPrimitiveTopRefValue<T>(in builder);
            StackPrimitiveTopPtr<T>(in builder);
            StackPrimitiveTopFuture<T>(in builder);
            StackPrimitiveDispose<T>(in builder, in stackNamespace, in sizeOf, calculateSize);
            StackPrimitiveIndexator<T>(in builder);
            StackPrimitiveCopyCount<T>(in builder, in sizeOf, calculateSize);
            StackPrimitiveCopy<T>(in builder, in sizeOf, calculateSize);
            StackPrimitiveTopOutValue<T>(in builder);
            StackPrimitiveCopyInStack<T>(in builder, in sizeOf, calculateSize);

            if (stackNamespace == "Class")
            {
                StackPrimitiveIEnumerable<T>(in builder);
            }

            StackPrimitiveEnd(in builder);

            context.AddSource($"StackOf{typeof(T).Name}{stackNamespace}.g.cs", builder.ToString());
        }

        private void StackPrimitiveStart<T>(
            in StringBuilder builder,
            in string stackNamespace
            ) where T : unmanaged
        {
            string implements;
            if (stackNamespace == "Class")
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

namespace StackMemoryCollections.{stackNamespace}
{{
    public unsafe {stackNamespace.ToLowerInvariant()} StackOf{typeof(T).Name} : {implements}
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private {typeof(T).Name}* _start;
        private readonly bool _memoryOwner = false;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
        private int _version = 0;
");
            }
        }

        private void StackPrimitiveConstructor1<T>(
            in StringBuilder builder,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public StackOf{typeof(T).Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf * 4).ToString())});
            _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void StackPrimitiveConstructor2<T>(
            in StringBuilder builder,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public StackOf{typeof(T).Name}(
            nuint count,
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = ({typeof(T).Name}*)stackMemory->AllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : sizeOf.ToString())} * count);
            _stackMemoryS = stackMemory;
            Capacity = count;
        }}
");
        }

        private void StackPrimitiveConstructor3<T>(
            in StringBuilder builder,
            in int sizeOf,
            bool calculateSize
            ) where T: unmanaged
        {
            builder.Append($@"
        public StackOf{typeof(T).Name}(
            nuint count,
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = ({typeof(T).Name}*)stackMemory.AllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())} * count);
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
            Capacity = count;
        }}
");
        }

        private void StackPrimitiveConstructor4<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public StackOf{typeof(T).Name}(
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

        private void StackPrimitiveProperties<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public nuint Capacity {{ get; private set; }}

        public nuint Size {{ get; set; }} = 0;

        public bool IsEmpty => Size == 0;

        public {typeof(T).Name}* Start => _start;
");
        }

        private void StackPrimitiveReducingCapacity<T>(
            in StringBuilder builder,
            in int sizeOf,
            bool calculateSize
            ) where T :unmanaged
        {
            builder.Append($@"
        public void ReducingCapacity(in nuint reducingCount)
        {{
            if (reducingCount <= 0)
            {{
                return;
            }}

            if (Size > 0 && Size < Capacity - reducingCount)
            {{
                throw new Exception(""Can't reduce available memory, it's already in use"");
            }}

            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())} * (Capacity - reducingCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())} * (Capacity - reducingCount)
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            }}
            else
            {{
                if(_stackMemoryS != null)
                {{
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                    {{
                        throw new Exception(""Failed to reduce available memory, stack moved further"");
                    }}

                    _stackMemoryS->FreeMemory(reducingCount * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                }}
                else if (_stackMemoryC != null)
                {{
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                    {{
                        throw new Exception(""Failed to reduce available memory, stack moved further"");
                    }}

                    _stackMemoryC.FreeMemory(reducingCount * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                }}
            }}

            Capacity -= reducingCount;
        }}
");
        }

        private void StackPrimitiveExpandCapacity<T>(
            in StringBuilder builder,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public void ExpandCapacity(in nuint expandCount)
        {{

            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())} * (Capacity + expandCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    _stackMemoryC.ByteCount
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            }}
            else
            {{
                if(_stackMemoryS != null)
                {{
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                    {{
                        throw new Exception(""Failed to expand available memory, stack moved further"");
                    }}

                    _stackMemoryS->AllocateMemory(expandCount * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                }}
                else if (_stackMemoryC != null)
                {{
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                    {{
                        throw new Exception(""Failed to expand available memory, stack moved further"");
                    }}

                    _stackMemoryC.AllocateMemory(expandCount * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                }}
            }}

            Capacity += expandCount;
        }}
");
        }

        private void StackPrimitiveTrimExcess(
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

        private void StackPrimitivePushIn<T>(
            in StringBuilder builder,
            in string stackNamespace,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Push(in {typeof(T).Name} item)
        {{
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {{
                if (_memoryOwner)
                {{
                    ExpandCapacity(Capacity);
                }}
                else
                {{
                    if(_stackMemoryS != null)
                    {{
                        if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                        {{
                            throw new Exception(""Failed to expand available memory, stack moved further"");
                        }}

                        _stackMemoryS->AllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                    }}
                    else if (_stackMemoryC != null)
                    {{
                        if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                        {{
                            throw new Exception(""Failed to expand available memory, stack moved further"");
                        }}

                        _stackMemoryC.AllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                    }}
                    else
                    {{
                        throw new Exception(""Not enough memory to allocate stack element"");
                    }}
                    
                    Capacity += 1;
                }}
            }}

            *(_start + Size) = item;
            Size = tempSize;
");
            if(stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
        }}
");
        }

        private void StackPrimitivePushFuture(
            in StringBuilder builder,
            in string stackNamespace
            )
        {
            builder.Append($@"
        public void PushFuture()
        {{
            if (Size == Capacity)
            {{
                throw new Exception(""Not enough memory to allocate stack element"");
            }}

            Size += 1;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
        }}
");
        }

        private void StackPrimitivePushInPtr<T>(
            in StringBuilder builder,
            in string stackNamespace,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Push(in {typeof(T).Name}* ptr)
        {{
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {{
                if (_memoryOwner)
                {{
                    ExpandCapacity(Capacity);
                }}
                else
                {{
                    if(_stackMemoryS != null)
                    {{
                        if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                        {{
                            throw new Exception(""Failed to expand available memory, stack moved further"");
                        }}

                        _stackMemoryS->AllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                    }}
                    else if (_stackMemoryC != null)
                    {{
                        if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                        {{
                            throw new Exception(""Failed to expand available memory, stack moved further"");
                        }}

                        _stackMemoryC.AllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                    }}
                    else
                    {{
                        throw new Exception(""Not enough memory to allocate stack element"");
                    }}
                    
                    Capacity += 1;
                }}
            }}

            *(_start + Size) = *ptr;
            Size = tempSize;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
        }}
");
        }

        private void StackPrimitiveTryPushIn<T>(
            in StringBuilder builder,
            in string stackNamespace,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public bool TryPush(in {typeof(T).Name} item)
        {{
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {{
                if (_memoryOwner)
                {{
                    ExpandCapacity(Capacity);
                }}
                else
                {{
                    if(_stackMemoryS != null)
                    {{
                        if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                        {{
                            return false;
                        }}
                        
                        if(!_stackMemoryS->TryAllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())}, out _))
                        {{
                            return false;
                        }}
                    }}
                    else if (_stackMemoryC != null)
                    {{
                        if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                        {{
                            return false;
                        }}

                        if(!_stackMemoryC.TryAllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())}, out _))
                        {{
                            return false;
                        }}
                    }}
                    else
                    {{
                        return false;
                    }}
                    
                    Capacity += 1;
                }}
            }}

            *(_start + Size) = item;
            Size = tempSize;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
            return true;
        }}
");
        }

        private void StackPrimitiveTryPushInPtr<T>(
            in StringBuilder builder,
            in string stackNamespace,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public bool TryPush(in {typeof(T).Name}* ptr)
        {{
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {{
                if (_memoryOwner)
                {{
                    ExpandCapacity(Capacity);
                }}
                else
                {{
                    if(_stackMemoryS != null)
                    {{
                        if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                        {{
                            return false;
                        }}
                        
                        if(!_stackMemoryS->TryAllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())}, out _))
                        {{
                            return false;
                        }}
                    }}
                    else if (_stackMemoryC != null)
                    {{
                        if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())})))
                        {{
                            return false;
                        }}

                        if(!_stackMemoryC.TryAllocateMemory({(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())}, out _))
                        {{
                            return false;
                        }}
                    }}
                    else
                    {{
                        return false;
                    }}
                    
                    Capacity += 1;
                }}
            }}

            *(_start + Size) = *ptr;
            Size = tempSize;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
            return true;
        }}
");
        }

        private void StackPrimitivePop(
            in StringBuilder builder,
            in string stackNamespace
            )
        {
            builder.Append($@"
        public void Pop()
        {{
            if (Size <= 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            Size -= 1;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
        }}
");
        }

        private void StackPrimitiveTryPop(
            in StringBuilder builder,
            in string stackNamespace
            )
        {
            builder.Append($@"
        public bool TryPop()
        {{
            if (Size <= 0)
            {{
                return false;
            }}

            Size -= 1;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"

            return true;
        }}
");
        }

        private void StackPrimitiveClear(
            in StringBuilder builder,
            in string stackNamespace
            )
        {
            builder.Append($@"
        public void Clear()
        {{
            if (Size != 0)
            {{
                Size = 0;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
            }}
        }}
");
        }

        private void StackPrimitiveTop<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name} Top()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            return
                *(_start + (Size - 1));
        }}
");
        }

        private void StackPrimitiveTopFuture<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name}* TopFuture()
        {{
            if (Capacity == 0 || Size == Capacity)
            {{
                throw new Exception(""Future element not available"");
            }}

            return
                _start + Size;
        }}
");
        }

        private void StackPrimitiveTopInPtr<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Top(in {typeof(T).Name}* ptr)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            *ptr = *(_start + (Size - 1));
        }}
");
        }

        private void StackPrimitiveTopRefValue<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Top(ref {typeof(T).Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            item = *(_start + (Size - 1));
        }}
");
        }

        private void StackPrimitiveTopOutValue<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void TopOut(out {typeof(T).Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            item = *(_start + (Size - 1));
        }}
");
        }

        private void StackPrimitiveTopPtr<T>(
            in StringBuilder builder
            ) where T :unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name}* TopPtr()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            return _start + (Size - 1);
        }}
");
        }

        private void StackPrimitiveDispose<T>(
            in StringBuilder builder,
            in string stackNamespace,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            if(stackNamespace == "Class")
            {
                builder.Append($@"
        #region IDisposable

        private bool _disposed;

        ~StackOf{typeof(T).Name}() => Dispose(false);

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
                            _stackMemoryS->FreeMemory(Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                        }}
                        else if (_stackMemoryC != null)
                        {{
                            _stackMemoryC.FreeMemory(Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
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
                    _stackMemoryS->FreeMemory(Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
                }}
                else if (_stackMemoryC != null)
                {{
                    _stackMemoryC.FreeMemory(Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())});
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

        private void StackPrimitiveIEnumerable<T>(
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
            private readonly Class.StackOf{typeof(T).Name} _stack;
            private {typeof(T).Name}* _current;
            private int _currentIndex;
            private int _version;

            internal Enumerator(Class.StackOf{typeof(T).Name} stack)
            {{
                _stack = stack;
                _currentIndex = -1;
                _current = default;
                _version = _stack._version;
            }}

            public {typeof(T).Name} Current => *_current;

            object System.Collections.IEnumerator.Current => Current;

            public void Dispose()
            {{
                _currentIndex = -1;
            }}

            public bool MoveNext()
            {{
                if (_version != _stack._version)
                {{
                    throw new InvalidOperationException(""The stack was changed during the enumeration"");
                }}

                if (_stack.Size < 0)
                {{
                    return false;
                }}

                if (_currentIndex == -2)
                {{
                    _currentIndex = (int)_stack.Size - 1;
                    _current = _stack._start + _currentIndex;
                    return true;
                }}

                if (_currentIndex == -1)
                {{
                    return false;
                }}

                if (--_currentIndex >= 0)
                {{
                    _current = _stack._start + _currentIndex;
                    return true;
                }}
                else
                {{
                    _current = default;
                    return false;
                }}
            }}

            public void Reset()
            {{
                _currentIndex = -2;
            }}
        }}

        #endregion
");
        }

        private void StackPrimitiveIndexator<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name}* this[nuint index]
        {{
            get
            {{
                if (Size <= 0 || Size <= index)
                {{
                    throw new Exception(""Element outside the stack"");
                }}

                return
                    _start + (Size - 1 - index);
            }}
        }}
");
        }

        private void StackPrimitiveCopy<T>(
            in StringBuilder builder,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Copy(in void* ptrDest, in nuint count)
        {{
            if(Size < (nuint)count)
            {{
                throw new Exception(""The collection does not have that many elements"");
            }}

            Buffer.MemoryCopy(
                _start,
                ptrDest,
                count * (nuint){(calculateSize ? $"sizeof({typeof(T).Name})" : (sizeOf).ToString())},
                count * (nuint){(calculateSize ? $"sizeof({typeof(T).Name})" : (sizeOf).ToString())}
                );
        }}
");
        }

        private void StackPrimitiveCopyCount<T>(
            in StringBuilder builder,
            in int sizeOf,
            bool calculateSize
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
                Size * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())},
                Size * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())}
                );
        }}
");
        }

        private void StackPrimitiveCopyInStack<T>(
            in StringBuilder builder,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Copy(in Class.StackOf{typeof(T).Name} destStack)
        {{
            if(Size == 0)
            {{
                return;
            }}

            if (destStack.Capacity < Size)
            {{
                throw new ArgumentException(""Destination stack not enough capacity"");
            }}

            Buffer.MemoryCopy(
                _start,
                destStack.Start,
                destStack.Capacity * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())},
                Size * {(calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : (sizeOf).ToString())}
                );

            destStack.Size = Size;
        }}
");
        }

        private void StackPrimitiveEnd(
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