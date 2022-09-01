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

| Name | Description | ForType |
| ------------- | ------------- | ------------- |
| [StackMemory()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Constructor1.md)  | Creates an instance and allocates memory | Struct |
| [StackMemory(nuint)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Constructor2.md)  | Creates an instance and allocates memory | All |

# Properties

| Name | Description | ForType |
| ------------- | ------------- | ------------- |
| [Start](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Start.md)  | Start of memory | All |
| [Current](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Current.md)  | Pointer to free memory | All |
| [ByteCount](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/ByteCount.md)  | Total memory size | All |
| [FreeByteCount](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/FreeByteCount.md)  | Free memory size | All |

# Methods


| Name | Description | ForType |
| ------------- | ------------- | ------------- |
| [AllocateMemory](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/AllocateMemory.md)  | Allocate free memory | All |
| [TryAllocateMemory](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/TryAllocateMemory.md) | Try Allocate free memory | All |
| [FreeMemory](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/FreeMemory.md)  | Free up occupied memory | All |
| [TryFreeMemory](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/TryFreeMemory.md)  | Free up occupied memory | All |
| [Dispose](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/StackMemory/Dispose.md)  | Free all memory | All |

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
