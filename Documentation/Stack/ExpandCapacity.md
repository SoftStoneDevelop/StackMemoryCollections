# ExpandCapacity(in nuint) Method

## Definition
Allocate memory from StackMemory if use him.

```C#
public void ExpandCapacity(in nuint expandCount)
```

## Parameters
`expandCount nuint`

Allocate memory size in count of items

## Exceptions

```C#
Exception
```
If The Stack collection is created using StackMemory and the StackMemory has moved further

## Remark
Increases Capacity by quantity 'expandCount'
If the instance was created using a [constructor on a pointer](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Constructor4.md) then: Copies elements to new memory (`Capacity + expandCount`), freeing the current memory.
