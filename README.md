[![Nuget](https://img.shields.io/nuget/v/StackMemoryCollections?logo=StackMemoryCollections)](https://www.nuget.org/packages/StackMemoryCollections/)

# Readme:
Fast unsafe collections for memory reuse by stack type. Adding elements without overhead when increasing Capacity. Can also be used in as classic collection with resizing or on a custom memory allocator.

Allows you to allocate memory for a method / class and place all sets of variables in it.
Avoid repeated copying of structures when placing them in collections.
And other use cases.

See [Documentation](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Readme.md) for details.

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
    using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.GetSize() + (nuint)sizeof(IntPtr)))
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
//All alocate memory = JobStructHelper.GetSize() * (nuint)100)
unsafe
{
    using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.GetSize() * (nuint)100))//allocate memory
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

## Benchmarks:

<details><summary>Stack</summary>

### Primitive types:
Stack elements are primitives: `byte`, `float`, `int`, `short`, `decimal`... .
  
|                     Method |    Size |           Mean | Ratio | Allocated |
|--------------------------- |-------- |---------------:|------:|----------:|
|     **StackMemoryCollections** |     **100** |       **369.2 ns** |  **1.05** |         **400 B** |
| System.Collections.Generic |     100 |       350.0 ns |  1.00 |     456 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |    **1000** |     **2,777.0 ns** |  **0.81** |         **4000 B** |
| System.Collections.Generic |    1000 |     3,408.0 ns |  1.00 |    4056 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |   **10000** |    **26,643.4 ns** |  **0.77** |         **40000 B** |
| System.Collections.Generic |   10000 |    34,708.5 ns |  1.00 |   40056 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **100000** |   **279,317.0 ns** |  **0.60** |         **400000 B** |
| System.Collections.Generic |  100000 |   467,766.8 ns |  1.00 |  400098 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **250000** |   **705,213.9 ns** |  **0.60** |       **1000001 B** |
| System.Collections.Generic |  250000 | 1,171,231.2 ns |  1.00 | 1000140 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **500000** | **1,670,376.9 ns** |  **0.71** |       **2000002 B** |
| System.Collections.Generic |  500000 | 2,341,880.7 ns |  1.00 | 2000225 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** | **1000000** | **3,326,526.8 ns** |  **0.71** |       **4000006 B** |
| System.Collections.Generic | 1000000 | 4,683,416.4 ns |  1.00 | 4000393 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/PrimitiveSimpleJob.cs)
______
  
|                     Method |    Size |          Mean | Ratio |   Allocated |
|--------------------------- |-------- |--------------:|------:|------------:|
|     **StackMemoryCollections** |     **100** |      **56.60 μs** |  **1.04** |           **400 B** |
| System.Collections.Generic |     100 |      54.59 μs |  1.00 |     91200 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |    **1000** |     **488.73 μs** |  **0.94** |         **4001 B** |
| System.Collections.Generic |    1000 |     519.02 μs |  1.00 |    811200 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |   **10000** |   **4,809.66 μs** |  **0.90** |         **40007 B** |
| System.Collections.Generic |   10000 |   5,368.13 μs |  1.00 |   8011204 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **100000** |  **48,078.07 μs** |  **0.64** |       **400136 B** |
| System.Collections.Generic |  100000 |  74,574.55 μs |  1.00 |  80019867 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **250000** | **120,246.62 μs** |  **0.65** |       **1000872 B** |
| System.Collections.Generic |  250000 | 186,345.43 μs |  1.00 | 200029085 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **500000** | **240,641.79 μs** |  **0.66** |       **2000235 B** |
| System.Collections.Generic |  500000 | 367,274.71 μs |  1.00 | 400048032 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** | **1000000** | **481,340.25 μs** |  **0.64** |      **4003792 B** |
| System.Collections.Generic | 1000000 | 750,769.20 μs |  1.00 | 800080288 B |
  
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/PrimitiveOptimalJob.cs)
______
### Class:
Stack elements are classes.
|                     Method |    Size |             Mean | Ratio |  Allocated |
|--------------------------- |-------- |-----------------:|------:|-----------:|
|     **StackMemoryCollections** |     **100** |         **757.4 ns** |  **0.55** |          **2424 B** |
| System.Collections.Generic |     100 |       1,378.3 ns |  1.00 |     8056 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |    **1000** |       **7,004.2 ns** |  **0.51** |          **24024 B** |
| System.Collections.Generic |    1000 |      13,713.2 ns |  1.00 |    80056 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |   **10000** |      **84,682.6 ns** |  **0.58** |          **240024 B** |
| System.Collections.Generic |   10000 |     146,452.7 ns |  1.00 |   800056 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |  **100000** |   **1,436,862.3 ns** |  **0.34** |        **2400025 B** |
| System.Collections.Generic |  100000 |   4,409,689.9 ns |  1.00 |  8000222 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |  **250000** |   **3,647,072.0 ns** |  **0.18** |        **6000028 B** |
| System.Collections.Generic |  250000 |  20,485,464.3 ns |  1.00 | 20000287 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |  **500000** |   **6,939,238.0 ns** |  **0.14** |        **12000028 B** |
| System.Collections.Generic |  500000 |  50,628,540.0 ns |  1.00 | 40000420 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** | **1000000** |  **13,830,743.4 ns** |  **0.11** |        **24000032 B** |
| System.Collections.Generic | 1000000 | 129,356,360.0 ns |  1.00 | 80000790 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/ClassSimpleJob.cs)
______

|                     Method |    Size |            Mean | Ratio |     Allocated |
|--------------------------- |-------- |----------------:|------:|--------------:|
|     **StackMemoryCollections** |     **100** |        **137.7 μs** |  **0.56** |             **2424 B** |
| System.Collections.Generic |     100 |        246.0 μs |  1.00 |     1611200 B |
|                            |         |                 |       |               |
|     **StackMemoryCollections** |    **1000** |      **1,338.0 μs** |  **0.57** |           **24025 B** |
| System.Collections.Generic |    1000 |      2,356.5 μs |  1.00 |    16011202 B |
|                            |         |                 |       |               |
|     **StackMemoryCollections** |   **10000** |     **14,030.9 μs** |  **0.55** |           **240032 B** |
| System.Collections.Generic |   10000 |     25,265.6 μs |  1.00 |   160011220 B |
|                            |         |                 |       |               |
|     **StackMemoryCollections** |  **100000** |    **135,796.6 μs** |  **0.18** |         **2400358 B** |
| System.Collections.Generic |  100000 |    708,012.4 μs |  1.00 |  1600044272 B |
|                            |         |                 |       |               |
|     **StackMemoryCollections** |  **250000** |    **336,080.1 μs** |  **0.09** |        **6001112 B** |
| System.Collections.Generic |  250000 |  3,987,479.2 μs |  1.00 |  4000059496 B |
|                            |         |                 |       |               |
|     **StackMemoryCollections** |  **500000** |    **688,559.2 μs** |  **0.06** |         **12000504 B** |
| System.Collections.Generic |  500000 | 11,788,582.6 μs |  1.00 |  8000116584 B |
|                            |         |                 |       |               |
|     **StackMemoryCollections** | **1000000** |  **2,288,730.2 μs** |  **0.07** |         **24000504 B** |
| System.Collections.Generic | 1000000 | 32,844,409.8 μs |  1.00 | 16000122016 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/ClassOptimalJob.cs)
______
### Struct:
Stack elements are structures.

|                     Method |    Size |            Mean | Ratio |  Allocated |
|--------------------------- |-------- |----------------:|------:|-----------:|
|     **StackMemoryCollections** |     **100** |        **387.6 ns** |  **0.33** |          **2400 B** |
| System.Collections.Generic |     100 |      1,183.7 ns |  1.00 |     3256 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |    **1000** |      **3,246.9 ns** |  **0.28** |          **24000 B** |
| System.Collections.Generic |    1000 |     11,496.5 ns |  1.00 |    32056 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |   **10000** |     **41,866.3 ns** |  **0.21** |          **240000 B** |
| System.Collections.Generic |   10000 |    197,441.8 ns |  1.00 |   320090 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **100000** |    **895,636.1 ns** |  **0.51** |          **2400000 B** |
| System.Collections.Generic |  100000 |  1,973,598.3 ns |  1.00 |  3200392 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **250000** |  **1,719,951.9 ns** |  **0.42** |        **6000001 B** |
| System.Collections.Generic |  250000 |  4,104,451.0 ns |  1.00 |  8000150 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **500000** |  **4,265,441.4 ns** |  **0.55** |        **12000002 B** |
| System.Collections.Generic |  500000 |  7,727,996.1 ns |  1.00 | 16000156 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** | **1000000** |  **8,731,516.6 ns** |  **0.45** |        **24000004 B** |
| System.Collections.Generic | 1000000 | 20,522,355.3 ns |  1.00 | 32000396 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/StructSimpleJob.cs)
______

|                     Method |    Size |            Mean | Ratio |    Allocated |
|--------------------------- |-------- |----------------:|------:|-------------:|
|           **StackOfJobStruct** |     **100** |        **54.05 μs** |  **0.25** |            **2400 B** |
| System.Collections.Generic |     100 |       219.89 μs |  1.00 |     651200 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |    **1000** |       **520.60 μs** |  **0.24** |            **24000 B** |
| System.Collections.Generic |    1000 |     2,139.08 μs |  1.00 |    6411202 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |   **10000** |     **5,341.41 μs** |  **0.15** |          **240004 B** |
| System.Collections.Generic |   10000 |    36,358.46 μs |  1.00 |   64017930 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |  **100000** |    **53,834.98 μs** |  **0.15** |        **2400107 B** |
| System.Collections.Generic |  100000 |   357,425.35 μs |  1.00 |  640081632 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |  **250000** |   **134,170.07 μs** |  **0.17** |        **6000888 B** |
| System.Collections.Generic |  250000 |   768,718.03 μs |  1.00 | 1600028840 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |  **500000** |   **271,483.13 μs** |  **0.19** |        **12000240 B** |
| System.Collections.Generic |  500000 | 1,432,699.91 μs |  1.00 | 3200030400 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** | **1000000** |   **549,009.97 μs** |  **0.14** |       **24003568 B** |
| System.Collections.Generic | 1000000 | 3,855,068.21 μs |  1.00 | 6400079184 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/StructOptimalJob.cs)

______

</details>
