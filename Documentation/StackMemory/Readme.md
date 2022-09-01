# Definition
Represents memory management methods as a stack

Class
```C#

namespace StackMemoryCollections.Class
{
    public unsafe class StackMemory : IDisposable
}

```

Struct
```C#

namespace StackMemoryCollections.Struct
{
    public unsafe struct StackMemory : IDisposable
}

```

Implements IDisposable

# Constructors

| Name | ForType |
| ------------- | ------------- |
| [StackMemory()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Constructor1.md)  | Struct |
| [StackMemory(nuint)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Constructor2.md)  | All |

# Properties

| Name | ForType |
| ------------- | ------------- |
| [Start](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Start.md)  | All |
| [Current](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Current.md)  | All |
| [ByteCount](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/ByteCount.md)  | All |
| [FreeByteCount](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/FreeByteCount.md)  | All |

# Methods


| Name | ForType |
| ------------- | ------------- |
| [AllocateMemory(nuint)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/AllocateMemory.md)  | All |
| [TryAllocateMemory(nuint, out void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/TryAllocateMemory.md) | All |
| [FreeMemory()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/FreeMemory.md)  | All |
| [TryFreeMemory()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/TryFreeMemory.md)  | All |
| [Dispose()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Dispose.md)  | All |

## Examples
```C#

unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory(16))//allocate 16 bytes
    {
    }
}

```


```C#

unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory())//throw error "Constructor without parameters is not supported"
    {
    }
}

```

```C#

unsafe
{
    using (var memory = new StackMemoryCollections.Class.StackMemory(16))//allocate 16 bytes
    {
    }
}

```
