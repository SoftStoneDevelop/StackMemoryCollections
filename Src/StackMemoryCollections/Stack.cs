using System.Collections;
using System.Runtime.InteropServices;

namespace StackMemoryCollections
{
    public unsafe struct Stack<T> : IDisposable, IEnumerable<T> where T : struct
    {
        private StackMemory* _stackMemory;
        private void* _start;
        private int _version = 0;

        public Stack()
        {
            throw new ArgumentException("Default constructor not supported");
        }

        public Stack(
            nuint count,
            StackMemory* stackMemory
            )
        {
            _start = (*stackMemory).AllocateMemory<T>(count);
            _stackMemory = stackMemory;
            Capacity = count;

            Marshal.PtrToStructure<int>(new IntPtr(_start));
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
            
            if (new IntPtr((*_stackMemory).Current) != new IntPtr((byte*)_start + (Capacity * (nuint)Marshal.SizeOf<T>())))
            {
                throw new Exception("Failed to reduce available memory, stack moved further");
            }

            if (Size < Capacity - reducingCount)
            {
                throw new Exception("Can't reduce available memory, it's already in use");
            }

            Capacity -= reducingCount;
            (*_stackMemory).FreeMemory(reducingCount * (nuint)Marshal.SizeOf<T>());
        }

        public void ExpandAvailableMemory(in nuint expandBytes)
        {
            Capacity += expandBytes;
        }

        public void SetAvailableMemoryCurrentUsed()
        {
            ReducingAvailableMemory(Capacity - Size);
        }

        public void Push(in T item)
        {
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {
                throw new Exception("Not enough memory to allocate stack element");
            }

            Marshal.StructureToPtr(item, new IntPtr((byte*)_start + (Size * (nuint)Marshal.SizeOf<T>())), false);
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

            Marshal.StructureToPtr(item, new IntPtr((byte*)_start + (Size * (nuint)Marshal.SizeOf<T>())), false);
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

        public T Front()
        {
            if(Size == 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            return
                Marshal.PtrToStructure<T>(new IntPtr((byte*)_start + ((Size - 1) * (nuint)Marshal.SizeOf<T>())));
        }

        public void* FrontPtr()
        {
            if (Size == 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            return (byte*)_start + ((Size - 1) * (nuint)Marshal.SizeOf<T>());
        }

        public void Dispose()
        {
            (*_stackMemory).FreeMemory(Capacity * (nuint)Marshal.SizeOf<T>());
        }

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly Stack<T> _stack;
            private T _current;
            private int _currentIndex;
            private int _version;

            internal Enumerator(Stack<T> stack)
            {
                _stack = stack;
                _currentIndex = -1;
                _current = default;
                _version = _stack._version;
            }

            public T Current => _current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _currentIndex = -1;
            }

            public bool MoveNext()
            {
                if(_version != _stack._version)
                {
                    throw new InvalidOperationException("The stack was changed during the enumeration");
                }

                if(_stack.Size < 0)
                {
                    return false;
                }

                if(_currentIndex == -2)
                {
                    _currentIndex = (int)_stack.Size - 1;
                    _current =
                        Marshal.PtrToStructure<T>(new IntPtr((byte*)_stack._start + (_currentIndex * Marshal.SizeOf<T>())));
                    return true;
                }

                if(_currentIndex == -1)
                {
                    return false;
                }

                --_currentIndex;
                if(_currentIndex >= 0)
                {
                    _current =
                        Marshal.PtrToStructure<T>(new IntPtr((byte*)_stack._start + (_currentIndex * Marshal.SizeOf<T>())));
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

        public T this[nuint index]
        {
            get
            {
                if (Size < index)
                {
                    throw new Exception("Element outside the stack");
                }

                return
                    Marshal.PtrToStructure<T>(new IntPtr((byte*)_start - ((Size + index) * (nuint)Marshal.SizeOf<T>())));
            }
        }
    }
}