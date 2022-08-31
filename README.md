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
  <a href="https://github.com/SoftStoneDevelop/StackMemoryCollections/wiki">Documentation</a>
  <a href="https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Benchmarks.md">Benchmarks</a>
</h3>

Fast unsafe collections for memory reuse by stack type. Adding elements without overhead when increasing Capacity. Can also be used in as classic collection with resizing or on a custom memory allocator.

Allows you to allocate memory for a method / class and place all sets of variables in it.
Avoid repeated copying of structures when placing them in collections.
And other use cases.

The generated code uses .Net 5 features. So use only with .Net 5+.

Supported collections:
- Stack
- List [TODO](https://github.com/SoftStoneDevelop/StackMemoryCollections/issues/1)
- Queue [TODO](https://github.com/SoftStoneDevelop/StackMemoryCollections/issues/2)

Usage:

```C#
//Marking a class/struct with attributes is all that is required of you.
[GenerateHelper]
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
        JobStruct2 = default;
    }

    public long Int64;
    public int Int32;
    public JobStruct2 JobStruct2;
}

```

```C#
//Stack of pointers
unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.SizeOf + (nuint)sizeof(IntPtr)))
    {
        using var stack = new StackMemoryCollections.Struct.StackOfIntPtr(1, &memory);
        {
            var item = new Struct.JobStructWrapper(&memory);
            item.Int32 = 456;
            stack.Push(new IntPtr(item.Ptr));
        }
        var item2 = new Struct.JobStructWrapper(stack.Top().ToPointer());
        //item2 point to same memory as is item
    }
}
```

```C#
//Stack of structures
//All alocate memory = JobStructHelper.SizeOf * (nuint)100)
unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.SizeOf * (nuint)100))//allocate memory
    {
        var item = new JobStruct(0, 0);
        
        {
            using var stack = new Benchmark.Struct.StackOfJobStruct((nuint)Size, &memory);//get memory
            for (int i = 0; i < Size; i++)
            {
                item.Int32 = i;
                item.Int64 = i * 2;
                item.JobStruct2.Int32 = 15;
                item.JobStruct2.Int64 = 36;
                stack.Push(in item);
            }
        
            //Do whatever you want with stack
        }//return memory

        var stack2 = new Benchmark.Struct.StackOfJobStruct((nuint)Size, &memory);//get memory
        for (int i = 0; i < 100; i++)
        {
            item.Int32 = i;
            item.Int64 = i * 2;
            item.JobStruct2.Int32 = 15;
            item.JobStruct2.Int64 = 36;
            stack2.Push(in item);
        }
    }//free all memory
}

```
