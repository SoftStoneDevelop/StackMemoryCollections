# Start Property

## Definition
Gets a pointer to the beginning of the collection's memory

```C#
public void* Start { get; }
```

## Remarks
For collections on primitive types returns `{ItemType}*` pointer instead of `void*`
