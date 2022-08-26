using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace StackGenerators
{
    public partial class Generator
    {
        private void GenerateStack(
            in List<INamedTypeSymbol> typeStack,
            in GeneratorExecutionContext context,
            in Dictionary<string, TypeInfo> typeInfos,
            in StringBuilder builder
            )
        {
            for (int i = 0; i < typeStack.Count; i++)
            {
                var currentType = typeStack[i];
                builder.Clear();
                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                builder.Append($@"
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
    public unsafe struct StackOf{currentType.Name} : IDisposable
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private void* _start;
        private readonly bool _memoryOwner = false;

        public StackOf{currentType.Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({typeInfo.Members.Sum(s => s.Size) * 4});
            _start = _stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}

        public StackOf{currentType.Name}(
            nuint count,
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory->AllocateMemory({typeInfo.Members.Sum(s => s.Size)} * count);
            _stackMemoryS = stackMemory;
            Capacity = count;
        }}

        public StackOf{currentType.Name}(
            nuint count,
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory.AllocateMemory({typeInfo.Members.Sum(s => s.Size)} * count);
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
            Capacity = count;
        }}

        public StackOf{currentType.Name}(
            nuint count,
            void* memoryStart
            )
        {{
            if (memoryStart == null)
            {{
                throw new ArgumentNullException(nameof(memoryStart));
            }}

            _start = memoryStart;
            _stackMemoryS = null;
            Capacity = count;
        }}

        public nuint Capacity {{ get; private set; }}

        public nuint Size {{ get; set; }} = 0;

        public bool IsEmpty => Size == 0;

        public void* Start => _start;

        public void ReducingCapacity(in nuint reducingCount)
        {{
            if (reducingCount <= 0)
            {{
                return;
            }}

            if (Size > 0 && Size < Capacity - reducingCount)
            {{
                throw new Exception(""Can't reduce available memory, it's already in use"");
            }}

            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({typeInfo.Members.Sum(s => s.Size)} * (Capacity - reducingCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    {typeInfo.Members.Sum(s => s.Size)} * (Capacity - reducingCount)
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;
            }}
            else
            {{
                if(_stackMemoryS != null)
                {{
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {typeInfo.Members.Sum(s => s.Size)})))
                    {{
                        throw new Exception(""Failed to reduce available memory, stack moved further"");
                    }}

                    _stackMemoryS->FreeMemory(reducingCount * {typeInfo.Members.Sum(s => s.Size)});
                }}
                else if (_stackMemoryC != null)
                {{
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {typeInfo.Members.Sum(s => s.Size)})))
                    {{
                        throw new Exception(""Failed to reduce available memory, stack moved further"");
                    }}

                    _stackMemoryC.FreeMemory(reducingCount * {typeInfo.Members.Sum(s => s.Size)});
                }}
            }}

            Capacity -= reducingCount;
        }}

        public void ExpandCapacity(in nuint expandCount)
        {{

            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({typeInfo.Members.Sum(s => s.Size)} * (Capacity + expandCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    _stackMemoryC.ByteCount
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;
            }}
            else
            {{
                if(_stackMemoryS != null)
                {{
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {typeInfo.Members.Sum(s => s.Size)})))
                    {{
                        throw new Exception(""Failed to expand available memory, stack moved further"");
                    }}

                    _stackMemoryS->AllocateMemory(expandCount * {typeInfo.Members.Sum(s => s.Size)});
                }}
                else if (_stackMemoryC != null)
                {{
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {typeInfo.Members.Sum(s => s.Size)})))
                    {{
                        throw new Exception(""Failed to expand available memory, stack moved further"");
                    }}

                    _stackMemoryC.AllocateMemory(expandCount * {typeInfo.Members.Sum(s => s.Size)});
                }}
            }}

            Capacity += expandCount;
        }}

        public void TrimExcess()
        {{
            if (_memoryOwner)
            {{
                ReducingCapacity(
                    Size == 0 ?
                        Capacity > 4 ? (nuint)(-(4 - (long)Capacity))
                            : 0
                        : Capacity - Size
                        );
            }}
            else
            {{
                ReducingCapacity(Capacity - Size);
            }}
        }}


        public void Push(in {currentType.Name} item)
        {{
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {{
                if (_memoryOwner)
                {{
                    ExpandCapacity(Capacity);
                }}
                else
                {{
                    throw new Exception(""Not enough memory to allocate stack element"");
                }}
            }}

            {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (Size * {typeInfo.Members.Sum(s => s.Size)}));
            Size = tempSize;
        }}

        public bool TryPush(in {currentType.Name} item)
        {{
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {{
                return false;
            }}

            {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (Size * {typeInfo.Members.Sum(s => s.Size)}));
            Size = tempSize;

            return true;
        }}

        public void Pop()
        {{
            if (Size <= 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            Size--;
        }}

        public void Clear()
        {{
            if (Size != 0)
            {{
                Size = 0;
            }}
        }}

        public {currentType.Name} Top()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            {currentType.Name} result = new {currentType.Name}();
            {currentType.Name}Helper.CopyToValue((byte*)_start + ((Size - 1) * {typeInfo.Members.Sum(s => s.Size)}), ref result);
            return
                result;
        }}

        public void* TopPtr()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            return (byte*)_start + ((Size - 1) * {typeInfo.Members.Sum(s => s.Size)});
        }}

        public void Dispose()
        {{
            if(!_memoryOwner)
            {{
                if(_stackMemoryS != null)
                {{
                    _stackMemoryS->FreeMemory(Capacity * {typeInfo.Members.Sum(s => s.Size)});
                }}
                else if (_stackMemoryC != null)
                {{
                    _stackMemoryC.FreeMemory(Capacity * {typeInfo.Members.Sum(s => s.Size)});
                }}
            }}
            else
            {{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }}
            
        }}

        public void* this[nuint index]
        {{
            get
            {{
                if (Size <= 0 || Size <= index)
                {{
                    throw new Exception(""Element outside the stack"");
                }}

                return
                    (byte*)_start + ((Size - 1 - index) * {typeInfo.Members.Sum(s => s.Size)});
            }}
        }}

        public void Copy(in void* ptrDest)
        {{
            Buffer.MemoryCopy(
                _start,
                ptrDest,
                Capacity * {typeInfo.Members.Sum(s => s.Size)},
                Capacity * {typeInfo.Members.Sum(s => s.Size)}
                );
        }}

        public void Copy(in {currentType.ContainingNamespace}.Class.StackOf{currentType.Name} destStack)
        {{
            if (destStack.Capacity < Capacity)
            {{
                throw new ArgumentException(""Destination stack not enough capacity"");
            }}

            Buffer.MemoryCopy(
                _start,
                destStack.Start,
                destStack.Capacity * {typeInfo.Members.Sum(s => s.Size)},
                Capacity * {typeInfo.Members.Sum(s => s.Size)}
                );

            destStack.Size = Size;
        }}
    }}
}}
");
                context.AddSource($"{currentType.Name}StackStruct.g.cs", builder.ToString());

                builder.Clear();
                builder.Append($@"
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
using System.Collections;
using {currentType.ContainingNamespace};

namespace {currentType.ContainingNamespace}.Class
{{
    public unsafe class StackOf{currentType.Name} : IDisposable, System.Collections.Generic.IEnumerable<{currentType.Name}>
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private void* _start;
        private readonly bool _memoryOwner = false;
        private int _version = 0;

        public StackOf{currentType.Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({typeInfo.Members.Sum(s => s.Size) * 4});
            _start = _stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}

        public StackOf{currentType.Name}(
            nuint count,
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory->AllocateMemory({typeInfo.Members.Sum(s => s.Size)} * count);
            _stackMemoryS = stackMemory;
            Capacity = count;
        }}

        public StackOf{currentType.Name}(
            nuint count,
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory.AllocateMemory({typeInfo.Members.Sum(s => s.Size)} * count);
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
            Capacity = count;
        }}

        public StackOf{currentType.Name}(
            nuint count,
            void* memoryStart
            )
        {{
            if (memoryStart == null)
            {{
                throw new ArgumentNullException(nameof(memoryStart));
            }}

            _start = memoryStart;
            _stackMemoryS = null;
            Capacity = count;
        }}

        public nuint Capacity {{ get; private set; }}

        public nuint Size {{ get; set; }} = 0;

        public bool IsEmpty => Size == 0;

        public void* Start => _start;

        public void ReducingCapacity(in nuint reducingCount)
        {{
            if (reducingCount <= 0)
            {{
                return;
            }}

            if (Size > 0 && Size < Capacity - reducingCount)
            {{
                throw new Exception(""Can't reduce available memory, it's already in use"");
            }}

            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({typeInfo.Members.Sum(s => s.Size)} * (Capacity - reducingCount ));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    {typeInfo.Members.Sum(s => s.Size)} * (Capacity - reducingCount)
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;
            }}
            else
            {{
                if(_stackMemoryS != null)
                {{
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {typeInfo.Members.Sum(s => s.Size)})))
                    {{
                        throw new Exception(""Failed to reduce available memory, stack moved further"");
                    }}

                    _stackMemoryS->FreeMemory(reducingCount * {typeInfo.Members.Sum(s => s.Size)});
                }}
                else if (_stackMemoryC != null)
                {{
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {typeInfo.Members.Sum(s => s.Size)})))
                    {{
                        throw new Exception(""Failed to reduce available memory, stack moved further"");
                    }}

                    _stackMemoryC.FreeMemory(reducingCount * {typeInfo.Members.Sum(s => s.Size)});
                }}
            }}

            Capacity -= reducingCount;
        }}

        public void ExpandCapacity(in nuint expandCount)
        {{
            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({typeInfo.Members.Sum(s => s.Size)} * (Capacity + expandCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    _stackMemoryC.ByteCount
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;
            }}
            else
            {{
                if(_stackMemoryS != null)
                {{
                    if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {typeInfo.Members.Sum(s => s.Size)})))
                    {{
                        throw new Exception(""Failed to expand available memory, stack moved further"");
                    }}

                    _stackMemoryS->AllocateMemory(expandCount * {typeInfo.Members.Sum(s => s.Size)});
                }}
                else if (_stackMemoryC != null)
                {{
                    if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {typeInfo.Members.Sum(s => s.Size)})))
                    {{
                        throw new Exception(""Failed to expand available memory, stack moved further"");
                    }}

                    _stackMemoryC.AllocateMemory(expandCount * {typeInfo.Members.Sum(s => s.Size)});
                }}
            }}

            Capacity += expandCount;
        }}

        public void TrimExcess()
        {{
            if(_memoryOwner)
            {{
                ReducingCapacity(
                    Size == 0 ?
                        Capacity > 4 ? (nuint)(-(4 - (long)Capacity))
                            : 0
                        : Capacity - Size
                        );
            }}
            else
            {{
                ReducingCapacity(Capacity - Size);
            }}
        }}


        public void Push(in {currentType.Name} item)
        {{
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {{
                if(_memoryOwner)
                {{
                    ExpandCapacity(Capacity);
                }}
                else
                {{
                    throw new Exception(""Not enough memory to allocate stack element"");
                }}
            }}

            {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (Size * {typeInfo.Members.Sum(s => s.Size)}));
            Size = tempSize;
            _version++;
        }}

        public bool TryPush(in {currentType.Name} item)
        {{
            var tempSize = Size + 1;
            if (tempSize > Capacity)
            {{
                return false;
            }}

            {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (Size * {typeInfo.Members.Sum(s => s.Size)}));
            Size = tempSize;
            _version++;

            return true;
        }}

        public void Pop()
        {{
            if (Size <= 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            Size--;
            _version++;
        }}

        public void Clear()
        {{
            if (Size != 0)
            {{
                Size = 0;
                _version++;
            }}
        }}

        public {currentType.Name} Top()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            {currentType.Name} result = new {currentType.Name}();
            {currentType.Name}Helper.CopyToValue((byte*)_start + ((Size - 1) * {typeInfo.Members.Sum(s => s.Size)}), ref result);
            return
                result;
        }}

        public void* TopPtr()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            return (byte*)_start + ((Size - 1) * {typeInfo.Members.Sum(s => s.Size)});
        }}

        #region IDisposable

        private bool _disposed;

        ~StackOf{currentType.Name}() => Dispose(false);

        public void Dispose()
        {{
            Dispose(true);
            GC.SuppressFinalize(this);
        }}

        protected virtual void Dispose(bool disposing)
        {{
            if (!_disposed)
            {{
                if (!_memoryOwner)
                {{
                    if (disposing)
                    {{
                        if(_stackMemoryS != null)
                        {{
                            _stackMemoryS->FreeMemory(Capacity * {typeInfo.Members.Sum(s => s.Size)});
                        }}
                        else if (_stackMemoryC != null)
                        {{
                            _stackMemoryC.FreeMemory(Capacity * {typeInfo.Members.Sum(s => s.Size)});
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

        #region IEnumerable<T>

        public System.Collections.Generic.IEnumerator<{currentType.Name}> GetEnumerator()
        {{
            return new Enumerator(this);
        }}

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {{
            return new Enumerator(this);
        }}

        public struct Enumerator : System.Collections.Generic.IEnumerator<{currentType.Name}>, System.Collections.IEnumerator
        {{
            private readonly StackOf{currentType.Name} _stack;
            private void* _current;
            private int _currentIndex;
            private int _version;

            internal Enumerator(StackOf{currentType.Name} stack)
            {{
                _stack = stack;
                _currentIndex = -1;
                _current = default;
                _version = _stack._version;
            }}

            public {currentType.Name} Current 
            {{
                get
                {{
                    {currentType.Name} result = new {currentType.Name}();
                    {currentType.Name}Helper.CopyToValue(_current, ref result);
                    return result;
                }}
            }}

            object System.Collections.IEnumerator.Current => Current;

            public void Dispose()
            {{
                _currentIndex = -1;
            }}

            public bool MoveNext()
            {{
                if (_version != _stack._version)
                {{
                    throw new InvalidOperationException(""The stack was changed during the enumeration"");
                }}

                if (_stack.Size < 0)
                {{
                    return false;
                }}

                if (_currentIndex == -2)
                {{
                    _currentIndex = (int)_stack.Size - 1;
                    _current = (byte*)_stack._start + (_currentIndex * {typeInfo.Members.Sum(s => s.Size)});
                    return true;
                }}

                if (_currentIndex == -1)
                {{
                    return false;
                }}

                --_currentIndex;
                if (_currentIndex >= 0)
                {{
                    _current = (byte*)_stack._start + (_currentIndex * {typeInfo.Members.Sum(s => s.Size)});
                    return true;
                }}
                else
                {{
                    _current = default;
                    return false;
                }}
            }}

            public void Reset()
            {{
                _currentIndex = -2;
            }}
        }}

        #endregion

        public void* this[nuint index]
        {{
            get
            {{
                if (Size <= 0 || Size <= index)
                {{
                    throw new Exception(""Element outside the stack"");
                }}

                return
                    (byte*)_start + ((Size - 1 - index) * {typeInfo.Members.Sum(s => s.Size)});
            }}
        }}

        public void Copy(in void* ptrDest)
        {{
            Buffer.MemoryCopy(
                _start,
                ptrDest,
                Capacity * {typeInfo.Members.Sum(s => s.Size)},
                Capacity * {typeInfo.Members.Sum(s => s.Size)}
                );
        }}

        public void Copy(in {currentType.ContainingNamespace}.Class.StackOf{currentType.Name} destStack)
        {{
            if (destStack.Capacity < Capacity)
            {{
                throw new ArgumentException(""Destination stack not enough capacity"");
            }}

            Buffer.MemoryCopy(
                _start,
                destStack.Start,
                destStack.Capacity * {typeInfo.Members.Sum(s => s.Size)},
                Capacity * {typeInfo.Members.Sum(s => s.Size)}
                );

            destStack.Size = Size;
        }}
    }}
}}
");
                context.AddSource($"{currentType.Name}StackClass.g.cs", builder.ToString());
            }
        }
    }
}