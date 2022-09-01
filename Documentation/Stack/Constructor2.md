# StackOf{ItemType}(nuint, Struct.StackMemory*) Constructor

## Definition
Creates an instance with Capacity equal `count` and allocate collection memory from StackMemory.

```C#
public StackOf{ItemType}(nuint count, Struct.StackMemory* stackMemory)
```
## Parametrs
`count nuint`

Capacity of Collection


`Struct.StackMemory* stackMemory`

The memory on which the collection runs

## Exceptions
`ArgumentNullException`
if stackMemory is null
