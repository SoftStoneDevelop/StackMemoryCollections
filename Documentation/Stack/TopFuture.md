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

```C#

unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 10))
    {
        using var stack = new StackMemoryCollections.Struct.StackOfInt32(10, &memory);
        for (int i = 0; i < 10; i++)
        {
            int* futureTop = stack.TopFuture();
            *futureTop = i;
            stack.PushFuture();
        }
    }
}

```
