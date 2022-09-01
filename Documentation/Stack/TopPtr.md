# TopPtr() Method

## Definition
Return pointer to the element from the top of the stack.

```C#
public void* TopPtr()
```

## Return
`void*`

Pointer to the element from the top of the stack. For collections on primitive types return type is `{ItemType}*` instead of `void*`.

## Exceptions

```C#
Exception
```

If there are no elements on the stack
