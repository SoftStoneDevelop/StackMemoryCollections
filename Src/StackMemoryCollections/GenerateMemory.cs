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
            GenerateMemory(in context, in builder, "Class");
            GenerateMemory(in context, in builder, "Struct");
        }

        private void GenerateMemory(
            in GeneratorExecutionContext context,
            in StringBuilder builder,
            in string memoryNamespace
            )
        {
            builder.Clear();
            MemoryStart(in builder, in memoryNamespace);

            MemoryConstructor1(in builder, in memoryNamespace);
            MemoryConstructor2(in builder);
            MemoryProperties(in builder);
            MemoryAllocateMemory(in builder);
            MemoryTryAllocateMemory(in builder);
            MemoryFreeMemory(in builder);
            MemoryTryFreeMemory(in builder);
            MemoryDispose(in builder, in memoryNamespace);

            MemoryEnd(in builder);
            context.AddSource($"StackMemory{memoryNamespace}.g.cs", builder.ToString());
        }

        private void MemoryStart(
            in StringBuilder builder,
            in string memoryNamespace
            )
        {
            builder.Append($@"
using System;
using System.Runtime.InteropServices;

namespace StackMemoryCollections.{memoryNamespace}
{{
    public unsafe {memoryNamespace.ToLowerInvariant()} StackMemory : IDisposable
    {{
");
        }

        private void MemoryConstructor1(
            in StringBuilder builder,
            in string memoryNamespace
            )
        {
            if(memoryNamespace == "Class")
            {
                return;
            }

            builder.Append($@"
        public StackMemory()
        {{
            throw new Exception(""Constructor without parameters is not supported"");
        }}
");
        }

        private void MemoryConstructor2(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public StackMemory(nuint byteCount)
        {{
            Start = NativeMemory.Alloc(byteCount);
            Current = Start;
            ByteCount = byteCount;
            FreeByteCount = byteCount;
            _disposed = false;
        }}
");
        }

        private void MemoryProperties(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public void* Start {{ get; init; }}
        public void* Current {{ get; private set; }}
        public nuint ByteCount {{ get; init; }}
        public nuint FreeByteCount {{ get; private set; }}
");
        }

        private void MemoryAllocateMemory(
            in StringBuilder builder
            )
        {
            builder.Append($@"
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

        private void MemoryTryAllocateMemory(
            in StringBuilder builder
            )
        {
            builder.Append($@"
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

        private void MemoryFreeMemory(
            in StringBuilder builder
            )
        {
            builder.Append($@"
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

        private void MemoryTryFreeMemory(
            in StringBuilder builder
            )
        {
            builder.Append($@"
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
            in StringBuilder builder,
            in string memoryNamespace
            )
        {
            if(memoryNamespace == "Class")
            {
                builder.Append($@"
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
                builder.Append($@"
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

        private void MemoryEnd(
            in StringBuilder builder
            )
        {
            builder.Append($@"
    }}
}}
");
        }
    }
}