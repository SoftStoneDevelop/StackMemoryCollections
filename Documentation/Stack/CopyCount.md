# Copy(in void*, in nuint) Method

## Definition
Copies a count of elements in the collection into memory, starting from the bottom of the stack.

```C#
public void Copy(in void* ptrDest, in nuint count)
```

## Parameters
`in void* ptrDest`

Pointer to the memory where the elements will be copied

`in nuint count`

Count of elements to copy


## Exceptions

```C#
Exception
```
If the collection does not have that many elements
