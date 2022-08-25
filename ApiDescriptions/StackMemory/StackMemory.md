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
| FreeMemory  | Free up occupied memory |
| Dispose  | Free all memory |
