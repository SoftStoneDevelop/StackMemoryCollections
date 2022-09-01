# Definition
Represents a wrapper over memory that allows access to data in it.

## For class/struct items:
Class
```C#

namespace {ItemTypeNamespace}.Class
{
    public unsafe class {ItemType}Wrapper : IDisposable
}

```
Implements
`IDisposable`

Struct
```C#

namespace {ItemTypeNamespace}.Struct
{
    public unsafe struct {ItemType}Wrapper : IDisposable
}

```
Implements
`IDisposable`

## For primitive types:
`Int32`, `Int64`, `UInt32`... and `IntPtr`.
Class
```C#

namespace StackMemoryCollections.Class
{
    public unsafe class {PrimitiveTypeName}Wrapper : IDisposable
}

```
Implements
`IDisposable`

Struct
```C#

namespace StackMemoryCollections.Struct
{
    public unsafe struct {PrimitiveTypeName}Wrapper : IDisposable
}

```
Implements
`IDisposable`

# Constructors

| Name | ForType |
| ------------- | ------------- |
| [{ItemType}Wrapper()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Constructor1.md)  | All |
| [{ItemType}Wrapper(Struct.StackMemory*))](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Constructor2.md)  | All |
| [{ItemType}Wrapper(Class.StackMemory)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Constructor3.md)  | All |
| [{ItemType}Wrapper(void*, bool)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Constructor4.md)  | All |

# Properties

| Name | ForType |
| ------------- | ------------- |
| [Capacity](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Capacity.md)  | All |
| [Size](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Size.md)  | All |
| [Start](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Start.md) | All |
| [IsEmpty](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/IsEmpty.md)  | All |

# Methods

| Name | ForType |
| ------------- | ------------- |
| [ReducingCapacity(in nuint)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/ReducingCapacity.md)  | All |
| [ExpandCapacity(in nuint)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/ExpandCapacity.md)  | All |
| [TrimExcess()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/TrimExcess.md)  | All |
| [Push(in {ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Push.md)  | All |
| [Push(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/PushPtr.md)  | All |
| [TryPush(in {ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/TryPush.md)  | All |
| [TryPush(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/TryPushPtr.md)  | All |
| [Pop()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Pop.md)  | All |
| [TryPop()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/TryPop.md)  | All |
| [Clear()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Clear.md)  | All |
| [Top()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Top.md)  | All |
| [Top(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/TopInPtr.md)  | All |
| [Top(ref {ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/TopRef.md)  | All |
| [TopPtr()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/TopPtr.md)  | All |
| [TopOut(out {ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/TopOut.md)  | All |
| [indexator[nuint]](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/indexator.md)  | All |
| [Copy(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Copy.md)  | All |
| [Copy(in void*, nuint)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/CopyCount.md)  | All |
| [Copy(in Class.StackOf{ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/CopyToStack.md)  | All |
| [IEnumerator<{ItemType}> GetEnumerator()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/GetEnumeratorItemType.md)  | Class |
| [IEnumerable GetEnumerator()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/GetEnumerator.md)  | Class |
| [Dispose()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Dispose.md)  | All |

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
    using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.SizeOf * (nuint)5))
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
        while (stack.TryPop())
        {
        }
    }
}

```

```C#

unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.SizeOf * (nuint)10))
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

        while (stack.TryPop())
        {
        }
    }
}

```
