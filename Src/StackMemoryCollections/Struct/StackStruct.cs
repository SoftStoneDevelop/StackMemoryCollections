namespace StackMemoryCollections.Struct
{
    public unsafe struct Stack<T> : IDisposable where T : unmanaged
    {
        private readonly Struct.StackMemory* _stackMemoryS;
        private readonly Class.StackMemory? _stackMemoryC = null;
        private readonly T* _start;
        private int _version = 0;

        public Stack()
        {
            throw new ArgumentException("Default constructor not supported");
        }

        public Stack(
            nuint count,
            Struct.StackMemory* stackMemory
            )
        {
            if(stackMemory == null)
            {
                throw new ArgumentNullException(nameof(stackMemory));
            }

            if (!TypeHelper.IsPrimitive<T>())
            {
                throw new ArgumentNullException("T must be primitive type");
            }

            _start = (T*)(*stackMemory).AllocateMemory((nuint)sizeof(T) * count);
            _stackMemoryS = stackMemory;
            Capacity = count;
        }

        public Stack(
            nuint count,
            Class.StackMemory stackMemory
            )
        {
            if (stackMemory == null)
            {
                throw new ArgumentNullException(nameof(stackMemory));
            }

            if (!TypeHelper.IsPrimitive<T>())
            {
                throw new ArgumentNullException("T must be primitive type");
            }

            _start = (T*)stackMemory.AllocateMemory((nuint)sizeof(T) * count);
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
            Capacity = count;
        }

        public Stack(
            nuint count,
            void* memoryStart
            )
        {
            if (memoryStart == null)
            {
                throw new ArgumentNullException(nameof(memoryStart));
            }

            if (!TypeHelper.IsPrimitive<T>())
            {
                throw new ArgumentNullException("T must be primitive type");
            }

            _start = (T*)memoryStart;
            _stackMemoryC = null;
            _stackMemoryS = null;
            Capacity = count;
        }

        public nuint Capacity { get; private set; }

        public nuint Size { get; private set; } = 0;

        public bool IsEmpty => Size == 0;

        public void ReducingCapacity(in nuint reducingCount)
        {
            if (reducingCount <= 0)
            {
                return;
            }

            if (Size < Capacity - reducingCount)
            {
                throw new Exception("Can't reduce available memory, it's already in use");
            }

            if (_stackMemoryS != null)
            {
                if (new IntPtr((*_stackMemoryS).Current) != new IntPtr((byte*)_start + (Capacity * (nuint)sizeof(T))))
                {
                    throw new Exception("Failed to reduce available memory, stack moved further");
                }

                (*_stackMemoryS).FreeMemory(reducingCount * (nuint)sizeof(T));
            }
            else if(_stackMemoryC != null)
            {
                if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * (nuint)sizeof(T))))
                {
                    throw new Exception("Failed to reduce available memory, stack moved further");
                }

                _stackMemoryC.FreeMemory(reducingCount * (nuint)sizeof(T));
            }

            Capacity -= reducingCount;
        }

        public void ExpandCapacity(in nuint expandBytes)
        {
            Capacity += expandBytes;
        }

        public void TrimExcess()
        {
            ReducingCapacity(Capacity - Size);
        }

        public void Push(in T item)
        {
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {
                throw new Exception("Not enough memory to allocate stack element");
            }

            *(_start + Size) = item;
            Size = tempSize;
            _version++;
        }

        public bool TryPush(in T item)
        {
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {
                return false;
            }

            *(_start + Size) = item;
            Size = tempSize;
            _version++;

            return true;
        }

        public void Pop()
        {
            if (Size <= 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            Size--;
            _version++;
        }

        public void Clear()
        {
            if(Size != 0)
            {
                Size = 0;
                _version++;
            }
        }

        public T Top()
        {
            if(Size == 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            return
                *(_start + (Size - 1));
        }

        public T* TopPtr()
        {
            if (Size == 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            return _start + (Size - 1);
        }

        public void Dispose()
        {
            if (_stackMemoryS != null)
            {
                (*_stackMemoryS).FreeMemory(Capacity * (nuint)sizeof(T));
            }
            else if (_stackMemoryC != null)
            {
                _stackMemoryC.FreeMemory(Capacity * (nuint)sizeof(T));
            }
        }

        public T* this[nuint index]
        {
            get
            {
                if (Size <= 0 || Size <= index)
                {
                    throw new Exception("Element outside the stack");
                }

                return
                    _start + (Size - 1 - index);
            }
        }

        public void Copy(in void* ptrDest)
        {
            Buffer.MemoryCopy(
                _start,
                ptrDest,
                Capacity * (nuint)sizeof(T),
                Capacity * (nuint)sizeof(T)
                );
        }
    }
}