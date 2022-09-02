# ChangePtr(in void*) Method

## Definition
Set pointer to memory on new value. This method only for instance that was created from [constructor on pointer](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Constructor4.md)

```C#
public ChangePtr(in void* newPtr)
```

## Parameters
`in void*` newPtr

A new pointer to the beginning of the wrapper memory.

## Exceptions
`Exception`

if instance created not from [constructor on pointer](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Constructor4.md)
