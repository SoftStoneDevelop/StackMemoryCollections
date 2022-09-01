# StackOf{ItemType}(nuint, Class.StackMemory) Constructor

## Definition
Creates an instance with Capacity equal `count` and allocate collection memory from StackMemory.

```C#
public StackOf{ItemType}(nuint count, Class.StackMemory stackMemory)
```
## Parametrs
`count nuint`

Capacity of Collection


`Class.StackMemory stackMemory`

The memory on which the collection runs

## Exceptions
`ArgumentNullException`
if stackMemory is null
