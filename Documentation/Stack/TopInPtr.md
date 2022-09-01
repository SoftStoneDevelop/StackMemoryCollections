# Top(in void*) Method

## Definition
Copy the element from the top of the stack into memory

```C#
public void Top(in void* ptr)
```

## Parameters
`in void* ptr`

Pointer to the memory into which the element will be copied

## Exceptions

```C#
Exception
```

If there are no elements on the stack

## Remarks
For collections on primitive types then ptr is `{ItemType}*` pointer instead of `void*`
