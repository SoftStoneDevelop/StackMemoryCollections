# ReducingCapacity(in nuint) Method

## Definition
Free memory from StackMemory if use him.

```C#
public void ReducingCapacity(in nuint reducingCount)
```

## Parameters
reducingCount nuint

Freed memory size in count of items

## Exceptions

```C#
Exception
```
If The number of elements to be deallocated exceeds the number of free slots in the collection
-or-
If The Stack collection is created using StackMemory and the StackMemory has moved further

## Remark
Reduces Capacity by quantity 'reducingCount'
