# Readme:
Fast unsafe collections(generated at build time with Roslyn Generator) for memory reuse by stack type. Can also be used in as classic collection with resizing or on a custom memory allocator(constructor with `void*`), then it's your responsibility to make sure the pointer is correct.

Allows you to allocate memory for a method / class and place all sets of variables in it.
Avoid repeated copying of structures when placing them in collections.
And other use cases.

See [Documentation](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Readme.md) for details.

Supported collections:
- Stack
- List [TODO](https://github.com/SoftStoneDevelop/StackMemoryCollections/issues/1)
- Queue [TODO](https://github.com/SoftStoneDevelop/StackMemoryCollections/issues/2)

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
|                     Method |    Size |            Mean | Ratio |  Allocated |
|--------------------------- |-------- |----------------:|------:|-----------:|
|     **StackMemoryCollections** |     **100** |        **291.6 ns** |  **0.30** |       **1232 B** |
| System.Collections.Generic |     100 |        965.2 ns |  1.00 |     4056 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |    **1000** |      **2,223.5 ns** |  **0.24** |       **12032 B** |
| System.Collections.Generic |    1000 |      9,333.2 ns |  1.00 |    40056 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |   **10000** |     **21,648.9 ns** |  **0.23** |       **120032 B** |
| System.Collections.Generic |   10000 |     94,883.7 ns |  1.00 |   400056 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **100000** |    **607,289.0 ns** |  **0.33** |       **1200032 B** |
| System.Collections.Generic |  100000 |  1,877,933.5 ns |  1.00 |  4000158 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **250000** |  **1,003,259.6 ns** |  **0.10** |       **3000033 B** |
| System.Collections.Generic |  250000 | 10,510,118.3 ns |  1.00 | 10000279 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **500000** |  **2,834,058.1 ns** |  **0.12** |       **6000034 B** |
| System.Collections.Generic |  500000 | 26,277,167.7 ns |  1.00 | 20000648 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** | **1000000** |  **5,659,929.6 ns** |  **0.12** |       **12000036 B** |
| System.Collections.Generic | 1000000 | 53,257,801.4 ns |  1.00 | 40000425 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/ClassSimpleJob.cs)
______

|                     Method |    Size |            Mean | Ratio |    Allocated |
|--------------------------- |-------- |----------------:|------:|-------------:|
|     **StackMemoryCollections** |     **100** |        **35.49 μs** |  **0.23** |         **1232 B** |
| System.Collections.Generic |     100 |       155.84 μs |  1.00 |     811200 B |
|                            |         |                 |       |              |
|     **StackMemoryCollections** |    **1000** |       **332.97 μs** |  **0.22** |         **12032 B** |
| System.Collections.Generic |    1000 |     1,521.58 μs |  1.00 |    8011201 B |
|                            |         |                 |       |              |
|     **StackMemoryCollections** |   **10000** |     **3,348.83 μs** |  **0.23** |         **120034 B** |
| System.Collections.Generic |   10000 |    14,740.49 μs |  1.00 |   80011215 B |
|                            |         |                 |       |              |
|     **StackMemoryCollections** |  **100000** |    **37,512.58 μs** |  **0.11** |         **1200066 B** |
| System.Collections.Generic |  100000 |   340,913.42 μs |  1.00 |  800044536 B |
|                            |         |                 |       |              |
|     **StackMemoryCollections** |  **250000** |    **91,972.42 μs** |  **0.04** |        **3000112 B** |
| System.Collections.Generic |  250000 | 2,222,325.18 μs |  1.00 | 2000068912 B |
|                            |         |                 |       |              |
|     **StackMemoryCollections** |  **500000** |   **180,948.87 μs** |  **0.03** |        **6000192 B** |
| System.Collections.Generic |  500000 | 5,608,830.92 μs |  1.00 | 4000092016 B |
|                            |         |                 |       |              |
|     **StackMemoryCollections** | **1000000** |   **335,629.28 μs** |  **0.04** |       **12002256 B** |
| System.Collections.Generic | 1000000 | 9,305,081.77 μs |  1.00 | 8000081312 B |

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
