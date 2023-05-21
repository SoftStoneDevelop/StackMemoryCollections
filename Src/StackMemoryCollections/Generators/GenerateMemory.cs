using Microsoft.CodeAnalysis;
using System.Text;

namespace StackMemoryCollections
{
    internal class MemoryGenerator
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public void GenerateMemory(
            in GeneratorExecutionContext context
            )
        {
            GenerateMemory(in context, "Class");
            GenerateMemory(in context, "Struct");
        }

        private void GenerateMemory(
            in GeneratorExecutionContext context,
            in string memoryNamespace
            )
        {
            _builder.Clear();
            MemoryStart(in memoryNamespace);

            MemoryConstructor1(in memoryNamespace);
            MemoryConstructor2();
            MemoryProperties();
            MemoryAllocateMemory();
            MemoryTryAllocateMemory();
            MemoryShiftRight();
            MemoryShiftLeft();
            MemoryFreeMemory();
            MemoryTryFreeMemory();
            MemoryDispose(in memoryNamespace);

            MemoryEnd();
            context.AddSource($"StackMemory{memoryNamespace}.g.cs", _builder.ToString());
        }

        private void MemoryStart(
            in string memoryNamespace
            )
        {
            _builder.Append($@"
using System;
using System.Runtime.InteropServices;

namespace StackMemoryCollections.{memoryNamespace}
{{
    public unsafe {memoryNamespace.ToLowerInvariant()} StackMemory : IDisposable
    {{
");
        }

        private void MemoryConstructor1(
            in string memoryNamespace
            )
        {
            if(memoryNamespace == "Class")
            {
                return;
            }

            _builder.Append($@"
        public StackMemory()
        {{
            throw new Exception(""Constructor without parameters is not supported"");
        }}
");
        }

        private void MemoryConstructor2()
        {
            _builder.Append($@"
        public StackMemory(nuint byteCount)
        {{
            if(byteCount == 0)
            {{
                throw new ArgumentException(""Allocated memory size cannot be zero"");
            }}

            Start = NativeMemory.Alloc(byteCount);
            Current = Start;
            ByteCount = byteCount;
            FreeByteCount = byteCount;
            _disposed = false;
        }}
");
        }

        private void MemoryProperties()
        {
            _builder.Append($@"
        public void* Start {{ get; init; }}
        public void* Current {{ get; private set; }}
        public nuint ByteCount {{ get; init; }}
        public nuint FreeByteCount {{ get; private set; }}
");
        }

        private void MemoryAllocateMemory()
        {
            _builder.Append($@"
        public void* AllocateMemory(nuint allocateBytes)
        {{
            if (_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}

            if (FreeByteCount < allocateBytes)
            {{
                throw new ArgumentException(""Can't allocate memory"");
            }}

            FreeByteCount -= allocateBytes;
            var start = Current;
            Current = (byte*)start + allocateBytes;
            return start;
        }}
");
        }

        private void MemoryTryAllocateMemory()
        {
            _builder.Append($@"
        public bool TryAllocateMemory(nuint allocateBytes, out void* ptr)
        {{
            if (_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}

            if (FreeByteCount < allocateBytes)
            {{
                ptr = null;
                return false;
            }}

            FreeByteCount -= allocateBytes;
            var start = Current;
            Current = (byte*)start + allocateBytes;
            ptr = start;

            return true;
        }}
");
        }

        private void MemoryFreeMemory()
        {
            _builder.Append($@"
        public void FreeMemory(nuint reducingBytes)
        {{
            if(_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}

            if (ByteCount - FreeByteCount < reducingBytes)
            {{
                throw new Exception(""Unable to free memory, it is out of available memory"");
            }}

            FreeByteCount += reducingBytes;
            Current = (byte*)Current - reducingBytes;
        }}
");
        }

        private void MemoryTryFreeMemory()
        {
            _builder.Append($@"
        public bool TryFreeMemory(nuint reducingBytes)
        {{
            if (_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}

            if (ByteCount - FreeByteCount < reducingBytes)
            {{
                return false;
            }}

            FreeByteCount += reducingBytes;
            Current = (byte*)Current - reducingBytes;
            return true;
        }}
");
        }

        private void MemoryDispose(
            in string memoryNamespace
            )
        {
            if(memoryNamespace == "Class")
            {
                _builder.Append($@"
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
                NativeMemory.Free(Start);
                _disposed = true;
            }}
        }}

        #endregion
");
            }
            else
            {
                _builder.Append($@"
        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {{
            if(!_disposed)
            {{
                NativeMemory.Free(Start);
                _disposed = true;
            }}
        }}

        #endregion
");
            }
        }

        private void MemoryShiftRight()
        {
            _builder.Append($@"
        public void ShiftRight(
            in byte* start,
            in byte* end,
            long bytesShift
            )
        {{
            if (_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}
            
            var current = end - 1;
            while(current >= start)
            {{
                *(current + bytesShift) = *current;
                current -= 1;
            }}
        }}
");
        }

        private void MemoryShiftLeft()
        {
            _builder.Append($@"
        public void ShiftLeft(
            in byte* start,
            in byte* end,
            long bytesShift
            )
        {{
            if (_disposed)
            {{
                throw new ObjectDisposedException(nameof(StackMemory));
            }}

            var current = start - bytesShift;
            var newEnd = end - bytesShift;
            while(current != newEnd)
            {{               
                *current = *(current + bytesShift);
                current += 1;
            }}
        }}
");
        }

        private void MemoryEnd()
        {
            _builder.Append($@"
    }}
}}
");
        }
    }
}