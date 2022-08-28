using Microsoft.CodeAnalysis;
using System.Text;

namespace StackMemoryCollections
{
    public partial class Generator
    {
        private void GenerateMemory(
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            builder.Clear();

            builder.Append($@"
using System;
using System.Runtime.InteropServices;

namespace StackMemoryCollections.Class
{{
    public unsafe class StackMemory : IDisposable
    {{
        private nuint _offsetBytes;

        public StackMemory(nuint byteCount)
        {{
            Start = NativeMemory.Alloc(byteCount);
            Current = Start;
            ByteCount = byteCount;
            _offsetBytes = 0;
        }}

        public void* Start {{ get; init; }}
        public void* Current {{ get; private set; }}
        public nuint ByteCount {{ get; init; }}
        public nuint FreeByteCount => ByteCount - _offsetBytes;

        public void* AllocateMemory(nuint allocateBytes)
        {{
            if (_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}

            if (ByteCount - _offsetBytes < allocateBytes)
            {{
                throw new ArgumentException(""Can't allocate memory"");
            }}

            _offsetBytes += allocateBytes;
            var start = Current;
            Current = (byte*)start + allocateBytes;
            return start;
        }}

        public bool TryAllocateMemory(nuint allocateBytes, out void* ptr)
        {{
            if (_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}

            if (ByteCount - _offsetBytes < allocateBytes)
            {{
                ptr = null;
                return false;
            }}

            _offsetBytes += allocateBytes;
            var start = Current;
            Current = (byte*)start + allocateBytes;
            ptr = start;

            return true;
        }}

        public void FreeMemory(nuint reducingBytes)
        {{
            if(_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}

            var start = new IntPtr(Start);
            var newCurrent = new IntPtr((byte*)Current - reducingBytes);

            if (newCurrent.CompareTo(start) < 0)
            {{
                throw new Exception(""Unable to free memory, it is out of available memory"");
            }}

            _offsetBytes -= reducingBytes;
            Current = newCurrent.ToPointer();
        }}

        public bool TryFreeMemory(nuint reducingBytes)
        {{
            if (_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}

            var start = new IntPtr(Start);
            var newCurrent = new IntPtr((byte*)Current - reducingBytes);

            if (newCurrent.CompareTo(start) < 0)
            {{
                return false;
            }}

            _offsetBytes -= reducingBytes;
            Current = newCurrent.ToPointer();
            return true;
        }}

        #region IDisposable

        private bool _disposed;

        ~StackMemory() => Dispose(false);

        public void Dispose()
        {{
            Dispose(true);
            GC.SuppressFinalize(this);
        }}

        protected virtual void Dispose(bool disposing)
        {{
            if (!_disposed)
            {{
                if (disposing)
                {{
                    
                }}

                NativeMemory.Free(Start);
                _disposed = true;
            }}
        }}

        #endregion
    }}
}}

");
            context.AddSource($"StackMemoryClass.g.cs", builder.ToString());

            builder.Clear();
            builder.Append($@"
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;

namespace StackMemoryCollections.Struct
{{
    public unsafe struct StackMemory : IDisposable
    {{
        private nuint _offsetBytes;

        public StackMemory()
        {{
            throw new ArgumentException(""Default constructor not supported"");
        }}

        public StackMemory(nuint byteCount)
        {{
            Start = NativeMemory.Alloc(byteCount);
            Current = Start;
            ByteCount = byteCount;
            _offsetBytes = 0;
        }}

        public void* Start {{ get; init; }}
        public void* Current {{ get; private set; }}
        public nuint ByteCount {{ get; init; }}
        public nuint FreeByteCount => ByteCount - _offsetBytes;

        public void* AllocateMemory(nuint allocateBytes)
        {{
            if (ByteCount - _offsetBytes < allocateBytes)
            {{
                throw new ArgumentException(""Can't allocate memory"");
            }}

            _offsetBytes += allocateBytes;
            var start = Current;
            Current = (byte*)start + allocateBytes;
            return start;
        }}

        public bool TryAllocateMemory(nuint allocateBytes, out void* ptr)
        {{
            if (ByteCount - _offsetBytes < allocateBytes)
            {{
                ptr = null;
                return false;
            }}

            _offsetBytes += allocateBytes;
            var start = Current;
            Current = (byte*)start + allocateBytes;
            ptr = start;

            return true;
        }}

        public void FreeMemory(nuint reducingBytes)
        {{
            var start = new IntPtr(Start);
            var newCurrent = new IntPtr((byte*)Current - reducingBytes);

            if (newCurrent.CompareTo(start) < 0)
            {{
                throw new Exception(""Unable to free memory, it is out of available memory"");
            }}

            _offsetBytes -= reducingBytes;
            Current = newCurrent.ToPointer();
        }}

        #region IDisposable

        public void Dispose()
        {{
            NativeMemory.Free(Start);
        }}

        #endregion
    }}
}}

");
            context.AddSource($"StackMemoryStruct.g.cs", builder.ToString());
        }
    }
}