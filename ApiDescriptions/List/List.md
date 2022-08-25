# Definition
Represents memory management methods as a stack

# Constructors

| Name | Description |
| ------------- | ------------- |
| [StackMemory(nuint)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/StackMemory/Constructor1.md)  | Creates an instance and allocates memory |

# Properties

| Name | Description |
| ------------- | ------------- |
| Start  | Start of memory |
| Current  | Pointer to free memory |
| ByteCount  | Total memory size |
| FreeByteCount  | Free memory size |

# Methods


| Name | Description |
| ------------- | ------------- |
| [AllocateMemory](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/StackMemory/AllocateMemory.md)  | Allocate free memory |
| [FreeMemory](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/StackMemory/FreeMemory.md)  | Free up occupied memory |
| [Dispose](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/StackMemory/Dispose.md)  | Free all memory |

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
    using (var memory = new StackMemoryCollections.Struct.StackMemory())//throw error "Default constructor not supported"
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
