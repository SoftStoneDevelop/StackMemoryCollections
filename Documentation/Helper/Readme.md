# Definition
Contains helper methods for getting the size of a type, setting or getting ItemType members.

```C#

namespace {ItemTypeNamespace}
{
    public unsafe static class {ItemType}Helper
}

```

# Fields

| Name |
| ------------- |
| [SizeOf](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/SizeOf.md)  |
| [{MemberName}Offset](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/Offset.md)  |

# Methods

| Name | ForMemberType |
| ------------- | ------------- |
| [IsNullable()](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Wrapper/GetValue.md)  | For ItemType |
| [Get{MemberName}Ptr(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/GetPtr.md)  | All |
| [Get{MemberName}Value(in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/GetValue.md)  | All |
| [GetRef{MemberName}Value(in void*, ref {MemberType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/GetRef.md)  | Primitive |
| [GetOut{MemberName}Value(in void*, out {MemberType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/GetOut.md)  | Primitive |
| [Set{MemberName}Value(in void*, in {MemberType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/SetInValue.md)  | All |
| [Set{MemberName}Value(in void*, in {ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/SetValueFromRoot.md)  | Class or Struct |
| [Set{MemberName}Value(in void*, in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/SetInPtr.md)  | Primitive |
| [CopyToPtr(in ItemType, in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/CopyToPtr.md)  | For ItemType |
| [CopyToValue(in void*, ref {ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/CopyToValue.md)  | For ItemType |
| [CopyToValueOut(in void*, out {ItemType})](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/CopyToValueOut.md)  | For ItemType |
| [Copy(in void*, in void*)](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Helper/CopyToValueOut.md)  | For ItemType |
