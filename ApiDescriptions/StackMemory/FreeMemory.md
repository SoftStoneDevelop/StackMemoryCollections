# Definition
Represents memory management methods as a stack

# Constructors

| Name | Description |
| ------------- | ------------- |
| StackMemory(nuint)  | Creates an instance and allocates memory |

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
| AllocateMemory  | Allocate free memory |
| FreeMemory  | Free up occupied memory |
| Dispose  | Free all memory |
