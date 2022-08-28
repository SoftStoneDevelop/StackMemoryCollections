using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace StackMemoryCollections
{
    public partial class Generator
    {
        private void GeneratePrimitiveWrappers(
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            GeneratePrimitiveWrapper<Int32>(in context, in builder, "Class", 4);
            GeneratePrimitiveWrapper<Int32>(in context, in builder, "Struct", 4);

            GeneratePrimitiveWrapper<UInt32>(in context, in builder, "Class", 4);
            GeneratePrimitiveWrapper<UInt32>(in context, in builder, "Struct", 4);

            GeneratePrimitiveWrapper<Int64>(in context, in builder, "Class", 8);
            GeneratePrimitiveWrapper<Int64>(in context, in builder, "Struct", 8);

            GeneratePrimitiveWrapper<UInt64>(in context, in builder, "Class", 8);
            GeneratePrimitiveWrapper<UInt64>(in context, in builder, "Struct", 8);

            GeneratePrimitiveWrapper<SByte>(in context, in builder, "Class", 1);
            GeneratePrimitiveWrapper<SByte>(in context, in builder, "Struct", 1);

            GeneratePrimitiveWrapper<Byte>(in context, in builder, "Class", 1);
            GeneratePrimitiveWrapper<Byte>(in context, in builder, "Struct", 1);

            GeneratePrimitiveWrapper<Int16>(in context, in builder, "Class", 2);
            GeneratePrimitiveWrapper<Int16>(in context, in builder, "Struct", 2);

            GeneratePrimitiveWrapper<UInt16>(in context, in builder, "Class", 2);
            GeneratePrimitiveWrapper<UInt16>(in context, in builder, "Struct", 2);

            GeneratePrimitiveWrapper<Char>(in context, in builder, "Class", 2);
            GeneratePrimitiveWrapper<Char>(in context, in builder, "Struct", 2);

            GeneratePrimitiveWrapper<Decimal>(in context, in builder, "Class", 16);
            GeneratePrimitiveWrapper<Decimal>(in context, in builder, "Struct", 16);

            GeneratePrimitiveWrapper<Double>(in context, in builder, "Class", 8);
            GeneratePrimitiveWrapper<Double>(in context, in builder, "Struct", 8);

            GeneratePrimitiveWrapper<Boolean>(in context, in builder, "Class", 1);//1 byte is not optimal
            GeneratePrimitiveWrapper<Boolean>(in context, in builder, "Struct", 1);//1 byte is not optimal

            GeneratePrimitiveWrapper<Single>(in context, in builder, "Class", 4);
            GeneratePrimitiveWrapper<Single>(in context, in builder, "Struct", 4);
        }

        private void GeneratePrimitiveWrapper<T>(
            in GeneratorExecutionContext context,
            in StringBuilder builder,
            in string wrapperNamespace,
            in int sizeOf
            ) where T : unmanaged
        {
            builder.Clear();

            PrimitiveWrapperStart<T>(in builder, in wrapperNamespace);
            PrimitiveWrapperConstructor1<T>(in sizeOf, in builder);
            PrimitiveWrapperConstructor2<T>(in sizeOf, in builder);
            PrimitiveWrapperConstructor3<T>(in sizeOf, in builder);
            PrimitiveWrapperConstructor4<T>(in builder);

            PrimitiveWrapperProperties<T>(in builder);

            PrimitiveWrapperDispose<T>(in sizeOf, in builder, in wrapperNamespace);

            PrimitiveWrapperEnd(in builder);

            context.AddSource($"{typeof(T).Name}Wrapper{wrapperNamespace}.g.cs", builder.ToString());
        }

        private void PrimitiveWrapperStart<T>(
            in StringBuilder builder,
            in string wrapperNamespace
            ) where T : unmanaged
        {
            builder.Append($@"
/*
{Resource.License}
*/

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
            in int sizeOf,
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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
            in int sizeOf,
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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
            in int sizeOf,
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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

        private void PrimitiveWrapperConstructor4<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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
            in StringBuilder builder,
            in string wrapperNamespace
            ) where T : unmanaged
        {
            if(wrapperNamespace == "Class")
            {
                builder.Append($@"
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
                builder.Append($@"
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

        private void PrimitiveWrapperProperties<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            PrimitiveWrapperPtr<T>(in builder);
            PrimitiveWrapperGetSet<T>(in builder);
            PrimitiveWrapperSetIn<T>(in builder);
            PrimitiveWrapperSetPtr<T>(in builder);
            PrimitiveWrapperGetOut<T>(in builder);
            PrimitiveWrapperChangePtr<T>(in builder);
        }

        private void PrimitiveWrapperPtr<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public {typeof(T).Name}* Ptr => _start;
");
        }

        private void PrimitiveWrapperGetSet<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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

        private void PrimitiveWrapperSetIn<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Set(in {typeof(T).Name} item)
        {{
            *_start = item;
        }}
");
        }

        private void PrimitiveWrapperSetPtr<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
        public void Set(in {typeof(T).Name}* valuePtr)
        {{
            *_start = *valuePtr;
        }}
");
        }

        private void PrimitiveWrapperGetOut<T>(
            in StringBuilder builder
            ) where T: unmanaged
        {
            builder.Append($@"
        public void GetOut(out {typeof(T).Name} item)
        {{
            item = *_start;
        }}
");
        }

        private void PrimitiveWrapperChangePtr<T>(
            in StringBuilder builder
            ) where T : unmanaged
        {
            builder.Append($@"
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

        private void PrimitiveWrapperEnd(
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