using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace StackMemoryCollections.Generators.Primitive
{
    internal class PrimitiveWrappersGenerator
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public void GeneratePrimitiveWrappers(
            in GeneratorExecutionContext context
            )
        {
            GenerateWrapper<int>(context, 4);
            GenerateWrapper<uint>(context, 4);

            GenerateWrapper<long>(context, 8);
            GenerateWrapper<ulong>(context, 8);

            GenerateWrapper<sbyte>(context, 1);
            GenerateWrapper<byte>(context, 1);

            GenerateWrapper<short>(context, 2);
            GenerateWrapper<ushort>(context, 2);

            GenerateWrapper<char>(context, 2);
            GenerateWrapper<decimal>(context, 16);
            GenerateWrapper<double>(context, 8);
            GenerateWrapper<bool>(context, 1);//1 byte is not optimal
            GenerateWrapper<float>(context, 4);
        }

        private void GenerateWrapper<T>(
            GeneratorExecutionContext context,
            int sizeOf
            ) where T : unmanaged
        {
            GeneratePrimitiveWrapper<T>(in context, "Class", sizeOf);
            GeneratePrimitiveWrapper<T>(in context, "Struct", sizeOf);
        }

        private void GeneratePrimitiveWrapper<T>(
            in GeneratorExecutionContext context,
            in string wrapperNamespace,
            in int sizeOf
            ) where T : unmanaged
        {
            _builder.Clear();

            PrimitiveWrapperStart<T>(in wrapperNamespace);
            PrimitiveWrapperConstructor1<T>(in sizeOf);
            PrimitiveWrapperConstructor2<T>(in sizeOf);
            PrimitiveWrapperConstructor3<T>(in sizeOf);
            PrimitiveWrapperConstructor4<T>();

            PrimitiveWrapperProperties<T>();

            PrimitiveWrapperDispose<T>(in sizeOf, in wrapperNamespace);

            PrimitiveWrapperEnd();

            context.AddSource($"{typeof(T).Name}Wrapper{wrapperNamespace}.g.cs", _builder.ToString());
        }

        private void PrimitiveWrapperStart<T>(
            in string wrapperNamespace
            ) where T : unmanaged
        {
            _builder.Append($@"
using System;

namespace StackMemoryCollections.{wrapperNamespace}
{{
    public unsafe {wrapperNamespace.ToLowerInvariant()} {typeof(T).Name}Wrapper : IDisposable
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private readonly StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private {typeof(T).Name}* _start;
        private readonly bool _memoryOwner = false;
");
        }

        private void PrimitiveWrapperConstructor1<T>(
            in int sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public {typeof(T).Name}Wrapper()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({sizeOf});
            _start = ({typeof(T).Name}*)_stackMemoryC.Start;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void PrimitiveWrapperConstructor2<T>(
            in int sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public {typeof(T).Name}Wrapper(
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = ({typeof(T).Name}*)stackMemory->AllocateMemory({sizeOf});
            _stackMemoryS = stackMemory;
        }}
");
        }

        private void PrimitiveWrapperConstructor3<T>(
            in int sizeOf
            ) where T : unmanaged
        {
            _builder.Append($@"
        public {typeof(T).Name}Wrapper(
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = ({typeof(T).Name}*)stackMemory.AllocateMemory({sizeOf});
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
        }}
");
        }

        private void PrimitiveWrapperConstructor4<T>() where T : unmanaged
        {
            _builder.Append($@"
        public {typeof(T).Name}Wrapper(
            void* start
            )
        {{
            if (start == null)
            {{
                throw new ArgumentNullException(nameof(start));
            }}

            _start = ({typeof(T).Name}*)start;
            _stackMemoryC = null;
            _stackMemoryS = null;
        }}
");
        }

        private void PrimitiveWrapperDispose<T>(
            in int sizeOf,
            in string wrapperNamespace
            ) where T : unmanaged
        {
            if (wrapperNamespace == "Class")
            {
                _builder.Append($@"
        #region IDisposable

        private bool _disposed;

        ~{typeof(T).Name}Wrapper() => Dispose(false);

        public void Dispose()
        {{
            Dispose(true);
            GC.SuppressFinalize(this);
        }}

        protected virtual void Dispose(bool disposing)
        {{
            if (!_disposed)
            {{
                if(!_memoryOwner)
                {{
                    if (disposing)
                    {{
                        if(_stackMemoryC != null)
                        {{
                            _stackMemoryC?.FreeMemory({sizeOf});
                        }}
                        else if (_stackMemoryS != null)
                        {{
                            _stackMemoryS->FreeMemory({sizeOf});
                        }}
                    }}
                }}
                else
                {{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    _stackMemoryC.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }}

                _disposed = true;
            }}
        }}

        #endregion
");
            }
            else
            {
                _builder.Append($@"
        public void Dispose()
        {{
            if(!_memoryOwner)
            {{
                if(_stackMemoryC != null)
                {{
                    _stackMemoryC?.FreeMemory({sizeOf});
                }}
                else if (_stackMemoryS != null)
                {{
                    _stackMemoryS->FreeMemory({sizeOf});
                }}
            }}
            else
            {{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }}
            
        }}
");
            }
        }

        private void PrimitiveWrapperProperties<T>() where T : unmanaged
        {
            PrimitiveWrapperPtr<T>();
            PrimitiveWrapperGetSet<T>();
            PrimitiveWrapperSetIn<T>();
            PrimitiveWrapperSetPtr<T>();
            PrimitiveWrapperGetOut<T>();
            PrimitiveWrapperChangePtr<T>();
        }

        private void PrimitiveWrapperPtr<T>() where T : unmanaged
        {
            _builder.Append($@"
        public {typeof(T).Name}* Ptr => _start;
");
        }

        private void PrimitiveWrapperGetSet<T>() where T : unmanaged
        {
            _builder.Append($@"
        public {typeof(T).Name} Value
        {{
            get
            {{
                return *_start;
            }}

            set
            {{
                *_start = value;
            }}
        }}
");
        }

        private void PrimitiveWrapperSetIn<T>() where T : unmanaged
        {
            _builder.Append($@"
        public void Set(in {typeof(T).Name} item)
        {{
            *_start = item;
        }}
");
        }

        private void PrimitiveWrapperSetPtr<T>() where T : unmanaged
        {
            _builder.Append($@"
        public void Set(in {typeof(T).Name}* valuePtr)
        {{
            *_start = *valuePtr;
        }}
");
        }

        private void PrimitiveWrapperGetOut<T>() where T : unmanaged
        {
            _builder.Append($@"
        public void GetOut(out {typeof(T).Name} item)
        {{
            item = *_start;
        }}
");
        }

        private void PrimitiveWrapperChangePtr<T>() where T : unmanaged
        {
            _builder.Append($@"
        public void ChangePtr(in {typeof(T).Name}* newPtr)
        {{
            if(_stackMemoryC != null || _stackMemoryS != null)
            {{
                throw new Exception(""Only an instance created via a void* constructor can change the pointer."");
            }}

            _start = newPtr;
        }}
");
        }

        private void PrimitiveWrapperEnd()
        {
            _builder.Append($@"
    }}
}}
");
        }
    }
}