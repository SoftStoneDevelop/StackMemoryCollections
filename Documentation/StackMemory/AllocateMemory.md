# AllocateMemory(nuint) Method

## Definition
Allocate free memory.

```C#
public void* AllocateMemory(nuint allocateBytes)
```

## Parameters
`allocateBytes nuint`

Size of allocated memory in bytes

## Returns
`void*`

Pointer to start of allocated memory

# Exceptions

```C#
ObjectDisposedException
```

```C#
ArgumentException
```
If more memory requested than can be allocated
