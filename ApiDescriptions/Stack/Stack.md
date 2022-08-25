# Definition
Represents a simple last-in-first-out (LIFO) collection of {ItemName}. Where ItemName is you class/struct name.
The collection is auto-generated, for generation the class/struct {ItemName} must be marked with the attribute [GenerateStack].



Class
```C#

namespace {ItemTypeNamespace}.Class
{
    public unsafe class StackOf{ItemTypeName} : IDisposable, System.Collections.Generic.IEnumerable<{ItemType}>
}

```

Struct
```C#

namespace {ItemTypeNamespace}.Struct
{
    public unsafe struct StackOf{ItemTypeName} : IDisposable
}

```

Implements
IEnumerable<{ItemType}>, IEnumerable, IDisposable

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
| ReducingCapacity(in nuint)  | Reducing The Capacity of a collection |
| ExpandCapacity(in nuint)  | Expand The Capacity of a collection |
| TrimExcess()  |  |
| Push(in {ItemTypeName})  | Inserts an item at the top of The Stack. |
| TryPush(in {ItemTypeName})  | Inserts an item at the top of The Stack. |
| Pop())  | Removes the item at the top of The Stack. |
| Clear())  | Removes all items from The Stack. |
| Top())  | Returns the item at the top of The Stack. |
| TopPtr())  | Returns the pointer on item at the top of The Stack. |
| [nuint])  | Returns the pointer on item by index. |
| copy(in void*)  | Copy elements to memory |
| Dispose()  | Free memory |
| IEnumerator<{ItemTypeName}> GetEnumerator()  | Return enumerator |
| IEnumerable GetEnumerator()  | Return enumerator |

## Examples

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
    using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.GetSize() * (nuint)Size))
    {
        var item = new JobStruct(0, 0);
        using var stack = new Struct.StackOfJobStruct((nuint)Size, &memory);
        for (int i = 0; i < Size; i++)
        {
            item.Int32 = i;
            item.Int64 = i * 2;
            stack.Push(in item);
        }

        while (!stack.IsEmpty)
        {
            stack.Pop();
        }
    }
}

```
