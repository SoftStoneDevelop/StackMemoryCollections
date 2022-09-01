# StackOf{ItemType}(nuint, void*) Constructor

## Definition
Creates an instance with a capacity of `count`, with memory for the collection starting at memoryStart.

```C#
public StackOf{ItemType}(nuint count, void* memoryStart)
```
## Parametrs
`count nuint`

Capacity of Collection


`void* memoryStart`

The memory on which the collection runs

## Exceptions
`ArgumentNullException`
if memoryStart is null

## Remarks

The correctness of the memory remains under the control of the developer. The collection has no way to check the correctness of the passed pointer.
