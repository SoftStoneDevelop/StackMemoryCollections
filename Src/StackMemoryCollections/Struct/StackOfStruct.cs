using GenerateAttibutes;
using System.Collections;

namespace StackMemoryCollections
{
    [GenerateHelper]
    [GenerateStack]
    [GenerateWrapper]
    public struct SimpleStruct
    {
        public SimpleStruct(
            int int32,
            long int64
            )
        {
            Int32 = int32;
            Int64 = int64;
        }

        public long Int64;
        public int Int32;
    }

    public unsafe struct StackOfSimpleStruct : IDisposable, System.Collections.Generic.IEnumerable<SimpleStruct>
    {
        private readonly Struct.StackMemory* _stackMemoryS;
        private readonly Class.StackMemory? _stackMemoryC = null;
        private readonly void* _start;
        private int _version = 0;

        public StackOfSimpleStruct()
        {
            throw new Exception("Default constructor not supported");
        }

        public StackOfSimpleStruct(
            nuint count,
            Struct.StackMemory* stackMemory
            )
        {
            if (stackMemory == null)
            {
                throw new ArgumentNullException(nameof(stackMemory));
            }

            _start = (*stackMemory).AllocateMemory(SimpleStructHelper.GetSize() * count);
            _stackMemoryS = stackMemory;
            Capacity = count;
        }

        public StackOfSimpleStruct(
            nuint count,
            Class.StackMemory stackMemory
            )
        {
            if (stackMemory == null)
            {
                throw new ArgumentNullException(nameof(stackMemory));
            }

            _start = stackMemory.AllocateMemory(SimpleStructHelper.GetSize() * count);
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
            Capacity = count;
        }

        public StackOfSimpleStruct(
            nuint count,
            void* memoryStart
            )
        {
            if (memoryStart == null)
            {
                throw new ArgumentNullException(nameof(memoryStart));
            }

            _start = memoryStart;
            _stackMemoryS = null;
            Capacity = count;
        }

        public nuint Capacity { get; private set; }

        public nuint Size { get; private set; } = 0;

        public bool IsEmpty => Size == 0;

        public void ReducingAvailableMemory(in nuint reducingCount)
        {
            if (reducingCount <= 0)
            {
                return;
            }

            if (Size < Capacity - reducingCount)
            {
                throw new Exception("Can't reduce available memory, it's already in use");
            }

            if(_stackMemoryS != null)
            {
                if (new IntPtr((*_stackMemoryS).Current) != new IntPtr((byte*)_start + (Capacity * SimpleStructHelper.GetSize())))
                {
                    throw new Exception("Failed to reduce available memory, stack moved further");
                }

                (*_stackMemoryS).FreeMemory(reducingCount * SimpleStructHelper.GetSize());
            }
            else if (_stackMemoryC != null)
            {
                if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * SimpleStructHelper.GetSize())))
                {
                    throw new Exception("Failed to reduce available memory, stack moved further");
                }

                _stackMemoryC.FreeMemory(reducingCount * SimpleStructHelper.GetSize());
            }

            Capacity -= reducingCount;
        }

        public void ExpandAvailableMemory(in nuint expandBytes)
        {
            Capacity += expandBytes;
        }

        public void SetAvailableMemoryCurrentUsed()
        {
            ReducingAvailableMemory(Capacity - Size);
        }


        public void Push(in SimpleStruct item)
        {
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {
                throw new Exception("Not enough memory to allocate stack element");
            }

            SimpleStructHelper.CopyToPtr(in item, (byte*)_start + (Size * SimpleStructHelper.GetSize()));
            Size = tempSize;
            _version++;
        }

        public bool TryPush(in SimpleStruct item)
        {
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {
                return false;
            }

            SimpleStructHelper.CopyToPtr(in item, (byte*)_start + (Size * SimpleStructHelper.GetSize()));
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
            if (Size != 0)
            {
                Size = 0;
                _version++;
            }
        }

        public SimpleStruct Front()
        {
            if (Size == 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            SimpleStruct result = default;
            SimpleStructHelper.CopyToValue((byte*)_start + ((Size - 1) * SimpleStructHelper.GetSize()), ref result);
            return
                result;
        }

        public void* FrontPtr()
        {
            if (Size == 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            return (byte*)_start + ((Size - 1) * SimpleStructHelper.GetSize());
        }

        public void Dispose()
        {
            if(_stackMemoryS != null)
            {
                (*_stackMemoryS).FreeMemory(Capacity * SimpleStructHelper.GetSize());
            }
            else if (_stackMemoryC != null)
            {
                _stackMemoryC.FreeMemory(Capacity * SimpleStructHelper.GetSize());
            }
        }

        #region IEnumerable<T>

        public IEnumerator<SimpleStruct> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<SimpleStruct>, IEnumerator
        {
            private readonly StackOfSimpleStruct _stack;
            private void* _current;
            private int _currentIndex;
            private int _version;

            internal Enumerator(StackOfSimpleStruct stack)
            {
                _stack = stack;
                _currentIndex = -1;
                _current = default;
                _version = _stack._version;
            }

            public SimpleStruct Current 
            {
                get
                {
                    SimpleStruct result = new SimpleStruct();
                    SimpleStructHelper.CopyToValue(_current, ref result);
                    return result;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _currentIndex = -1;
            }

            public bool MoveNext()
            {
                if (_version != _stack._version)
                {
                    throw new InvalidOperationException("The stack was changed during the enumeration");
                }

                if (_stack.Size < 0)
                {
                    return false;
                }

                if (_currentIndex == -2)
                {
                    _currentIndex = (int)_stack.Size - 1;
                    _current = (byte*)_stack._start + (_currentIndex * (int)SimpleStructHelper.GetSize());
                    return true;
                }

                if (_currentIndex == -1)
                {
                    return false;
                }

                --_currentIndex;
                if (_currentIndex >= 0)
                {
                    _current = (byte*)_stack._start + (_currentIndex * (int)SimpleStructHelper.GetSize());
                    return true;
                }
                else
                {
                    _current = default;
                    return false;
                }
            }

            public void Reset()
            {
                _currentIndex = -2;
            }
        }

        #endregion

        public SimpleStruct this[nuint index]
        {
            get
            {
                if (Size <= 0 || Size <= index)
                {
                    throw new Exception("Element outside the stack");
                }
                SimpleStruct result = default;
                SimpleStructHelper.CopyToValue((byte*)_start + ((Size - 1 - index) * SimpleStructHelper.GetSize()), ref result);
                return
                    result;
            }
        }

        public void* GetByIndex(nuint index)
        {
            if (Size <= 0 || Size <= index)
            {
                throw new Exception("Element outside the stack");
            }
            
            return
                (byte*)_start + ((Size - 1 - index) * SimpleStructHelper.GetSize());
        }
    }
}