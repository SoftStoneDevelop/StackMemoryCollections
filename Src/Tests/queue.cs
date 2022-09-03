
/*
MIT License

Copyright (c) 2022 Brevnov Vyacheslav Sergeevich

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using Tests;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Tests.Class
{
    public unsafe class QueueOfTestStruct : IDisposable
    {
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private void* _start;
        private readonly bool _memoryOwner = false;

        private nuint _head = 0;
        private nuint _tail = 0;
        private int _version = 0;

        public QueueOfTestStruct()
        {
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory(TestStructHelper.SizeOf * 4);
            _start = _stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }

        public QueueOfTestStruct(
            nuint count,
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {
            if (stackMemory == null)
            {
                throw new ArgumentNullException(nameof(stackMemory));
            }

            _start = stackMemory->AllocateMemory(TestStructHelper.SizeOf * count);
            _stackMemoryS = stackMemory;
            Capacity = count;
        }

        public QueueOfTestStruct(
            nuint count,
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {
            if (stackMemory == null)
            {
                throw new ArgumentNullException(nameof(stackMemory));
            }

            _start = stackMemory.AllocateMemory(TestStructHelper.SizeOf * count);
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
            Capacity = count;
        }

        public QueueOfTestStruct(
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

        public nuint Size { get; set; } = 0;

        public bool IsEmpty => Size == 0;

        public void* Start => _start;

        public void ReducingCapacity(in nuint reducingCount)
        {
            if (reducingCount <= 0)
            {
                return;
            }

            if (Size == Capacity || Capacity - Size < reducingCount)
            {
                throw new Exception("Can't reduce available memory, it's already in use");
            }

            if (_memoryOwner)
            {
                var newMemory = new StackMemoryCollections.Class.StackMemory(TestStructHelper.SizeOf * (Capacity - reducingCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (Size == 0)
                {
                    _tail = 0;
                    _head = 0;
                }
                else if (_tail > _head)
                {
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * TestStructHelper.SizeOf),
                        newMemory.Start,
                        newMemory.ByteCount,
                        Size * TestStructHelper.SizeOf
                        );
                    _tail = Size - 1;
                    _head = 0;
                }
                else
                {
                    var headToEndByteCount = (Capacity - _head) * TestStructHelper.SizeOf;
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * TestStructHelper.SizeOf),
                        newMemory.Start,
                        newMemory.ByteCount,
                        headToEndByteCount
                        );
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start,
                        (byte*)newMemory.Start + headToEndByteCount,
                        newMemory.ByteCount - headToEndByteCount,
                        (_tail + 1) * TestStructHelper.SizeOf
                        );
                    _tail = Size - 1;
                    _head = 0;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;
                _version += 1;
            }
            else
            {
                if (_stackMemoryS != null)
                {
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * TestStructHelper.SizeOf)))
                    {
                        throw new Exception("Failed to reduce available memory, stack moved further");
                    }

                    if(_head > _tail && Capacity - _head + 1 < reducingCount)
                    {
                        _stackMemoryS->ShiftLeft((byte*)_start + _head, (long)(reducingCount * TestStructHelper.SizeOf));
                        _head -= reducingCount;
                        _version += 1;
                    }
                    else
                    if ((_head < _tail && Capacity - _tail + 1 < reducingCount) ||
                       (_head == _tail && Capacity - _tail + 1 < reducingCount)
                       )
                    {
                        _stackMemoryS->ShiftLeft((byte*)_start + _tail, (long)(reducingCount * TestStructHelper.SizeOf));
                        _tail -= reducingCount;
                        _version += 1;
                    }
                    else
                    {
                        _stackMemoryS->FreeMemory(reducingCount * TestStructHelper.SizeOf);
                    }
                }
                else if (_stackMemoryC != null)
                {
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * TestStructHelper.SizeOf)))
                    {
                        throw new Exception("Failed to reduce available memory, stack moved further");
                    }

                    if (_head > _tail && Capacity - _head + 1 < reducingCount)
                    {
                        _stackMemoryC.ShiftLeft((byte*)_start + _head, (long)(reducingCount * TestStructHelper.SizeOf));
                        _head -= reducingCount;
                        _version += 1;
                    }
                    else
                    if ((_head < _tail && Capacity - _tail + 1 < reducingCount) ||
                       (_head == _tail && Capacity - _tail + 1 < reducingCount)
                       )
                    {
                        _stackMemoryC.ShiftLeft((byte*)_start + _tail, (long)(reducingCount * TestStructHelper.SizeOf));
                        _tail -= reducingCount;
                        _version += 1;
                    }
                    else
                    {
                        _stackMemoryC.FreeMemory(reducingCount * TestStructHelper.SizeOf);
                    }
                }
            }

            Capacity -= reducingCount;
        }

        public void ExpandCapacity(in nuint expandCount)
        {
            if (_memoryOwner)
            {
                var newMemory = new StackMemoryCollections.Class.StackMemory(TestStructHelper.SizeOf * (Capacity + expandCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if(Size == 0)
                {
                    _tail = 0;
                    _head = 0;
                }
                else if(_tail > _head)
                {
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * TestStructHelper.SizeOf),
                        newMemory.Start,
                        newMemory.ByteCount,
                        Size * TestStructHelper.SizeOf
                        );
                    _tail = Size - 1;
                    _head = 0;
                }
                else
                {
                    var headToEndByteCount = (Capacity - _head) * TestStructHelper.SizeOf;
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start + (_head * TestStructHelper.SizeOf),
                        newMemory.Start,
                        newMemory.ByteCount,
                        headToEndByteCount
                        );
                    Buffer.MemoryCopy(
                        (byte*)_stackMemoryC.Start,
                        (byte*)newMemory.Start + headToEndByteCount,
                        newMemory.ByteCount - headToEndByteCount,
                        (_tail + 1) * TestStructHelper.SizeOf
                        );
                    _tail = Size - 1;
                    _head = 0;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;
                _version += 1;
            }
            else
            {
                if (_stackMemoryS != null)
                {
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * TestStructHelper.SizeOf)))
                    {
                        throw new Exception("Failed to expand available memory, stack moved further");
                    }

                    _stackMemoryS->AllocateMemory(expandCount * TestStructHelper.SizeOf);
                    if(Size != 0 && _head != 0 && _head > _tail)
                    {
                        _stackMemoryS->ShiftRight((byte*)_start + _head + 1 + (Capacity * TestStructHelper.SizeOf), (long)(Capacity * TestStructHelper.SizeOf));
                        _version += 1;
                    }
                }
                else if (_stackMemoryC != null)
                {
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * TestStructHelper.SizeOf)))
                    {
                        throw new Exception("Failed to expand available memory, stack moved further");
                    }

                    _stackMemoryC.AllocateMemory(expandCount * TestStructHelper.SizeOf);
                    if (Size != 0 && _head != 0 && _head > _tail)
                    {
                        _stackMemoryS->ShiftRight((byte*)_start + _head + 1 + (Capacity * TestStructHelper.SizeOf), (long)(Capacity * TestStructHelper.SizeOf));
                        _version += 1;
                    }
                }
            }

            Capacity += expandCount;
        }

        public void TrimExcess()
        {
            if (_memoryOwner)
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

        public void Push(in TestStruct item)
        {
            if(Size == Capacity)
            {
                ExpandCapacity(Capacity);
            }

            if(Size == 0)
            {
                TestStructHelper.CopyToPtr(in item, (byte*)_start + (_tail * TestStructHelper.SizeOf));
            }
            else
            {
                if (_tail == Capacity)
                {
                    _tail = 0;
                    TestStructHelper.CopyToPtr(in item, (byte*)_start + (_tail * TestStructHelper.SizeOf));
                }
                else
                {
                    _tail += 1;
                    TestStructHelper.CopyToPtr(in item, (byte*)_start + (_tail * TestStructHelper.SizeOf));
                }
            }

            Size += 1;
            _version += 1;
        }

        public void Pop()
        {
            if (Size <= 0)
            {
                throw new Exception("There are no elements on the queue");
            }

            Size -= 1;
            if(_head++ == _tail)
            {
                _head = 0;
                _tail = 0;
            }
            _version += 1;

        }

        public void Clear()
        {
            if (Size != 0)
            {
                Size = 0;
                _tail = 0;
                _head = 0;
                _version += 1;
            }
        }

        public TestStruct Front()
        {
            if (Size == 0)
            {
                throw new Exception("There are no elements on the queue");
            }

            TestStruct result = new TestStruct();
            TestStructHelper.CopyToValue((byte*)_start + (_head * TestStructHelper.SizeOf), ref result);
            return
                result;
        }

        public TestStruct Back()
        {
            if (Size == 0)
            {
                throw new Exception("There are no elements on the queue");
            }

            TestStruct result = new TestStruct();
            TestStructHelper.CopyToValue((byte*)_start + (_tail * TestStructHelper.SizeOf), ref result);
            return
                result;
        }

        #region IDisposable

        private bool _disposed;

        ~QueueOfTestStruct() => Dispose(false);

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
                            _stackMemoryS->FreeMemory(Capacity * TestStructHelper.SizeOf);
                        }
                        else if (_stackMemoryC != null)
                        {
                            _stackMemoryC.FreeMemory(Capacity * TestStructHelper.SizeOf);
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

    }
}
