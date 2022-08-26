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

                helperBuilder.Append($@"
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
using {currentType.ContainingNamespace};

namespace {currentType.ContainingNamespace}.Struct
{{
    public unsafe struct {currentType.Name}Wrapper : IDisposable
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private readonly StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private readonly void* _start;
        private readonly bool _memoryOwner = false;

        public {currentType.Name}Wrapper()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({typeInfo.Members.Sum(s => s.Size)});
            _start = _stackMemoryC.Start;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}

        public {currentType.Name}Wrapper(
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory->AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
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
            if(!_memoryOwner)
            {{
                if(_stackMemoryC != null)
                {{
                    _stackMemoryC?.FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                }}
                else if (_stackMemoryS != null)
                {{
                    _stackMemoryS->FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                }}
            }}
            else
            {{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }}
            
        }}
    }}
}}
");
                context.AddSource($"{currentType.Name}WrapperStruct.g.cs", helperBuilder.ToString());

                helperBuilder.Clear();
                helperBuilder.Append($@"
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
using {currentType.ContainingNamespace};

namespace {currentType.ContainingNamespace}.Class
{{
    public unsafe class {currentType.Name}Wrapper : IDisposable
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private readonly StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private readonly void* _start;
        private readonly bool _memoryOwner = false;

        public {currentType.Name}Wrapper()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({typeInfo.Members.Sum(s => s.Size)});
            _start = _stackMemoryC.Start;
            _memoryOwner = true;
        }}

        public {currentType.Name}Wrapper(
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory->AllocateMemory({typeInfo.Members.Sum(s => s.Size)});
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
                if(!_memoryOwner)
                {{
                    if (disposing)
                    {{
                        if(_stackMemoryC != null)
                        {{
                            _stackMemoryC?.FreeMemory({typeInfo.Members.Sum(s => s.Size)});
                        }}
                        else if (_stackMemoryS != null)
                        {{
                            _stackMemoryS->FreeMemory({typeInfo.Members.Sum(s => s.Size)});
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
    }}
}}
");
                context.AddSource($"{currentType.Name}WrapperClass.g.cs", helperBuilder.ToString());
            }
        }
    }
}