﻿using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace StackGenerators
{
    public partial class Generator
    {
        private void GenerateWrappers(
            in List<INamedTypeSymbol> typeWrappers,
            in GeneratorExecutionContext context,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            var helperBuilder = new StringBuilder();
            for (int i = 0; i < typeWrappers.Count; i++)
            {
                var currentType = typeWrappers[i];
                helperBuilder.Clear();
                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                helperBuilder.Append($@"// <auto-generated by Brevnov Vyacheslav Sergeevich/>
using System;
using {currentType.ContainingNamespace};

namespace {currentType.ContainingNamespace}.Struct
{{
    public unsafe struct {currentType.Name}Wrapper : IDisposable
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private readonly StackMemoryCollections.Class.StackMemory? _stackMemoryC = null;
        private readonly void* _start;

        public {currentType.Name}Wrapper()
        {{
            throw new Exception(""Default constructor not supported"");
        }}

        public {currentType.Name}Wrapper(
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = (*stackMemory).AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
            _stackMemoryS = stackMemory;
        }}

        public {currentType.Name}Wrapper(
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory.AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
        }}

        public {currentType.Name}Wrapper(
            void* start
            )
        {{
            if (start == null)
            {{
                throw new ArgumentNullException(nameof(start));
            }}

            _start = start;
            _stackMemoryC = null;
            _stackMemoryS = null;
        }}

        public void* Ptr => _start;

        public void Dispose()
        {{
            if(_stackMemoryC != null)
            {{
                _stackMemoryC?.FreeMemory({typeInfo.Members.Sum(s => s.Size)});
            }}
            else if (_stackMemoryS != null)
            {{
                (*_stackMemoryS).FreeMemory({typeInfo.Members.Sum(s => s.Size)});
            }}
        }}
    }}
}}
");
                context.AddSource($"{currentType.Name}WrapperStruct.g.cs", helperBuilder.ToString());

                helperBuilder.Clear();
                helperBuilder.Append($@"// <auto-generated by Brevnov Vyacheslav Sergeevich/>
using System;
using {currentType.ContainingNamespace};

namespace {currentType.ContainingNamespace}.Class
{{
    public unsafe class {currentType.Name}Wrapper : IDisposable
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private readonly StackMemoryCollections.Class.StackMemory? _stackMemoryC = null;
        private readonly void* _start;

        public {currentType.Name}Wrapper()
        {{
            throw new Exception(""Default constructor not supported"");
        }}

        public {currentType.Name}Wrapper(
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = (*stackMemory).AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
            _stackMemoryS = stackMemory;
        }}

        public {currentType.Name}Wrapper(
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory.AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
        }}

        public {currentType.Name}Wrapper(
            void* start
            )
        {{
            if (start == null)
            {{
                throw new ArgumentNullException(nameof(start));
            }}

            _start = start;
            _stackMemoryC = null;
            _stackMemoryS = null;
        }}

        public void* Ptr => _start;

        #region IDisposable

        private bool _disposed;

        ~{currentType.Name}Wrapper() => Dispose(false);

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
                    if(_stackMemoryC != null)
                    {{
                        _stackMemoryC?.FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                    }}
                    else if (_stackMemoryS != null)
                    {{
                        (*_stackMemoryS).FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                    }}
                }}

                _disposed = true;
            }}
        }}

        #endregion
    }}
}}
");
                context.AddSource($"{currentType.Name}WrapperClass.g.cs", helperBuilder.ToString());
            }
        }
    }
}