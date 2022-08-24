using System.Runtime.InteropServices;

namespace StackMemoryCollections.Struct
{
    public unsafe struct StackMemory : IDisposable
    {
        private nuint _offsetBytes;

        public StackMemory()
        {
            throw new ArgumentException("Default constructor not supported");
        }

        public StackMemory(nuint byteCount)
        {
            Start = NativeMemory.Alloc(byteCount);
            Current = Start;
            ByteCount = byteCount;
            _offsetBytes = 0;
        }

        public void* Start { get; init; }
        public void* Current { get; private set; }
        public nuint ByteCount { get; init; }

        public void* AllocateMemory(nuint allocateBytes)
        {
            if (ByteCount - _offsetBytes < allocateBytes)
            {
                throw new ArgumentException("Can't allocate memory");
            }

            _offsetBytes += allocateBytes;
            var start = Current;
            Current = (byte*)start + allocateBytes;
            return start;
        }

        public void FreeMemory(nuint reducingBytes)
        {
            var start = new IntPtr(Start);
            var newCurrent = new IntPtr((byte*)Current - reducingBytes);

            if (newCurrent.CompareTo(start) < 0)
            {
                throw new Exception("Unable to free memory, it is out of available memory");
            }

            _offsetBytes -= reducingBytes;
            Current = newCurrent.ToPointer();
        }

        #region IDisposable

        public void Dispose()
        {
            NativeMemory.Free(Start);
        }

        #endregion
    }
}
