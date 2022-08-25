# ReducingCapacity(in nuint) Method

## Definition
Allocate memory from StackMemory if use him.

```C#
public void ReducingCapacity(in nuint expandCount)
```

## Parameters
expandCount nuint

Allocate memory size in count of items

## Exceptions

```C#
Exception
```
If The Stack collection is created using StackMemory and the StackMemory has moved further

## Remark
Increases Capacity by quantity 'expandCount'
