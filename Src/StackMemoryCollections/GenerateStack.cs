using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackMemoryCollections
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
                if (!typeInfos.TryGetValue($"{currentType.ContainingNamespace}.{currentType.Name}", out var typeInfo))
                {
                    throw new Exception($"{nameof(GenerateStack)}: Type information not found, types filling error. Type name: {currentType.ContainingNamespace}.{currentType.Name}");
                }

                GenerateStack(in context, in builder, in currentType, in typeInfo, "Class");
                GenerateStack(in context, in builder, in currentType, in typeInfo, "Struct");
            }
        }

        private void GenerateStack(
            in GeneratorExecutionContext context,
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in TypeInfo typeInfo,
            in string stackNamespace
            )
        {
            builder.Clear();
            var sizeOfStr = typeInfo.IsRuntimeCalculatedSize ? $"{currentType.Name}Helper.SizeOf" : $"{typeInfo.Size}";
            StackStart(in builder, in currentType, in stackNamespace);
            
            StackConstructor1(in builder, in currentType, in typeInfo, in sizeOfStr);
            StackConstructor2(in builder, in currentType, in sizeOfStr);
            StackConstructor3(in builder, in currentType, in sizeOfStr);
            StackConstructor4(in builder, in currentType);

            StackProperties(in builder);

            StackReducingCapacity(in builder, in sizeOfStr, in stackNamespace);
            StackExpandCapacity(in builder, in sizeOfStr, in stackNamespace);
            StackTryExpandCapacity(in builder, in sizeOfStr, in stackNamespace);
            StackTrimExcess(in builder);
            StackPushIn(in builder, in currentType, in stackNamespace, in sizeOfStr);
            StackPushFuture(in builder, in stackNamespace);
            StackPushInPtr(in builder, in currentType, in stackNamespace, in sizeOfStr);
            StackTryPushIn(in builder, in currentType, in stackNamespace, in sizeOfStr);
            StackTryPushInPtr(in builder, in currentType, in stackNamespace, in sizeOfStr);
            StackPop(in builder, in stackNamespace);
            StackTryPop(in builder, in stackNamespace);
            StackClear(in builder, in stackNamespace);
            StackTop(in builder, in currentType, in typeInfo, in sizeOfStr);
            StackTopInPtr(in builder, in currentType, in sizeOfStr);
            StackTopRefValue(in builder, in currentType, in sizeOfStr);
            StackTopPtr(in builder, in sizeOfStr);
            StackTopFuture(in builder, in sizeOfStr);
            StackTopOutValue(in builder, in currentType, in sizeOfStr);
            StackDispose(in builder, in currentType, in stackNamespace, in sizeOfStr);
            StackIndexator(in builder, in sizeOfStr);
            StackCopy(in builder, in sizeOfStr);
            StackCopyCount(in builder, in sizeOfStr);
            StackCopyInStack(in builder, in currentType, in sizeOfStr);

            if (stackNamespace == "Class")
            {
                StackIEnumerable(in builder, in currentType, in sizeOfStr);
            }

            StackEnd(in builder);

            context.AddSource($"StackOf{currentType.Name}{stackNamespace}.g.cs", builder.ToString());
        }

        private void StackStart(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string stackNamespace
            )
        {
            string implements;
            if (stackNamespace == "Class")
            {
                implements = $"IDisposable, System.Collections.Generic.IEnumerable<{currentType.Name}>";
            }
            else
            {
                implements = $"IDisposable";
            }

            builder.Append($@"
/*
{Resource.License}
*/

using System;
using {currentType.ContainingNamespace};
using System.Runtime.CompilerServices;

namespace {currentType.ContainingNamespace}.{stackNamespace}
{{
    public unsafe {stackNamespace.ToLowerInvariant()} StackOf{currentType.Name} : {implements}
    {{
        private readonly StackMemoryCollections.Struct.StackMemory* _stackMemoryS;
        private StackMemoryCollections.Class.StackMemory _stackMemoryC = null;
        private void* _start;
        private readonly bool _memoryOwner = false;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
        private int _version = 0;
");
            }
        }

        private void StackConstructor1(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in TypeInfo typeInfo,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public StackOf{currentType.Name}()
        {{
            _stackMemoryC = new StackMemoryCollections.Class.StackMemory({(typeInfo.IsRuntimeCalculatedSize ? sizeOfStr + "* 4" : (typeInfo.Size * 4).ToString())});
            _start = _stackMemoryC.Start;
            Capacity = 4;
            _memoryOwner = true;
            _stackMemoryS = null;
        }}
");
        }

        private void StackConstructor2(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public StackOf{currentType.Name}(
            nuint count,
            StackMemoryCollections.Struct.StackMemory* stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory->AllocateMemory({sizeOfStr} * count);
            _stackMemoryS = stackMemory;
            Capacity = count;
        }}
");
        }

        private void StackConstructor3(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public StackOf{currentType.Name}(
            nuint count,
            StackMemoryCollections.Class.StackMemory stackMemory
            )
        {{
            if (stackMemory == null)
            {{
                throw new ArgumentNullException(nameof(stackMemory));
            }}

            _start = stackMemory.AllocateMemory({sizeOfStr} * count);
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
            Capacity = count;
        }}
");
        }

        private void StackConstructor4(
            in StringBuilder builder,
            in INamedTypeSymbol currentType
            )
        {
            builder.Append($@"
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
");
        }

        private void StackProperties(
            in StringBuilder builder
            )
        {
            builder.Append($@"
        public nuint Capacity {{ get; private set; }}

        public nuint Size {{ get; set; }} = 0;

        public bool IsEmpty => Size == 0;

        public void* Start => _start;
");
        }

        private void StackReducingCapacity(
            in StringBuilder builder,
            in string sizeOfStr,
            in string stackNamespace
            )
        {
            var incrementVersion = stackNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
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
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOfStr} * (Capacity - reducingCount));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Buffer.MemoryCopy(
                    _stackMemoryC.Start,
                    newMemory.Start,
                    newMemory.ByteCount,
                    {sizeOfStr} * (Capacity - reducingCount)
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                _stackMemoryC.Dispose();
                _stackMemoryC = newMemory;
                _start = _stackMemoryC.Start;{incrementVersion}
            }}
            else if(_stackMemoryS != null)
            {{
                if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOfStr})))
                {{
                    throw new Exception(""Failed to reduce available memory, stack moved further"");
                }}

                _stackMemoryS->FreeMemory(reducingCount * {sizeOfStr});
            }}
            else if (_stackMemoryC != null)
            {{
                if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {sizeOfStr})))
                {{
                    throw new Exception(""Failed to reduce available memory, stack moved further"");
                }}

                _stackMemoryC.FreeMemory(reducingCount * {sizeOfStr});
            }}

            Capacity -= reducingCount;
        }}
");
        }

        private void StackExpandCapacity(
            in StringBuilder builder,
            in string sizeOfStr,
            in string stackNamespace
            )
        {
            var incrementVersion = stackNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public void ExpandCapacity(in nuint expandCount)
        {{
            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOfStr} * (Capacity + expandCount));
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
                _start = _stackMemoryC.Start;{incrementVersion}
            }}
            else if(_stackMemoryS != null)
            {{
                if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOfStr})))
                {{
                    throw new Exception(""Failed to expand available memory, stack moved further"");
                }}

                _stackMemoryS->AllocateMemory(expandCount * {sizeOfStr});
            }}
            else if (_stackMemoryC != null)
            {{
                if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {sizeOfStr})))
                {{
                    throw new Exception(""Failed to expand available memory, stack moved further"");
                }}

                _stackMemoryC.AllocateMemory(expandCount * {sizeOfStr});
            }}

            Capacity += expandCount;
        }}
");
        }

        private void StackTryExpandCapacity(
            in StringBuilder builder,
            in string sizeOfStr,
            in string stackNamespace
            )
        {
            var incrementVersion = stackNamespace == "Class" ?
                @"
                _version += 1;
"
:
"";
            builder.Append($@"
        public bool TryExpandCapacity(in nuint expandCount)
        {{
            if (_memoryOwner)
            {{
                var newMemory = new StackMemoryCollections.Class.StackMemory({sizeOfStr} * (Capacity + expandCount));
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
                _start = _stackMemoryC.Start;{incrementVersion}
            }}
            else if(_stackMemoryS != null)
            {{
                if (new IntPtr(_stackMemoryS->Current) != new IntPtr((byte*)_start + (Capacity * {sizeOfStr})))
                {{
                    return false;
                }}

                if(!_stackMemoryS->TryAllocateMemory(expandCount * {sizeOfStr}, out _))
                {{
                    return false;
                }}
            }}
            else if (_stackMemoryC != null)
            {{
                if (new IntPtr(_stackMemoryC.Current) != new IntPtr((byte*)_start + (Capacity * {sizeOfStr})))
                {{
                    throw new Exception(""Failed to expand available memory, stack moved further"");
                }}

                if(!_stackMemoryC.TryAllocateMemory(expandCount * {sizeOfStr}, out _))
                {{
                    return false;
                }}
            }}

            Capacity += expandCount;
            return true;
        }}
");
        }

        private void StackTrimExcess(
            in StringBuilder builder
            )
        {
            builder.Append($@"
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
");
        }

        private void StackPushIn(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string stackNamespace,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void Push(in {currentType.Name} item)
        {{
            if (Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (Size * {sizeOfStr}));
            Size += 1;
");
            if(stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
        }}
");
        }

        private void StackPushFuture(
            in StringBuilder builder,
            in string stackNamespace
            )
        {
            builder.Append($@"
        public void PushFuture()
        {{
            if (Size == Capacity)
            {{
                throw new Exception(""Not enough memory to allocate stack element"");
            }}

            Size += 1;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
        }}
");
        }

        private void StackPushInPtr(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string stackNamespace,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void Push(in void* ptr)
        {{
            if (Size == Capacity)
            {{
                ExpandCapacity(_memoryOwner ? Capacity : 1);
            }}

            {currentType.Name}Helper.Copy(in ptr, (byte*)_start + (Size * {sizeOfStr}));
            Size += 1;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
        }}
");
        }

        private void StackTryPushIn(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string stackNamespace,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public bool TryPush(in {currentType.Name} item)
        {{
            if (Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            {currentType.Name}Helper.CopyToPtr(in item, (byte*)_start + (Size * {sizeOfStr}));
            Size += 1;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
            return true;
        }}
");
        }

        private void StackTryPushInPtr(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string stackNamespace,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public bool TryPush(in void* ptr)
        {{
            if (Size == Capacity)
            {{
                if(!TryExpandCapacity(_memoryOwner ? Capacity : 1))
                {{
                    return false;
                }}
            }}

            {currentType.Name}Helper.Copy(in ptr, (byte*)_start + (Size * {sizeOfStr}));
            Size += 1;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
            return true;
        }}
");
        }

        private void StackPop(
            in StringBuilder builder,
            in string stackNamespace
            )
        {
            builder.Append($@"
        public void Pop()
        {{
            if (Size <= 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            Size -= 1;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
        }}
");
        }

        private void StackTryPop(
            in StringBuilder builder,
            in string stackNamespace
            )
        {
            builder.Append($@"
        public bool TryPop()
        {{
            if (Size <= 0)
            {{
                return false;
            }}

            Size -= 1;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"

            return true;
        }}
");
        }

        private void StackClear(
            in StringBuilder builder,
            in string stackNamespace
            )
        {
            builder.Append($@"
        public void Clear()
        {{
            if (Size != 0)
            {{
                Size = 0;
");
            if (stackNamespace == "Class")
            {
                builder.Append($@"
            _version += 1;
");
            }
            builder.Append($@"
            }}
        }}
");
        }

        private void StackTop(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in TypeInfo typeInfo,
            in string sizeOfStr
            )
        {
            if (typeInfo.IsValueType)
            {
                builder.Append($@"
        [SkipLocalsInit]
        public {typeInfo.TypeName} Top()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            {typeInfo.TypeName} result;
            Unsafe.SkipInit(out result);
            
            {typeInfo.TypeName}Helper.CopyToValue((byte*)_start + ((Size - 1) * {sizeOfStr}), ref result);

            return result;
        }}
");
                return;
            }

            builder.Append($@"
        public {currentType.Name} Top()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            {currentType.Name} result = new {currentType.Name}();
            {currentType.Name}Helper.CopyToValue((byte*)_start + ((Size - 1) * {sizeOfStr}), ref result);
            return
                result;
        }}
");
        }

        private void StackTopInPtr(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void Top(in void* ptr)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            {currentType.Name}Helper.Copy((byte*)_start + ((Size - 1) * {sizeOfStr}), in ptr);
        }}
");
        }

        private void StackTopRefValue(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void Top(ref {currentType.Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            {currentType.Name}Helper.CopyToValue((byte*)_start + ((Size - 1) * {sizeOfStr}), ref item);
        }}
");
        }

        private void StackTopOutValue(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void TopOut(out {currentType.Name} item)
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            {currentType.Name}Helper.CopyToValueOut((byte*)_start + ((Size - 1) * {sizeOfStr}), out item);
        }}
");
        }

        private void StackTopPtr(
            in StringBuilder builder,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void* TopPtr()
        {{
            if (Size == 0)
            {{
                throw new Exception(""There are no elements on the stack"");
            }}

            return (byte*)_start + ((Size - 1) * {sizeOfStr});
        }}
");
        }

        private void StackTopFuture(
            in StringBuilder builder,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void* TopFuture()
        {{
            if (Capacity == 0 || Size == Capacity)
            {{
                throw new Exception(""Future element not available"");
            }}

            return (byte*)_start + (Size * {sizeOfStr});
        }}
");
        }

        private void StackDispose(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string stackNamespace,
            in string sizeOfStr
            )
        {
            if(stackNamespace == "Class")
            {
                builder.Append($@"
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
                            _stackMemoryS->FreeMemory(Capacity * {sizeOfStr});
                        }}
                        else if (_stackMemoryC != null)
                        {{
                            _stackMemoryC.FreeMemory(Capacity * {sizeOfStr});
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
                if(_stackMemoryS != null)
                {{
                    _stackMemoryS->FreeMemory(Capacity * {sizeOfStr});
                }}
                else if (_stackMemoryC != null)
                {{
                    _stackMemoryC.FreeMemory(Capacity * {sizeOfStr});
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

        private void StackIEnumerable(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOfStr
            )
        {
            builder.Append($@"
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

                if (_stack.Size == 0)
                {{
                    return false;
                }}

                if (_currentIndex == -2)
                {{
                    _currentIndex = (int)_stack.Size - 1;
                    _current = (byte*)_stack._start + (_currentIndex * (int){sizeOfStr});
                    return true;
                }}

                if (_currentIndex == -1)
                {{
                    return false;
                }}

                if (--_currentIndex >= 0)
                {{
                    _current = (byte*)_stack._start + (_currentIndex * (int){sizeOfStr});
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
");
        }

        private void StackIndexator(
            in StringBuilder builder,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void* this[nuint index]
        {{
            get
            {{
                if (Size <= 0 || Size <= index)
                {{
                    throw new Exception(""Element outside the stack"");
                }}

                return
                    (byte*)_start + ((Size - 1 - index) * {sizeOfStr});
            }}
        }}
");
        }

        private void StackCopy(
            in StringBuilder builder,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void Copy(in void* ptrDest)
        {{
            if(Size == 0)
            {{
                return;
            }}

            Buffer.MemoryCopy(
                _start,
                ptrDest,
                Size * {sizeOfStr},
                Size * {sizeOfStr}
                );
        }}
");
        }

        private void StackCopyCount(
            in StringBuilder builder,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void Copy(in void* ptrDest, in nuint count)
        {{
            if(Size < count)
            {{
                throw new Exception(""The collection does not have that many elements"");
            }}

            Buffer.MemoryCopy(
                _start,
                ptrDest,
                count * {sizeOfStr},
                count * {sizeOfStr}
                );
        }}
");
        }

        private void StackCopyInStack(
            in StringBuilder builder,
            in INamedTypeSymbol currentType,
            in string sizeOfStr
            )
        {
            builder.Append($@"
        public void Copy(in {currentType.ContainingNamespace}.Class.StackOf{currentType.Name} destStack)
        {{
            if(Size == 0)
            {{
                destStack.Size = 0;
                return;
            }}

            if (destStack.Capacity < Size)
            {{
                throw new ArgumentException(""Destination stack not enough capacity"");
            }}

            Buffer.MemoryCopy(
                _start,
                destStack.Start,
                destStack.Capacity * {sizeOfStr},
                Size * {sizeOfStr}
                );

            destStack.Size = Size;
        }}
");
        }

        private void StackEnd(
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