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

General idea (on the example of a list):

Allocate memory for all your collections.

![1](https://user-images.githubusercontent.com/43916814/188752538-cff787a0-2c92-4d86-8439-6c9efec3eb57.png)

In our example, we will allocate a list of 5 elements on this memory.

![2](https://user-images.githubusercontent.com/43916814/188752689-bbc509e0-05be-4ea2-847f-5ba04ca5b066.png)

If we need to increase the capacity, and at the same time, the collection whose capacity increases is the last element in memory, then to increase the capacity, you just need to indicate that there is more available memory for the collection. No copying or reallocation.

![3](https://user-images.githubusercontent.com/43916814/188752910-11f87ccc-2384-4a9a-909c-91d85c2e67fa.png)

If we need to allocate a collection of elements of a different type on the same memory (and we do not need the old collection), then we do not have to allocate new memory, we can allocate it on the already allocated one.

If something else is written to memory after the collection, then the collection becomes sealed.
In the future, you can compress memory if there are areas that are no longer used, thereby not sealing the collection.
This can be useful for allocating memory for an entire method if we know approximately how much memory it can consume at the maximum.

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
