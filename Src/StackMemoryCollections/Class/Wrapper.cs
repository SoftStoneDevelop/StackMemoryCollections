﻿namespace StackMemoryCollections.Class
{
    public unsafe class Wrapper<T> : IDisposable where T : unmanaged
    {
        private readonly Struct.StackMemory* _stackMemoryS;
        private Class.StackMemory? _stackMemoryC = null;
        private readonly T* _start;
        private readonly bool _memoryOwner = false;

        public Wrapper()
        {
            if (!TypeHelper.IsPrimitive<T>())
            {
                throw new ArgumentNullException("T must be primitive type");
            }

            _stackMemoryC = new Class.StackMemory((nuint)sizeof(T));
            _start = (T*)_stackMemoryC.Start;
            _memoryOwner = true;
        }

        public Wrapper(
            Struct.StackMemory* stackMemory
            )
        {
            if (stackMemory == null)
            {
                throw new ArgumentNullException(nameof(stackMemory));
            }

            if (!TypeHelper.IsPrimitive<T>())
            {
                throw new ArgumentNullException("T must be primitive type");
            }

            _start = (T*)stackMemory->AllocateMemory((nuint)sizeof(T));
            _stackMemoryS = stackMemory;
        }

        public Wrapper(
            Class.StackMemory stackMemory
            )
        {
            if (stackMemory == null)
            {
                throw new ArgumentNullException(nameof(stackMemory));
            }

            if (!TypeHelper.IsPrimitive<T>())
            {
                throw new ArgumentNullException("T must be primitive type");
            }

            _start = (T*)stackMemory.AllocateMemory((nuint)sizeof(T));
            _stackMemoryC = stackMemory;
            _stackMemoryS = null;
        }

        public Wrapper(
            void* start
            )
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (!TypeHelper.IsPrimitive<T>())
            {
                throw new ArgumentNullException("T must be primitive type");
            }

            _start = (T*)start;
            _stackMemoryC = null;
            _stackMemoryS = null;
        }

        public T* Ptr => _start;

        #region IDisposable

        private bool _disposed;

        ~Wrapper() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if(!_memoryOwner)
                {
                    if (disposing)
                    {
                        if (_stackMemoryC != null)
                        {
                            _stackMemoryC?.FreeMemory((nuint)sizeof(T));
                        }
                        else if (_stackMemoryS != null)
                        {
                            _stackMemoryS->FreeMemory((nuint)sizeof(T));
                        }
                    }
                }
                else
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    _stackMemoryC.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }

                _disposed = true;
            }
        }

        #endregion
    }
}