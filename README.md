<h1 align="center">
  <a>StackMemoryCollections</a>
</h1>

<h3 align="center">

  [![Nuget](https://img.shields.io/nuget/v/StackMemoryCollections?logo=StackMemoryCollections)](https://www.nuget.org/packages/StackMemoryCollections/)
  [![Downloads](https://img.shields.io/nuget/dt/StackMemoryCollections.svg)](https://www.nuget.org/packages/StackMemoryCollections/)
  [![Stars](https://img.shields.io/github/stars/SoftStoneDevelop/StackMemoryCollections?color=brightgreen)](https://github.com/SoftStoneDevelop/StackMemoryCollections/stargazers)
  [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

</h3>

<h3 align="center">
  <a href="https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Readme.md">Documentation</a>
  <a href="https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Benchmarks.md">Benchmarks</a>
</h3>

Fast unsafe collections for memory reuse by stack type. Adding elements without overhead when increasing Capacity. Can also be used in as classic collection with resizing or on a custom memory allocator.

Allows you to allocate memory for a method / class and place all sets of variables in it.
Avoid repeated copying of structures when placing them in collections.
And other use cases.

The generated code uses .Net 6 features. So use only with .Net 6+.

Supported collections:
- Stack
- List
- Queue

Usage:

```C#
//Marking a class/struct with attributes is all that is required of you.
[GenerateStack]
[GenerateWrapper]
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

```

```C#
//Stack of pointers
unsafe
{
    using (var memory = new Struct.StackMemory(JobStructHelper.SizeOf + (nuint)sizeof(IntPtr)))
    {
        using var stack = new Struct.StackOfIntPtr(1, &memory);
        {
            var item = new Struct.JobStructWrapper(&memory);
            item.Int32 = 456;
            *stack.TopFuture() = new IntPtr(item.Ptr);
            stack.PushFuture();
        }
        var item2 = new Struct.JobStructWrapper(stack.Top().ToPointer());
        //item2 point to same memory as is item
    }
}
```

```C#
//Stack of structures
//All alocate memory = JobStructHelper.SizeOf * 100
unsafe
{
    using (var memory = new Struct.StackMemory(JobStructHelper.SizeOf * (nuint)100))//allocate memory
    {
        var item = new Struct.JobStructWrapper(memory.Start, false);
        var js2W = new Struct.JobStruct2Wrapper(memory.Start, false);
        
        {
            using var stack = new Struct.StackOfJobStruct((nuint)100, &memory);//get memory
            for (int i = 0; i < 100; i++)
            {
                item.ChangePtr(stack.TopFuture());
                item.Int32 = i;
                item.Int64 = i * 2;
                js2W.ChangePtr(item.JobStruct2Ptr);
                js2W.Int32 = 777;
                js2W.Int64 = 111;
                stack.PushFuture();
            }
        
            //Do whatever you want with stack
        }//return memory

        var stack2 = new Struct.StackOfJobStruct((nuint)100, &memory);//get memory
        for (int i = 0; i < 100; i++)
        {
            item.ChangePtr(stack2.TopFuture());
            item.Int32 = i;
            item.Int64 = i * 2;
            js2W.ChangePtr(item.JobStruct2Ptr);
            js2W.Int32 = 465;
            js2W.Int64 = 7898721;
            stack2.PushFuture();
        }
    }//free all memory
}

```
