# Definition
Represents a simple last-in-first-out (LIFO) collection of {ItemName}. Where ItemName is you class/struct name.
The collection is auto-generated, for generation the class {ItemName} must be marked with the attribute [GenerateStack].



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
    public unsafe struct StackOf{ItemTypeName} : IDisposable, System.Collections.Generic.IEnumerable<{ItemType}>
}

```

Implements
IEnumerable<{ItemType}>, IEnumerable, IDisposable

# Constructors

| Name | Description |
| ------------- | ------------- |
| StackOfJobStruct(nuint, StackMemoryCollections.Struct.StackMemory* stackMemory)  | Creates a new instance of the collection, allocates memory using StackMemory |
| StackOfJobStruct(nuint, StackMemoryCollections.Class.StackMemory stackMemory)  | Creates a new instance of the collection, allocates memory using StackMemory |
| StackOfJobStruct(nuint, void* memoryStart)  | Creates a new collection instance at a pointer to memory |

# Properties

| Name | Description |
| ------------- | ------------- |
| Capacity  | Сollection capacity |
| Size  | Current collection size |
| IsEmpty  | Sign of an empty collection |

# Methods


| Name | Description |
| ------------- | ------------- |
| ReducingCapacity  | Reducing the capacity of a collection |
//TODO

## Examples
//TODO
