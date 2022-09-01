# TopFuture() Method

## Definition
Return pointer to future the element from the top of the stack.

```C#
public void* TopFuture()
```

## Return
`void*`

Pointer to future the element from the top of the stack. For collections on primitive types return type is `{ItemType}*` instead of `void*`.

## Exceptions

```C#
Exception
```

If Future element not available
