# PushFuture() Method

## Definition
Advances the pointer to the next element as if the element had been added.

```C#
public void PushFuture()
```

## Exceptions

```C#
Exception
```
If there is no place for the element in the collection

## Example

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
