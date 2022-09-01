# FreeMemory(nuint) Method

## Definition
Frees up occupied memory.

```C#
public void FreeMemory(nuint reducingBytes)
```

## Parameters
`reducingBytes nuint`

Freed memory size in bytes

## Returns
`void`

## Exceptions

```C#
ObjectDisposedException
```


```C#
Exception
```
If all memory is already free
