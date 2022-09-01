# Push(in void*) Method

## Definition
Inserts an element at the top of the Stack.

```C#
public void Push(in void* ptr)
```

## Parameters
`in void* ptr`

Pointer to the element to be inserted

## Exceptions

```C#
Exception
```
If there is no place for the element in the collection

## Remarks
For collections on primitive types then ptr is `{ItemType}*` pointer instead of `void*`
If the instance was created using a constructor on a memory and no place for the element in the collection then: allocate memory from `memory`.
