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

| Name | ForType | ForPropertyType |
| ------------- | ------------- |
| [Ptr](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/MemoryPtr.md)  | All | Once on instance |
| [{PropertyName}Ptr { get; }](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/PropertyPtr.md)  | All | All |
| [{PropertyName} { get; set; }](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/PropertyGetSet.md)  | All | All |
| [{PropertyName}ValuiInPtr { get; }](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/PropertyValueInPtr.md)  | All | Class as Pointer |

# Methods

| Name | ForType | ForPropertyType |
| ------------- | ------------- |
| [GetValue()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/GetValue.md)  | All | For Instance |
| [Fill(in {ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Fill.md)  | All | For Instance |
| [Set{PropertyName}(in {PropertyType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/SetIn.md)  | All | All |
| [Set{PropertyName}(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/SetInPtr.md)  | All | All |
| [GetOut{PropertyName}(out {PropertyType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/GetOut.md)  | All | All |
| [Dispose()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Dispose.md)  | All | All |

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
