namespace StackMemoryCollections
{
    public unsafe struct Stack<T> : IDisposable where T : unmanaged
    {
        private StackMemory* _stackMemory;
        private T* _start;

        public Stack()
        {
            throw new ArgumentException("Default constructor not supported");
        }

        public Stack(
            nuint count,
            StackMemory* stackMemory
            )
        {
            _start = (T*)(*stackMemory).AllocateMemory<T>(count);
            _stackMemory = stackMemory;
            Capacity = count;
        }

        public nuint Capacity { get; private set; }

        public nuint Size { get; private set; } = 0;

        public void* End => _start + Size;

        public bool IsEmpty => Size == 0;

        public void ReducingAvailableMemory(nuint reducingCount)
        {
            if (reducingCount <= 0)
            {
                return;
            }

            if (new IntPtr((*_stackMemory).Current) != new IntPtr(_start + Capacity))
            {
                throw new Exception("Failed to reduce available memory, stack moved further");
            }

            if (Size < Capacity - reducingCount)
            {
                throw new Exception("Can't reduce available memory, it's already in use");
            }

            Capacity -= reducingCount;
            (*_stackMemory).FreeMemory(reducingCount * (nuint)sizeof(T));
        }

        public void ExpandAvailableMemory(uint expandBytes)
        {
            Capacity += expandBytes;
        }

        public void SetAvailableMemoryCurrentUsed()
        {
            ReducingAvailableMemory(Capacity - Size);
        }

        public void Push(T item)
        {
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {
                throw new Exception("Not enough memory to allocate stack element");
            }

            *(_start + Size) = item;
            Size = tempSize;
        }

        public bool TryPush(T item)
        {
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {
                return false;
            }

            *(_start + Size) = item;
            Size = tempSize;

            return true;
        }

        public void Pop()
        {
            if (Size <= 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            Size--;
        }

        public void Clear()
        {
            Size = 0;
        }

        public T* Front()
        {
            if(Size == 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            return _start + Size - 1;
        }

        public void Dispose()
        {
            (*_stackMemory).FreeMemory(Capacity * (nuint)sizeof(T));
        }

        public T* this[nuint index]
        {
            get
            {
                if (Size < index)
                {
                    throw new Exception("Element outside the stack");
                }

                return _start - (Size + index);
            }
        }
    }
}