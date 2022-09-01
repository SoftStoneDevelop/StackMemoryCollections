# Current Property

## Definition
Gets pointer to next free location in allocated memory.

```C#
public void* Current { get; private set; }
```

## Remarks
If there is no more free memory, then it points to the next section of memory, which does not belong to the instance.
