# StackMemory(nuint) Constructor

## Definition
Creates an instance and allocates memory

```C#
public StackMemory(nuint byteCount)
```
## Parametrs
byteCount nuint

Size of allocated memory in bytes

## Remarks
The default constructor of a struct will throw an Exception

## Examples
```C#

unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory(16))//allocate 16 bytes
    {
    }
}

```


```C#

unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory())//throw error "Default constructor not supported"
    {
    }
}

```

```C#

unsafe
{
    using (var memory = new StackMemoryCollections.Class.StackMemory(16))//allocate 16 bytes
    {
    }
}

```
