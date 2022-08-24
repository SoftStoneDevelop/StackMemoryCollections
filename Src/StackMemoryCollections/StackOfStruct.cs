using GenerateAttibutes;
using System.Collections;

namespace StackMemoryCollections
{
    [GenerateStack]
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

    public unsafe static class StructHelper
    {
        public static nuint GetSize()
        {
            return 12;
        }

        public static void* GetInt32Ptr(in void* ptr)
        {
            return ptr;
        }

        public static int GetInt32Value(in void* ptr)
        {
            return *(int*)ptr;
        }

        public static void SetInt32Value(in void* ptr, in int value)
        {
            *(int*)ptr = value;
        }

        public static void SetInt32Value(in void* ptr, in SimpleStruct item)
        {
            *(int*)ptr = item.Int32;
        }

        public static void* GetInt64Ptr(in void* ptr)
        {
            return (byte*)ptr + 4;
        }

        public static long GetInt64Value(in void* ptr)
        {
            return *(long*)((byte*)ptr + 4);
        }

        public static void SetInt64Value(in void* ptr, in long value)
        {
            *(long*)((byte*)ptr + 4) = value;
        }

        public static void SetInt64Value(in void* ptr, in SimpleStruct item)
        {
            *(long*)((byte*)ptr + 4)= item.Int64;
        }

        public static void CopyToPrt(in SimpleStruct item, in void* ptr)
        {
            SetInt32Value(in ptr, in item);
            SetInt64Value(in ptr, in item);
        }

        public static void CopyToStruct(in void* ptr, ref SimpleStruct item)
        {
            item.Int32 = GetInt32Value(in ptr);
            item.Int64 = GetInt64Value(in ptr);
        }
    }

    public unsafe struct StackOfSimpleStruct : IDisposable, IEnumerable<SimpleStruct>
    {
        private StackMemory* _stackMemory;
        private void* _start;
        private int _version = 0;
        private nuint _sizeOf = StructHelper.GetSize();

        public StackOfSimpleStruct()
        {
            throw new ArgumentException("Default constructor not supported");
        }

        public StackOfSimpleStruct(
            nuint count,
            StackMemory* stackMemory
            )
        {
            _start = (*stackMemory).AllocateMemory(_sizeOf * count);
            _stackMemory = stackMemory;
            Capacity = count;
        }

        public StackOfSimpleStruct(
            nuint count,
            void* memoryStart
            )
        {
            _start = memoryStart;
            _stackMemory = null;
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

            if(_stackMemory != null)
            {
                if (new IntPtr((*_stackMemory).Current) != new IntPtr((byte*)_start + (Capacity * _sizeOf)))
                {
                    throw new Exception("Failed to reduce available memory, stack moved further");
                }

                (*_stackMemory).FreeMemory(reducingCount * _sizeOf);
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

            StructHelper.CopyToPrt(in item, (byte*)_start + (Size * _sizeOf));
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

            StructHelper.CopyToPrt(in item, (byte*)_start + (Size * _sizeOf));
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
            StructHelper.CopyToStruct((byte*)_start + ((Size - 1) * _sizeOf), ref result);
            return
                result;
        }

        public void* FrontPtr()
        {
            if (Size == 0)
            {
                throw new Exception("There are no elements on the stack");
            }

            return (byte*)_start + ((Size - 1) * _sizeOf);
        }

        public void Dispose()
        {
            if(_stackMemory != null)
            {
                (*_stackMemory).FreeMemory(Capacity * _sizeOf);
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

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public struct Enumerator : IEnumerator<SimpleStruct>, IEnumerator
        {
            private readonly StackOfSimpleStruct _stack;
            private SimpleStruct _current;
            private int _currentIndex;
            private int _version;

            internal Enumerator(StackOfSimpleStruct stack)
            {
                _stack = stack;
                _currentIndex = -1;
                _current = default;
                _version = _stack._version;
            }

            public SimpleStruct Current => _current;

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
                    SimpleStruct result = default;
                    StructHelper.CopyToStruct((byte*)_stack._start + (_currentIndex * (int)_stack._sizeOf), ref result);
                    _current = result;
                    return true;
                }

                if (_currentIndex == -1)
                {
                    return false;
                }

                --_currentIndex;
                if (_currentIndex >= 0)
                {
                    SimpleStruct result = default;
                    StructHelper.CopyToStruct((byte*)_stack._start + (_currentIndex * (int)_stack._sizeOf), ref result);
                    _current = result;
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
                StructHelper.CopyToStruct((byte*)_start + ((Size - 1 - index) * _sizeOf), ref result);
                return
                    result;
            }
        }
    }
}
