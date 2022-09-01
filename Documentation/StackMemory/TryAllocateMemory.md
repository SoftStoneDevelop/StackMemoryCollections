# TryAllocateMemory(nuint) Method

## Definition
Try Allocate free memory.

```C#
public bool TryAllocateMemory(nuint allocateBytes, out void* ptr)
```

## Parameters
`allocateBytes nuint`

Size of allocated memory in bytes

`out void* ptr`
Pointer to start of allocated memory or null if not allocated

## Returns
`bool`

True if allocated is success otherwise False
