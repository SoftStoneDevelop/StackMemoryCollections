# Definition
Represents a simple last-in-first-out (LIFO) collection of {ItemTypeName}. Where ItemName is you class/struct name.
The collection is auto-generated, for generation the class/struct {ItemTypeName} must be marked with the attribute [GenerateStack].

## For class/struct items:
Class
```C#

namespace {ItemTypeNamespace}.Class
{
    public unsafe class StackOf{ItemTypeName} : IDisposable, System.Collections.Generic.IEnumerable<{ItemType}>
}

```
Implements
`IEnumerable<{ItemType}>, IEnumerable, IDisposable`

Struct
```C#

namespace {ItemTypeNamespace}.Struct
{
    public unsafe struct StackOf{ItemTypeName} : IDisposable
}

```

## For primitive types:
Class
```C#

namespace StackMemoryCollections.Class
{
    public unsafe class Stack<T> : IDisposable, IEnumerable<T> where T : unmanaged
}

```
Implements
`IEnumerable<T>, IEnumerable, IDisposable`

Struct
```C#

namespace StackMemoryCollections.Struct
{
    public unsafe struct Stack<T> : IDisposable where T : unmanaged
}

```


Implements
`IDisposable`

# Constructors

| Name | Description |
| ------------- | ------------- |
| StackOf{ItemTypeName}(nuint, StackMemoryCollections.Struct.StackMemory* stackMemory)  | Creates a new instance of the collection, allocates memory using StackMemory |
| StackOf{ItemTypeName}(nuint, StackMemoryCollections.Class.StackMemory stackMemory)  | Creates a new instance of the collection, allocates memory using StackMemory |
| StackOf{ItemTypeName}(nuint, void* memoryStart)  | Creates a new collection instance at a pointer to memory |

# Properties

| Name | Description |
| ------------- | ------------- |
| Capacity  | Сollection capacity |
| Size  | Current collection size |
| IsEmpty  | Sign of an empty collection |

# Methods


| Name | Description |
| ------------- | ------------- |
| [ReducingCapacity(in nuint)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/ReducingCapacity.md)  | Reducing The Capacity of a collection |
| [ExpandCapacity(in nuint)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/ExpandCapacity.md)  | Expand The Capacity of a collection |
| [TrimExcess()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/TrimExcess.md)  |  |
| [Push(in {ItemTypeName})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/Push.md)  | Inserts an item at the top of The Stack. |
| [TryPush(in {ItemTypeName})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/TryPush.md)  | Inserts an item at the top of The Stack. |
| [Pop()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/Pop.md)  | Removes the item at the top of The Stack. |
| [Clear()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/Clear.md)  | Removes all items from The Stack. |
| [Top()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/Top.md)  | Returns the item at the top of The Stack. |
| [TopPtr()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/TopPtr.md)  | Returns the pointer on item at the top of The Stack. |
| [indexator[nuint]](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/indexator.md)  | Returns the pointer on item by index. |
| [Copy(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/Copy.md)  | Copy elements to memory |
| [Dispose()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/ApiDescriptions/Stack/Dispose.md)  | Free memory |

## Only for the class
| Name | Description |
| ------------- | ------------- |
| IEnumerator<{ItemTypeName}> GetEnumerator()  | Return enumerator |
| IEnumerable GetEnumerator()  | Return enumerator |

# Examples

```C#

[GenerateStack]
public struct JobStruct
{
    public JobStruct(
        int int32,
        long int64
        )
    {
        Int32 = int32;
        Int64 = int64;
    }

    public long Int64;
    public int Int32;
}

unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.GetSize() * (nuint)5))
    {
        var item = new JobStruct(0, 0);
        using var stack = new Struct.StackOfJobStruct((nuint)5, &memory);
        for (int i = 0; i < 3; i++)
        {
            item.Int32 = i;
            item.Int64 = i * 2;
            stack.Push(in item);
        }

        stack.TrimExcess();//No copy just pointer offset
        while (!stack.IsEmpty)
        {
            stack.Pop();
        }
    }
}

```

```C#

unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.GetSize() * (nuint)10))
    {
        var item = new JobStruct(0, 0);
        using var stack = new Struct.StackOfJobStruct((nuint)5, &memory);
        for (int i = 0; i < 10; i++)
        {
            item.Int32 = i;
            item.Int64 = i * 2;
            
            if(!stack.TryPush(in item))
            {
                stack.ExpandCapacity(1);//No copy just pointer offset
                stack.Push(in item);
            }
        }

        while (!stack.IsEmpty)
        {
            stack.Pop();
        }
    }
}

```
