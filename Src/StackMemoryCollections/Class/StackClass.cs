﻿using System.Collections;

namespace StackMemoryCollections.Class
{
    public unsafe class Stack<T> : IDisposable, IEnumerable<T> where T : unmanaged
    {
        private readonly Struct.StackMemory* _stackMemoryS;
        private Class.StackMemory? _stackMemoryC = null;
        private T* _start;
        private readonly bool _memoryOwner = false;
        private int _version = 0;

        public Stack()
        {
            if (!TypeHelper.IsPrimitive<T>())
            {
                throw new ArgumentNullException("T must be primitive type");
            }

            _stackMemoryC = new Class.StackMemory((nuint)(sizeof(T) * 4));
            _start = (T*)_stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
        }

        public Stack(
            nuint count,
            Struct.StackMemory* stackMemory
            )
        {
            if (stackMemory == null)
            {
                throw new ArgumentNullException(nameof(stackMemory));
            }

            if(!TypeHelper.IsPrimitive<T>())
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

            if (Size > 0 && Size < Capacity - reducingCount)
            {
                throw new Exception("Can't reduce available memory, it's already in use");
            }

            if (_memoryOwner)
            {
                var newMemory = new Class.StackMemory((nuint)sizeof(T) * (Capacity - reducingCount ));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    (nuint)sizeof(T) * (Capacity - reducingCount)
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = (T*)_stackMemoryC.Start;
            }
            else
            {
                if (_stackMemoryS != null)
                {
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * (nuint)sizeof(T))))
                    {
                        throw new Exception("Failed to reduce available memory, stack moved further");
                    }

                    _stackMemoryS->FreeMemory(reducingCount * (nuint)sizeof(T));
                }
                else if (_stackMemoryC != null)
                {
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * (nuint)sizeof(T))))
                    {
                        throw new Exception("Failed to reduce available memory, stack moved further");
                    }

                    _stackMemoryC.FreeMemory(reducingCount * (nuint)sizeof(T));
                }
            }

            Capacity -= reducingCount;
        }

        public void ExpandCapacity(in nuint expandCount)
        {
            if (_memoryOwner)
            {
                var newMemory = new Class.StackMemory((nuint)sizeof(T) * (Capacity + expandCount));
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
                _start = (T*)_stackMemoryC.Start;
            }
            else
            {
                if (_stackMemoryS != null)
                {
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * (nuint)sizeof(T))))
                    {
                        throw new Exception("Failed to expand available memory, stack moved further");
                    }

                    _stackMemoryS->AllocateMemory(expandCount * (nuint)sizeof(T));
                }
                else if (_stackMemoryC != null)
                {
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * (nuint)sizeof(T))))
                    {
                        throw new Exception("Failed to expand available memory, stack moved further");
                    }

                    _stackMemoryC.AllocateMemory(expandCount * (nuint)sizeof(T));
                }
            }

            Capacity += expandCount;
        }

        public void TrimExcess()
        {
            if(_memoryOwner)
            {
                ReducingCapacity(
                    Size == 0 ?
                        Capacity > 4 ? (nuint)(-(4 - (long)Capacity))
                            : 0
                        : Capacity - Size
                        );
            }
            else
            {
                ReducingCapacity(Capacity - Size);
            }
        }

        public void Push(in T item)
        {
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {
                if(_memoryOwner)
                {
                    ExpandCapacity(Capacity);
                }
                else
                {
                    throw new Exception("Not enough memory to allocate stack element");
                }
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

        #region IDisposable

        private bool _disposed;

        ~Stack() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (!_memoryOwner)
                {
                    if (disposing)
                    {
                        if (_stackMemoryS != null)
                        {
                            _stackMemoryS->FreeMemory(Capacity * (nuint)sizeof(T));
                        }
                        else if (_stackMemoryC != null)
                        {
                            _stackMemoryC.FreeMemory(Capacity * (nuint)sizeof(T));
                        }
                    }
                }
                else
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    _stackMemoryC.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }

                _disposed = true;
            }
        }

        #endregion

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly Stack<T> _stack;
            private T* _current;
            private int _currentIndex;
            private int _version;

            internal Enumerator(Stack<T> stack)
            {
                _stack = stack;
                _currentIndex = -1;
                _current = default;
                _version = _stack._version;
            }

            public T Current => *_current;

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
                    _current = _stack._start + _currentIndex;
                    return true;
                }

                if(_currentIndex == -1)
                {
                    return false;
                }

                --_currentIndex;
                if(_currentIndex >= 0)
                {
                    _current = _stack._start + _currentIndex;
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