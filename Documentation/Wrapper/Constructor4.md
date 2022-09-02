# {ItemType}Wrapper(void*, bool) Constructor

## Definition
Creates an instance with memory for the instance starting at `start`.

```C#
public {ItemType}Wrapper(void* start, bool createInstance)
```
## Parametrs
`bool createInstance`

If true - set nullable mark to NotNull and set all IntPtr property to IntPtr.Zero


`void* start`

The memory on which the instance runs

## Exceptions
`ArgumentNullException`
if `start` is null

## Remarks

The correctness of the memory remains under the control of the developer. The instance has no way to check the correctness of the passed pointer.
