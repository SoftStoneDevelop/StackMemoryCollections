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
| [{ItemType}Wrapper(Struct.StackMemory*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Constructor2.md)  | All |
| [{ItemType}Wrapper(Class.StackMemory)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Constructor3.md)  | All |
| [{ItemType}Wrapper(void*, bool)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Stack/Constructor4.md)  | All |

# Properties

| Name | ForType | ForPropertyType |
| ------------- | ------------- |------------- |
| [Ptr](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/MemoryPtr.md)  | All | For Instance |
| [{PropertyName}Ptr { get; }](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/PropertyPtr.md)  | All | All |
| [{PropertyName} { get; set; }](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/PropertyGetSet.md)  | All | All |
| [{PropertyName}ValuiInPtr { get; }](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/PropertyValueInPtr.md)  | All | Class as Pointer |
| [IsNull](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/IsNull.md)  | All | Class |

# Methods

| Name | ForType | ForPropertyType |
| ------------- | ------------- |------------- |
| [GetValue()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/GetValue.md)  | All | For Instance |
| [Fill(in {ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Fill.md)  | All | For Instance |
| [Set{PropertyName}(in {PropertyType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/SetIn.md)  | All | All |
| [Set{PropertyName}(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/SetInPtr.md)  | All | All |
| [GetOut{PropertyName}(out {PropertyType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/GetOut.md)  | All | All |
| [ChangePtr(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/ChangePtr.md)  | All | For Instance |
| [CreateInstance()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/CreateInstance.md)  | All | For Instance |
| [Dispose()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/Dispose.md)  | All | For Instance |

# Examples

```C#

[GenerateHelper]
[GenerateWrapper]
public struct HelpStruct
{
    public HelpStruct(
        int int32,
        long int64,
        HelpClass helpClass,
        HelpClass helpClass2
        )
    {
        Int32 = int32;
        Int64 = int64;
        HelpClass = helpClass;
        HelpClass2 = helpClass2;
    }

    public long Int64;
    public int Int32;

    public HelpClass HelpClass;

    [AsPointer]
    public HelpClass HelpClass2 { get; set; }
}

[GenerateHelper]
[GenerateWrapper]
public struct HelpStruct2
{
    public HelpStruct2(
        int int32,
        long int64
        )
    {
        Int32 = int32;
        Int64 = int64;
    }

    public long Int64 { get; set; }
    public int Int32;
}

[GenerateHelper]
[GenerateWrapper]
public class HelpClass
{
    public HelpClass()
    {
    }

    public HelpClass(
        int int32,
        long int64,
        HelpStruct2 helpStruct2
        )
    {
        Int32 = int32;
        Int64 = int64;
        HelpStruct2 = helpStruct2;
    }

    public long Int64;

    [AsPointer]
    public HelpClass HelpClass2 { get; set; }

    public int Int32 { get; set; }

    public HelpStruct2 HelpStruct2 { get; set; }

    [GeneratorIgnore]
    public Dictionary<int, string> Dictionary { get; set; }
}

unsafe
{
    var wrap = new Struct.HelpStructWrapper();
    var helpStructValueIn = new HelpStruct()
    {
        Int32 = 45,
        Int64 = 4564564564,
        HelpClass = new HelpClass()
        {
            Int32 = 12,
            Int64 = 321123,
            HelpStruct2 = new HelpStruct2(321, 98746512),
            HelpClass2 = new HelpClass()
            {
                Int32 = 238,
                Int64 = 40,
                HelpStruct2 = new HelpStruct2(1, 2)
            }
        }
    };
    wrap.Fill(in helpStructValueIn);
    IntPtr helpClass2Out;
    wrap.GetOutHelpClass2(out helpClass2Out);
    
    var wrapClass = new Struct.HelpClassWrapper();
    wrapClass.Fill(new HelpClass(44, 235, new HelpStruct2(140, 78)));
    var wrapClass2 = new Struct.HelpClassWrapper(wrap.HelpClassPtr, false);
    wrapClass2.HelpClass2 = new IntPtr(wrapClass.Ptr);
}

```
