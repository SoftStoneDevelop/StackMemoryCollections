# Dispose() Method

## Definition
Free all memory.

```C#
public void FreeMemory(nuint reducingBytes)
```
## Remark
After calling Dispose, don't try to use the class/struct, it will result in UB
