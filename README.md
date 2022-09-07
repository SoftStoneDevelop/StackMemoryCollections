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
unsafe
{
    using (var memory = new Struct.StackMemory(sizeof(int) * 100))//Allocate memory for all your collections 400 byte.
    {   
        {
            using var listOfInt32 = new Struct.ListOfInt32(50, &memory);//50 * 4 = 200 byte
            list.ExpandCapacity(50);// + 200 byte
            //Do whatever you want with list of Int32 items
        }//return memory

        var listOfInt64 = new Struct.ListOfInt64(50, &memory);//get memory 400 byte
        //Do whatever you want with list of Int64 items
    }//free all memory
}

```

In our example, we will allocate a list of 50 elements on this memory.
Then we increase the capacity to the 100 elements. No copying or reallocation.
Then we free old collection and allocate new collection of Int64 on the same memory.

In the future(TODO), you can compress memory if there are areas that are no longer used, thereby not sealing the collection.
This can be useful for allocating memory for an entire method if we know approximately how much memory it can consume at the maximum.

_____
Stack of composite type example:

```C#
//Marking a class/struct with attributes is all that is required of you.
[GenerateStack]
[GenerateWrapper]
public struct SimpleStruct
{
    public SimpleStruct(
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

[GenerateStack]
[GenerateWrapper]
public class SimpleClass
{
    public SimpleClass(
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
    using (var memory = new Struct.StackMemory(SimpleStructHelper.SizeOf + (nuint)sizeof(IntPtr)))
    {
        using var stack = new Struct.StackOfIntPtr(1, &memory);
        {
            var item = new Struct.SimpleStructWrapper(&memory);
            item.Int32 = 456;
            *stack.TopFuture() = new IntPtr(item.Ptr);
            stack.PushFuture();
        }
        var item2 = new Struct.SimpleStructWrapper(stack.Top().ToPointer());
        //item2 point to same memory as is item
    }
}
```

```C#
//All alocate memory = SimpleStructHelper.SizeOf * 100 = 12* 100 = 1200 byte
unsafe
{
    using (var memory = new Struct.StackMemory(JobStructHelper.SizeOf * (nuint)100))//allocate memory
    {       
        {
            var item = new Struct.SimpleStructWrapper(memory.Start, false);
            using var stackOfSimpleStruct = new Struct.StackOfSimpleStruct((nuint)100, &memory);//get memory
            for (int i = 0; i < 100; i++)
            {
                item.ChangePtr(stackOfSimpleStruct.TopFuture());
                item.Int32 = i;
                item.Int64 = i * 2;
                stackOfSimpleStruct.PushFuture();
            }
        
            //Do whatever you want with stack
        }//return memory

        var item = new Struct.SimpleClassWrapper(memory.Start, false);
        var stackOfSimpleClass = new Struct.StackOfSimpleClass((nuint)100, &memory);//get memory
        for (int i = 0; i < 100; i++)
        {
            item.ChangePtr(stackOfSimpleClass.TopFuture());
            item.Int32 = i;
            item.Int64 = i * 2;
            stackOfSimpleClass.PushFuture();
        }
    }//free all memory
}

```
