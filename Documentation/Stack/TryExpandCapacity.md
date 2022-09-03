# TryExpandCapacity(in nuint) Method

## Definition
Try Allocate memory from StackMemory if use him.

```C#
public void ExpandCapacity(in nuint expandCount)
```

## Parameters
`expandCount nuint`

Allocate memory size in count of items

## Return

`True` if successfully expanded the Capacity otherwise `False`.

## Remark
Increases Capacity by quantity 'expandCount'
If the instance was created using a [constructor on a pointer](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Constructor4.md) then: Copies elements to new memory (`Capacity + expandCount`), freeing the current memory.
