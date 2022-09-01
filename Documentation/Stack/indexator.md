# [nuint] Method

## Definition
Return pointer to the element from the stack by index.

```C#
public void* this[nuint index]
```

## Return
`void*`

Pointer to the element from the top of the stack. For collections on primitive types return type is `{ItemType}*` instead of `void*`.

## Exceptions

```C#
Exception
```

If there is no element at that index
