# TryPush(in void*) Method

## Definition
Tries to push an element to the top of the stack.

```C#
public bool TryPush(in void* ptr)
```

## Parameters
`in void* ptr`

Pointer to the element to be inserted

## Returns
`bool`

True if element is successfully inserted otherwise False

## Remarks
For collections on primitive types then ptr is `{ItemType}*` pointer instead of `void*`
If the instance was created using a constructor on a memory and no place for the element in the collection then: try allocate memory from `memory`.
