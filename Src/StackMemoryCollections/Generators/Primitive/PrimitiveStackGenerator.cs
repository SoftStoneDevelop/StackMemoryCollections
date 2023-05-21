using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace StackMemoryCollections.Generators.Primitive
{
    public partial class PrimitiveStackGenerator
    {
        private StringBuilder _builder = new StringBuilder();

        public void GeneratePrimitiveStack(
            in GeneratorExecutionContext context
            )
        {
            GenerateStack<IntPtr>(context, 4, true);

            GenerateStack<int>(context, 4, false);
            GenerateStack<uint>(context, 4, false);

            GenerateStack<long>(context, 8, false);
            GenerateStack<ulong>(context, 8, false);

            GenerateStack<sbyte>(context, 1, false);
            GenerateStack<byte>(context, 1, false);

            GenerateStack<short>(context, 2, false);
            GenerateStack<ushort>(context, 2, false);

            GenerateStack<char>(context, 2, false);
            GenerateStack<decimal>(context, 16, false);
            GenerateStack<double>(context, 8, false);
            GenerateStack<bool>(context, 1, false);//1 byte is not optimal
            GenerateStack<float>(context, 4, false);
        }

        private void GenerateStack<T>(
            GeneratorExecutionContext context,
            in int sizeOf,
            bool calculateSize
            ) where T : unmanaged
        {
            var sizeOfStr = calculateSize ? $"(nuint)sizeof({typeof(T).Name})" : sizeOf.ToString();
            GeneratePrimitiveStack<T>(in context, "Class", 4, sizeOfStr, false);
            GeneratePrimitiveStack<T>(in context, "Struct", 4, sizeOfStr, false);
        }

        private void GeneratePrimitiveStack<T>(
            in GeneratorExecutionContext context,
            in string stackNamespace,
            in int sizeOf,
            in string sizeOfStr,
            bool calculateSize
            ) where T : unmanaged
        {
            _builder.Clear();

            StackPrimitiveStart<T>(in stackNamespace);

            StackPrimitiveConstructor1<T>(in sizeOf, in sizeOfStr, calculateSize);
            StackPrimitiveConstructor2<T>(in sizeOfStr);
            StackPrimitiveConstructor3<T>(in sizeOfStr);
            StackPrimitiveConstructor4<T>();

            StackPrimitiveProperties<T>();

            StackPrimitiveReducingCapacity<T>(in sizeOfStr, in stackNamespace);
            StackPrimitiveExpandCapacity<T>(in sizeOfStr, in stackNamespace);
            StackPrimitiveTryExpandCapacity<T>(in sizeOfStr, in stackNamespace);
            StackPrimitiveTrimExcess();

            StackPrimitivePushIn<T>(in stackNamespace);
            StackPrimitivePushFuture(in stackNamespace);
            StackPrimitivePushInPtr<T>(in stackNamespace);
            StackPrimitiveTryPushIn<T>(in stackNamespace);
            StackPrimitiveTryPushInPtr<T>(in stackNamespace);

            StackPrimitivePop(in stackNamespace);
            StackPrimitiveTryPop(in stackNamespace);

            StackPrimitiveClear(in stackNamespace);

            StackPrimitiveTop<T>();
            StackPrimitiveTopInPtr<T>();
            StackPrimitiveTopRefValue<T>();
            StackPrimitiveTopPtr<T>();
            StackPrimitiveTopFuture<T>();
            StackPrimitiveTopOutValue<T>();

            StackPrimitiveDispose<T>(in stackNamespace, in sizeOfStr);
            StackPrimitiveIndexator<T>();
            StackPrimitiveCopyCount<T>(in sizeOfStr);
            StackPrimitiveCopy<T>(in sizeOfStr);
            StackPrimitiveCopyInStack<T>(in sizeOfStr);

            if (stackNamespace == "Class")
            {
                StackPrimitiveIEnumerable<T>();
            }

            StackPrimitiveEnd();

            context.AddSource($"StackOf{typeof(T).Name}{stackNamespace}.g.cs", _builder.ToString());
        }

        private void StackPrimitiveStart<T>(
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

            _builder.Append($@"
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
                _builder.Append($@"
        private int _version = 0;
");
            }
        }

        private void StackPrimitiveConstructor1<T>(
            in int sizeOf,
            in string sizeOfStr,
            bool calculateSize
            ) where T : unmanaged
        {
            _builder.Append($@"
        public StackOf{typeof(T).Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({(calculateSize ? $"{sizeOfStr} * 4" : (sizeOf * 4).ToString())});
            _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void StackPrimitiveConstructor2<T>(
            in string sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public StackOf{typeof(T).Name}(
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

        private void StackPrimitiveConstructor3<T>(
            in string sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public StackOf{typeof(T).Name}(
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

        private void StackPrimitiveConstructor4<T>(
            ) where T : unmanaged
        {
            _builder.Append($@"
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
            ) where T : unmanaged
        {
            _builder.Append($@"
        public nuint Capacity {{ get; private set; }}

        public nuint Size {{ get; set; }} = 0;

        public bool IsEmpty => Size == 0;

        public {typeof(T).Name}* Start => _start;
");
        }

        private void StackPrimitiveReducingCapacity<T>(
            in string sizeOf,
            in string stackNamespace
            ) where T : unmanaged
        {
            var incrementVersion = stackNamespace == "Class" ?
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

            if (Size > 0 && Size < Capacity - reducingCount)
            {{
                throw new Exception(""Can't reduce available memory, it's already in use"");
            }}

            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOf} * (Capacity - reducingCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    {sizeOf} * (Capacity - reducingCount)
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;{incrementVersion}
            }}
            else if(_stackMemoryS != null)
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

        private void StackPrimitiveExpandCapacity<T>(
            in string sizeOf,
            in string stackNamespace
            ) where T : unmanaged
        {
            var incrementVersion = stackNamespace == "Class" ?
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
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    _stackMemoryC.ByteCount
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;{incrementVersion}
            }}
            else if(_stackMemoryS != null)
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

        private void StackPrimitiveTryExpandCapacity<T>(
            in string sizeOf,
            in string stackNamespace
            ) where T : unmanaged
        {
            var incrementVersion = stackNamespace == "Class" ?
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
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    _stackMemoryC.ByteCount
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = ({typeof(T).Name}*)_stackMemoryC.Start;{incrementVersion}
            }}
            else if(_stackMemoryS != null)
            {{
                if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOf})))
                {{
                    return false;
                }}
        
                if(!_stackMemoryS->TryAllocateMemory(expandCount * {sizeOf}, out _))
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

                if(!_stackMemoryC.TryAllocateMemory(expandCount * {sizeOf}, out _))
                {{
                    return false;
                }}
            }}

            Capacity += expandCount;
            return true;
        }}
");
        }

        private void StackPrimitiveTrimExcess()
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

        private void StackPrimitivePushIn<T>(
            in string stackNamespace
            ) where T : unmanaged
        {
            _builder.Append($@"
        public void Push(in {typeof(T).Name} item)
        {{
            if (Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            *(_start + Size) = item;
            Size += 1;
");
            if (stackNamespace == "Class")
            {
                _builder.Append($@"
            _version += 1;
");
            }
            _builder.Append($@"
        }}
");
        }

        private void StackPrimitivePushFuture(
            in string stackNamespace
            )
        {
            _builder.Append($@"
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
                _builder.Append($@"
            _version += 1;
");
            }
            _builder.Append($@"
        }}
");
        }

        private void StackPrimitivePushInPtr<T>(
            in string stackNamespace
            ) where T : unmanaged
        {
            _builder.Append($@"
        public void Push(in {typeof(T).Name}* ptr)
        {{
            if (Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            *(_start + Size) = *ptr;
            Size += 1;
");
            if (stackNamespace == "Class")
            {
                _builder.Append($@"
            _version += 1;
");
            }
            _builder.Append($@"
        }}
");
        }

        private void StackPrimitiveTryPushIn<T>(
            in string stackNamespace
            ) where T : unmanaged
        {
            _builder.Append($@"
        public bool TryPush(in {typeof(T).Name} item)
        {{
            if (Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            *(_start + Size) = item;
            Size += 1;
");
            if (stackNamespace == "Class")
            {
                _builder.Append($@"
            _version += 1;
");
            }
            _builder.Append($@"
            return true;
        }}
");
        }

        private void StackPrimitiveTryPushInPtr<T>(
            in string stackNamespace
            ) where T : unmanaged
        {
            _builder.Append($@"
        public bool TryPush(in {typeof(T).Name}* ptr)
        {{
            if (Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            *(_start + Size) = *ptr;
            Size += 1;
");
            if (stackNamespace == "Class")
            {
                _builder.Append($@"
            _version += 1;
");
            }
            _builder.Append($@"
            return true;
        }}
");
        }

        private void StackPrimitivePop(
            in string stackNamespace
            )
        {
            _builder.Append($@"
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
                _builder.Append($@"
            _version += 1;
");
            }
            _builder.Append($@"
        }}
");
        }

        private void StackPrimitiveTryPop(
            in string stackNamespace
            )
        {
            _builder.Append($@"
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
                _builder.Append($@"
            _version += 1;
");
            }
            _builder.Append($@"

            return true;
        }}
");
        }

        private void StackPrimitiveClear(
            in string stackNamespace
            )
        {
            _builder.Append($@"
        public void Clear()
        {{
            if (Size != 0)
            {{
                Size = 0;
");
            if (stackNamespace == "Class")
            {
                _builder.Append($@"
            _version += 1;
");
            }
            _builder.Append($@"
            }}
        }}
");
        }

        private void StackPrimitiveTop<T>() where T : unmanaged
        {
            _builder.Append($@"
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

        private void StackPrimitiveTopFuture<T>() where T : unmanaged
        {
            _builder.Append($@"
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

        private void StackPrimitiveTopInPtr<T>() where T : unmanaged
        {
            _builder.Append($@"
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

        private void StackPrimitiveTopRefValue<T>() where T : unmanaged
        {
            _builder.Append($@"
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

        private void StackPrimitiveTopOutValue<T>() where T : unmanaged
        {
            _builder.Append($@"
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

        private void StackPrimitiveTopPtr<T>() where T : unmanaged
        {
            _builder.Append($@"
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
            in string stackNamespace,
            in string sizeOf
            ) where T : unmanaged
        {
            if (stackNamespace == "Class")
            {
                _builder.Append($@"
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

        private void StackPrimitiveIEnumerable<T>() where T : unmanaged
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

                if (_stack.Size == 0)
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

        private void StackPrimitiveIndexator<T>() where T : unmanaged
        {
            _builder.Append($@"
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
            in string sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public void Copy(in void* ptrDest, in nuint count)
        {{
            if(Size < (nuint)count)
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

        private void StackPrimitiveCopyCount<T>(
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
                Size * {sizeOf},
                Size * {sizeOf}
                );
        }}
");
        }

        private void StackPrimitiveCopyInStack<T>(
            in string sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public void Copy(in Class.StackOf{typeof(T).Name} destStack)
        {{
            if(Size == 0)
            {{
                destStack.Size = 0;
                return;
            }}

            if (destStack.Capacity < Size)
            {{
                throw new ArgumentException(""Destination stack not enough capacity"");
            }}

            Buffer.MemoryCopy(
                _start,
                destStack.Start,
                destStack.Capacity * {sizeOf},
                Size * {sizeOf}
                );

            destStack.Size = Size;
        }}
");
        }

        private void StackPrimitiveEnd()
        {
            _builder.Append($@"
    }}
}}
");
        }
    }
}