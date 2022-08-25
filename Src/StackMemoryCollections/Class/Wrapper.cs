﻿namespace StackMemoryCollections.Class
{
    public unsafe class Wrapper<T> : IDisposable where T : unmanaged
    {
        private readonly Struct.StackMemory* _stackMemoryS;
        private readonly Class.StackMemory? _stackMemoryC = null;
        private readonly void* _start;

        public Wrapper()
        {
            throw new Exception("Default constructor not supported");
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

            _start = (*stackMemory).AllocateMemory((nuint)sizeof(T));
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

            _start = stackMemory.AllocateMemory((nuint)sizeof(T));
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

            _start = start;
            _stackMemoryC = null;
            _stackMemoryS = null;
        }

        public void* Ptr => _start;

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
                if (disposing)
                {
                    if (_stackMemoryC != null)
                    {
                        _stackMemoryC?.FreeMemory((nuint)sizeof(T));
                    }
                    else if (_stackMemoryS != null)
                    {
                        (*_stackMemoryS).FreeMemory((nuint)sizeof(T));
                    }
                }

                _disposed = true;
            }
        }

        #endregion
    }
}