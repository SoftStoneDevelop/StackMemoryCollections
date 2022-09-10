# Stack

### Primitive types:
Stack elements are primitives: `byte`, `float`, `int`, `short`, `decimal`... .
  
|                     Method |    Size |           Mean | Ratio | Allocated |
|--------------------------- |-------- |---------------:|------:|----------:|
|     **StackMemoryCollections** |     **100** |       **360.5 ns** |  **0.97** |         **400 B** |
| System.Collections.Generic |     100 |       373.4 ns |  1.00 |     456 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |    **1000** |     **2,903.0 ns** |  **0.80** |         **4000 B** |
| System.Collections.Generic |    1000 |     3,631.2 ns |  1.00 |    4056 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |   **10000** |    **28,385.7 ns** |  **0.79** |         **40000 B** |
| System.Collections.Generic |   10000 |    35,998.6 ns |  1.00 |   40056 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **100000** |   **299,093.5 ns** |  **0.64** |         **400000 B** |
| System.Collections.Generic |  100000 |   465,908.0 ns |  1.00 |  400098 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **250000** |   **748,565.4 ns** |  **0.65** |       **1000001 B** |
| System.Collections.Generic |  250000 | 1,151,418.7 ns |  1.00 | 1000140 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **500000** | **1,706,442.1 ns** |  **0.74** |       **2000001 B** |
| System.Collections.Generic |  500000 | 2,301,156.4 ns |  1.00 | 2000225 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** | **1000000** | **3,426,771.5 ns** |  **0.73** |       **4000002 B** |
| System.Collections.Generic | 1000000 | 4,671,735.4 ns |  1.00 | 4000393 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/PrimitiveSimpleJob.cs)
______
  
|                     Method |    Size |          Mean | Ratio |   Allocated |
|--------------------------- |-------- |--------------:|------:|------------:|
|     **StackMemoryCollections** |     **100** |      **18.73 μs** |  **0.35** |           **400 B** |
| System.Collections.Generic |     100 |      53.68 μs |  1.00 |     91200 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |    **1000** |     **154.49 μs** |  **0.29** |           **4000 B** |
| System.Collections.Generic |    1000 |     529.04 μs |  1.00 |    811200 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |   **10000** |   **1,522.92 μs** |  **0.29** |         **40001 B** |
| System.Collections.Generic |   10000 |   5,312.62 μs |  1.00 |   8011204 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **100000** |  **15,319.70 μs** |  **0.20** |         **400008 B** |
| System.Collections.Generic |  100000 |  75,864.45 μs |  1.00 |  80019705 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **250000** |  **80,703.48 μs** |  **0.47** |        **1000080 B** |
| System.Collections.Generic |  250000 | 182,662.39 μs |  1.00 | 200029085 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **500000** | **156,821.19 μs** |  **0.45** |       **2000120 B** |
| System.Collections.Generic |  500000 | 365,647.36 μs |  1.00 | 400048080 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** | **1000000** | **314,662.23 μs** |  **0.39** |       **4000240 B** |
| System.Collections.Generic | 1000000 | 744,976.05 μs |  1.00 | 800078544 B |
  
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/PrimitiveOptimalJob.cs)
______
### Class:
Stack elements are classes.
|                     Method |    Size |             Mean | Ratio |  Allocated |
|--------------------------- |-------- |-----------------:|------:|-----------:|
|     **StackMemoryCollections** |     **100** |         **871.9 ns** |  **0.63** |          **2400 B** |
| System.Collections.Generic |     100 |       1,386.5 ns |  1.00 |     8056 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |    **1000** |       **7,755.5 ns** |  **0.57** |          **24000 B** |
| System.Collections.Generic |    1000 |      13,575.8 ns |  1.00 |    80056 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |   **10000** |      **84,282.9 ns** |  **0.59** |          **240000 B** |
| System.Collections.Generic |   10000 |     143,682.3 ns |  1.00 |   800056 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |  **100000** |   **1,183,408.5 ns** |  **0.30** |        **2400001 B** |
| System.Collections.Generic |  100000 |   4,469,723.9 ns |  1.00 |  8000222 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |  **250000** |   **2,879,588.8 ns** |  **0.17** |        **6000002 B** |
| System.Collections.Generic |  250000 |  20,460,644.6 ns |  1.00 | 20000288 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |  **500000** |   **5,871,041.9 ns** |  **0.13** |        **12000004 B** |
| System.Collections.Generic |  500000 |  45,195,722.3 ns |  1.00 | 40000672 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** | **1000000** |  **12,164,394.5 ns** |  **0.08** |        **24000008 B** |
| System.Collections.Generic | 1000000 | 154,214,865.0 ns |  1.00 | 80000794 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/ClassSimpleJob.cs)
______

|                     Method |    Size |             Mean | Ratio |     Allocated |
|--------------------------- |-------- |-----------------:|------:|--------------:|
|     **StackMemoryCollections** |     **100** |         **39.35 μs** |  **0.16** |             **2400 B** |
| System.Collections.Generic |     100 |        248.25 μs |  1.00 |     1611200 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |    **1000** |        **370.24 μs** |  **0.15** |             **24000 B** |
| System.Collections.Generic |    1000 |      2,424.03 μs |  1.00 |    16011202 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |   **10000** |      **3,784.23 μs** |  **0.15** |           **240002 B** |
| System.Collections.Generic |   10000 |     25,129.48 μs |  1.00 |   160011216 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |  **100000** |     **38,971.40 μs** |  **0.06** |          **2400034 B** |
| System.Collections.Generic |  100000 |    677,033.05 μs |  1.00 |  1600044272 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |  **250000** |     **97,223.39 μs** |  **0.03** |         **6000179 B** |
| System.Collections.Generic |  250000 |  4,023,699.32 μs |  1.00 |  4000059432 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |  **500000** |    **198,014.94 μs** |  **0.02** |         **12000160 B** |
| System.Collections.Generic |  500000 | 11,924,326.36 μs |  1.00 |  8000122032 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** | **1000000** |    **454,021.48 μs** |  **0.01** |         **24000480 B** |
| System.Collections.Generic | 1000000 | 32,867,097.87 μs |  1.00 | 16000114184 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/ClassOptimalJob.cs)
______
### Struct:
Stack elements are structures.

|                     Method |    Size |            Mean | Ratio |  Allocated |
|--------------------------- |-------- |----------------:|------:|-----------:|
|     **StackMemoryCollections** |     **100** |        **558.6 ns** |  **0.45** |          **2400 B** |
| System.Collections.Generic |     100 |      1,236.6 ns |  1.00 |     3256 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |    **1000** |      **5,010.9 ns** |  **0.41** |          **24000 B** |
| System.Collections.Generic |    1000 |     12,268.9 ns |  1.00 |    32056 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |   **10000** |     **62,182.3 ns** |  **0.29** |          **240000 B** |
| System.Collections.Generic |   10000 |    202,464.0 ns |  1.00 |   320090 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **100000** |  **1,351,478.7 ns** |  **0.64** |        **2400002 B** |
| System.Collections.Generic |  100000 |  1,960,525.7 ns |  1.00 |  3200392 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **250000** |  **3,121,903.7 ns** |  **0.87** |        **6000002 B** |
| System.Collections.Generic |  250000 |  4,105,450.0 ns |  1.00 |  8000155 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **500000** |  **4,168,385.4 ns** |  **0.54** |        **12000004 B** |
| System.Collections.Generic |  500000 |  7,769,854.8 ns |  1.00 | 16000153 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** | **1000000** |  **8,373,185.8 ns** |  **0.40** |        **24000008 B** |
| System.Collections.Generic | 1000000 | 20,410,695.4 ns |  1.00 | 32000396 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/StructSimpleJob.cs)
______

|                     Method |    Size |            Mean | Ratio |    Allocated |
|--------------------------- |-------- |----------------:|------:|-------------:|
|           **StackMemoryCollections** |     **100** |        **30.77 μs** |  **0.14** |            **2400 B** |
| System.Collections.Generic |     100 |       218.81 μs |  1.00 |     651200 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |    **1000** |       **284.43 μs** |  **0.14** |            **24000 B** |
| System.Collections.Generic |    1000 |     2,064.78 μs |  1.00 |    6411202 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |   **10000** |     **2,873.41 μs** |  **0.08** |          **240002 B** |
| System.Collections.Generic |   10000 |    36,188.54 μs |  1.00 |   64017930 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |  **100000** |    **32,915.40 μs** |  **0.10** |         **2400030 B** |
| System.Collections.Generic |  100000 |   341,734.18 μs |  1.00 |  640081632 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |  **250000** |    **85,029.14 μs** |  **0.11** |         **6000069 B** |
| System.Collections.Generic |  250000 |   765,138.65 μs |  1.00 | 1600031640 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |  **500000** |   **173,910.08 μs** |  **0.12** |        **12000160 B** |
| System.Collections.Generic |  500000 | 1,452,486.01 μs |  1.00 | 3200031072 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** | **1000000** |   **417,978.28 μs** |  **0.11** |        **24000480 B** |
| System.Collections.Generic | 1000000 | 3,846,122.47 μs |  1.00 | 6400078816 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/StructOptimalJob.cs)

______

# Queue

### Primitive types:
Queue elements are primitives: `byte`, `float`, `int`, `short`, `decimal`... .
|                     Method |    Size |           Mean | Ratio | Allocated |
|--------------------------- |-------- |---------------:|------:|----------:|
|     **StackMemoryCollections** |     **100** |       **389.9 ns** |  **0.71** |         **400 B** |
| System.Collections.Generic |     100 |       550.1 ns |  1.00 |     464 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |    **1000** |     **3,177.2 ns** |  **0.58** |         **4000 B** |
| System.Collections.Generic |    1000 |     5,499.3 ns |  1.00 |    4064 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |   **10000** |    **31,035.0 ns** |  **0.60** |         **40000 B** |
| System.Collections.Generic |   10000 |    51,680.2 ns |  1.00 |   40064 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **100000** |   **327,341.1 ns** |  **0.53** |         **400000 B** |
| System.Collections.Generic |  100000 |   619,905.5 ns |  1.00 |  400106 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **250000** |   **815,390.2 ns** |  **0.52** |         **1000000 B** |
| System.Collections.Generic |  250000 | 1,556,367.8 ns |  1.00 | 1000148 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **500000** | **1,868,335.4 ns** |  **0.60** |       **2000001 B** |
| System.Collections.Generic |  500000 | 3,118,756.4 ns |  1.00 | 2000233 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** | **1000000** | **4,244,397.5 ns** |  **0.66** |       **4000004 B** |
| System.Collections.Generic | 1000000 | 6,273,466.6 ns |  1.00 | 4000402 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Simple/PrimitiveSimpleJob.cs)
______

|                     Method |    Size |            Mean | Ratio |   Allocated |
|--------------------------- |-------- |----------------:|------:|------------:|
|     **StackMemoryCollections** |     **100** |        **68.32 μs** |  **0.83** |           **2400 B** |
| System.Collections.Generic |     100 |        81.52 μs |  1.00 |     92800 B |
|                            |         |                 |       |             |
|     **StackMemoryCollections** |    **1000** |       **666.10 μs** |  **0.80** |           **24000 B** |
| System.Collections.Generic |    1000 |       826.18 μs |  1.00 |    812801 B |
|                            |         |                 |       |             |
|     **StackMemoryCollections** |   **10000** |     **6,284.88 μs** |  **0.79** |         **240005 B** |
| System.Collections.Generic |   10000 |     7,960.78 μs |  1.00 |   8012808 B |
|                            |         |                 |       |             |
|     **StackMemoryCollections** |  **100000** |    **57,392.77 μs** |  **0.56** |        **2400053 B** |
| System.Collections.Generic |  100000 |   102,318.69 μs |  1.00 |  80021846 B |
|                            |         |                 |       |             |
|     **StackMemoryCollections** |  **250000** |   **142,255.49 μs** |  **0.57** |       **6000268 B** |
| System.Collections.Generic |  250000 |   247,818.64 μs |  1.00 | 200029648 B |
|                            |         |                 |       |             |
|     **StackMemoryCollections** |  **500000** |   **285,382.63 μs** |  **0.58** |       **12000288 B** |
| System.Collections.Generic |  500000 |   493,753.23 μs |  1.00 | 400049648 B |
|                            |         |                 |       |             |
|     **StackMemoryCollections** | **1000000** |   **570,681.45 μs** |  **0.57** |      **24002224 B** |
| System.Collections.Generic | 1000000 | 1,003,561.57 μs |  1.00 | 800080144 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Optimal/PrimitiveOptimalJob.cs)
______
### Class:
Queue elements are classes.
|                     Method |    Size |             Mean | Ratio |  Allocated |
|--------------------------- |-------- |-----------------:|------:|-----------:|
|     **StackMemoryCollections** |     **100** |         **934.8 ns** |  **0.66** |          **2400 B** |
| System.Collections.Generic |     100 |       1,417.3 ns |  1.00 |     8064 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |    **1000** |       **8,345.5 ns** |  **0.61** |          **24000 B** |
| System.Collections.Generic |    1000 |      13,729.9 ns |  1.00 |    80064 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |   **10000** |     **105,277.2 ns** |  **0.69** |          **240000 B** |
| System.Collections.Generic |   10000 |     151,818.2 ns |  1.00 |   800064 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |  **100000** |   **1,206,658.6 ns** |  **0.31** |        **2400001 B** |
| System.Collections.Generic |  100000 |   4,112,381.3 ns |  1.00 |  8000149 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |  **250000** |   **3,129,761.7 ns** |  **0.16** |        **6000002 B** |
| System.Collections.Generic |  250000 |  20,969,077.0 ns |  1.00 | 20000305 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** |  **500000** |   **6,207,084.6 ns** |  **0.12** |        **12000004 B** |
| System.Collections.Generic |  500000 |  50,361,973.0 ns |  1.00 | 40000856 B |
|                            |         |                  |       |            |
|     **StackMemoryCollections** | **1000000** |  **12,657,254.9 ns** |  **0.09** |        **24000008 B** |
| System.Collections.Generic | 1000000 | 135,295,812.1 ns |  1.00 | 80000798 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Simple/ClassSimpleJob.cs)
______

|                     Method |    Size |             Mean | Ratio |     Allocated |
|--------------------------- |-------- |-----------------:|------:|--------------:|
|     **StackMemoryCollections** |     **100** |         **78.27 μs** |  **0.32** |             **2400 B** |
| System.Collections.Generic |     100 |        243.66 μs |  1.00 |     1612800 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |    **1000** |        **724.90 μs** |  **0.30** |             **24000 B** |
| System.Collections.Generic |    1000 |      2,418.62 μs |  1.00 |    16012802 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |   **10000** |      **7,404.89 μs** |  **0.28** |           **240004 B** |
| System.Collections.Generic |   10000 |     26,401.18 μs |  1.00 |   160012816 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |  **100000** |     **74,443.73 μs** |  **0.09** |          **2400060 B** |
| System.Collections.Generic |  100000 |    911,137.24 μs |  1.00 |  1600053936 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |  **250000** |    **181,279.10 μs** |  **0.05** |         **6000160 B** |
| System.Collections.Generic |  250000 |  4,024,212.19 μs |  1.00 |  4000060024 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |  **500000** |    **349,768.77 μs** |  **0.03** |        **12002224 B** |
| System.Collections.Generic |  500000 | 12,300,288.89 μs |  1.00 |  8000087392 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** | **1000000** |  **1,115,481.67 μs** |  **0.03** |         **24000480 B** |
| System.Collections.Generic | 1000000 | 33,229,449.56 μs |  1.00 | 16000114480 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Optimal/ClassOptimalJob.cs)
______
### Struct:
Queue elements are structures.

|                     Method |    Size |            Mean | Ratio |  Allocated |
|--------------------------- |-------- |----------------:|------:|-----------:|
|     **StackMemoryCollections** |     **100** |        **638.7 ns** |  **0.50** |          **2400 B** |
| System.Collections.Generic |     100 |      1,279.7 ns |  1.00 |     3264 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |    **1000** |      **5,276.0 ns** |  **0.42** |          **24000 B** |
| System.Collections.Generic |    1000 |     12,542.3 ns |  1.00 |    32064 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |   **10000** |     **64,383.9 ns** |  **0.31** |          **240000 B** |
| System.Collections.Generic |   10000 |    207,398.3 ns |  1.00 |   320098 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **100000** |  **1,251,235.0 ns** |  **0.68** |          **2400000 B** |
| System.Collections.Generic |  100000 |  2,068,957.6 ns |  1.00 |  3200401 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **250000** |  **2,217,628.5 ns** |  **0.50** |        **6000002 B** |
| System.Collections.Generic |  250000 |  4,427,810.8 ns |  1.00 |  8000151 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **500000** |  **6,112,196.6 ns** |  **0.77** |        **12000004 B** |
| System.Collections.Generic |  500000 |  8,190,663.7 ns |  1.00 | 16000184 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** | **1000000** | **12,699,370.3 ns** |  **0.54** |        **24000008 B** |
| System.Collections.Generic | 1000000 | 22,084,283.1 ns |  1.00 | 32000410 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Simple/StructSimpleJob.cs)
______

|                     Method |    Size |            Mean | Ratio |    Allocated |
|--------------------------- |-------- |----------------:|------:|-------------:|
|           **StackMemoryCollections** |     **100** |        **68.02 μs** |  **0.28** |            **2400 B** |
| System.Collections.Generic |     100 |       238.76 μs |  1.00 |     652800 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |    **1000** |       **600.34 μs** |  **0.28** |            **24000 B** |
| System.Collections.Generic |    1000 |     2,117.25 μs |  1.00 |    6412802 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |   **10000** |     **6,162.54 μs** |  **0.15** |          **240004 B** |
| System.Collections.Generic |   10000 |    40,011.38 μs |  1.00 |   64019535 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |  **100000** |    **61,010.02 μs** |  **0.16** |         **2400053 B** |
| System.Collections.Generic |  100000 |   373,640.10 μs |  1.00 |  640080144 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |  **250000** |   **157,181.17 μs** |  **0.19** |        **6000120 B** |
| System.Collections.Generic |  250000 |   823,647.11 μs |  1.00 | 1600032920 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** |  **500000** |   **303,541.81 μs** |  **0.19** |        **12000240 B** |
| System.Collections.Generic |  500000 | 1,612,937.90 μs |  1.00 | 3200034184 B |
|                            |         |                 |       |              |
|           **StackMemoryCollections** | **1000000** | **1,080,878.12 μs** |  **0.28** |        **24000480 B** |
| System.Collections.Generic | 1000000 | 3,920,606.55 μs |  1.00 | 6400080144 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Optimal/StructOptimalJob.cs)

______

# List

### Primitive types:
Queue elements are primitives: `byte`, `float`, `int`, `short`, `decimal`... .
|                     Method |    Size |           Mean | Ratio | Allocated |
|--------------------------- |-------- |---------------:|------:|----------:|
|     **StackMemoryCollections** |     **100** |       **402.2 ns** |  **0.95** |         **400 B** |
| System.Collections.Generic |     100 |       423.3 ns |  1.00 |     456 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |    **1000** |     **3,325.0 ns** |  **0.82** |         **4000 B** |
| System.Collections.Generic |    1000 |     4,070.2 ns |  1.00 |    4056 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |   **10000** |    **32,585.5 ns** |  **0.81** |         **40000 B** |
| System.Collections.Generic |   10000 |    40,326.4 ns |  1.00 |   40056 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **100000** |   **402,172.2 ns** |  **0.78** |         **400000 B** |
| System.Collections.Generic |  100000 |   510,568.8 ns |  1.00 |  400098 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **250000** | **1,306,561.9 ns** |  **1.03** |       **1000001 B** |
| System.Collections.Generic |  250000 | 1,294,331.1 ns |  1.00 | 1000140 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **500000** | **2,229,919.5 ns** |  **0.88** |       **2000002 B** |
| System.Collections.Generic |  500000 | 2,502,543.0 ns |  1.00 | 2000225 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** | **1000000** | **4,793,840.6 ns** |  **0.95** |       **4000004 B** |
| System.Collections.Generic | 1000000 | 5,005,536.2 ns |  1.00 | 4000393 B |
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Simple/PrimitiveSimpleJob.cs)
______

|                     Method |    Size |          Mean | Ratio |   Allocated |
|--------------------------- |-------- |--------------:|------:|------------:|
|     **StackMemoryCollections** |     **100** |      **50.24 μs** |  **0.82** |           **400 B** |
| System.Collections.Generic |     100 |      61.50 μs |  1.00 |     91200 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |    **1000** |     **472.60 μs** |  **0.80** |           **4000 B** |
| System.Collections.Generic |    1000 |     589.54 μs |  1.00 |    811200 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |   **10000** |   **4,699.71 μs** |  **0.83** |         **40004 B** |
| System.Collections.Generic |   10000 |   5,680.55 μs |  1.00 |   8011204 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **100000** |  **47,089.39 μs** |  **0.60** |        **400044 B** |
| System.Collections.Generic |  100000 |  78,663.28 μs |  1.00 |  80020059 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **250000** | **121,598.15 μs** |  **0.62** |        **1000096 B** |
| System.Collections.Generic |  250000 | 196,329.26 μs |  1.00 | 200029072 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **500000** | **236,688.60 μs** |  **0.63** |       **2000160 B** |
| System.Collections.Generic |  500000 | 378,948.09 μs |  1.00 | 400048032 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** | **1000000** | **471,179.85 μs** |  **0.61** |      **4002224 B** |
| System.Collections.Generic | 1000000 | 771,659.38 μs |  1.00 | 800080336 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Optimal/PrimitiveOptimalJob.cs)
______
### Class:
Queue elements are classes.
|                     Method |    Size |           Mean | Ratio |  Allocated |
|--------------------------- |-------- |---------------:|------:|-----------:|
|     **StackMemoryCollections** |     **100** |       **1.035 μs** |  **0.75** |          **2400 B** |
| System.Collections.Generic |     100 |       1.372 μs |  1.00 |     8056 B |
|                            |         |                |       |            |
|     **StackMemoryCollections** |    **1000** |      **10.238 μs** |  **0.76** |          **24000 B** |
| System.Collections.Generic |    1000 |      13.558 μs |  1.00 |    80056 B |
|                            |         |                |       |            |
|     **StackMemoryCollections** |   **10000** |     **108.900 μs** |  **0.75** |          **240000 B** |
| System.Collections.Generic |   10000 |     144.636 μs |  1.00 |   800056 B |
|                            |         |                |       |            |
|     **StackMemoryCollections** |  **100000** |   **1,315.957 μs** |  **0.30** |        **2400001 B** |
| System.Collections.Generic |  100000 |   4,130.503 μs |  1.00 |  8000141 B |
|                            |         |                |       |            |
|     **StackMemoryCollections** |  **250000** |   **3,348.137 μs** |  **0.19** |        **6000002 B** |
| System.Collections.Generic |  250000 |  17,567.766 μs |  1.00 | 20000287 B |
|                            |         |                |       |            |
|     **StackMemoryCollections** |  **500000** |   **6,687.458 μs** |  **0.11** |        **12000004 B** |
| System.Collections.Generic |  500000 |  57,512.715 μs |  1.00 | 40000670 B |
|                            |         |                |       |            |
|     **StackMemoryCollections** | **1000000** |  **13,881.078 μs** |  **0.09** |        **24000008 B** |
| System.Collections.Generic | 1000000 | 147,485.595 μs |  1.00 | 80000794 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Simple/ClassSimpleJob.cs)
______

|                     Method |    Size |             Mean | Ratio |     Allocated |
|--------------------------- |-------- |-----------------:|------:|--------------:|
|     **StackMemoryCollections** |     **100** |         **60.16 μs** |  **0.26** |             **2400 B** |
| System.Collections.Generic |     100 |        233.90 μs |  1.00 |     1611200 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |    **1000** |        **581.30 μs** |  **0.25** |           **24001 B** |
| System.Collections.Generic |    1000 |      2,312.54 μs |  1.00 |    16011202 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |   **10000** |      **5,866.07 μs** |  **0.23** |           **240004 B** |
| System.Collections.Generic |   10000 |     25,280.37 μs |  1.00 |   160011216 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |  **100000** |     **62,648.67 μs** |  **0.08** |          **2400053 B** |
| System.Collections.Generic |  100000 |    826,362.25 μs |  1.00 |  1600028144 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |  **250000** |    **155,187.57 μs** |  **0.04** |         **6000120 B** |
| System.Collections.Generic |  250000 |  4,179,162.18 μs |  1.00 |  4000058448 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** |  **500000** |    **313,635.15 μs** |  **0.03** |         **12000240 B** |
| System.Collections.Generic |  500000 | 11,768,582.86 μs |  1.00 |  8000133056 B |
|                            |         |                  |       |               |
|     **StackMemoryCollections** | **1000000** |    **679,751.59 μs** |  **0.02** |        **24001072 B** |
| System.Collections.Generic | 1000000 | 31,926,569.52 μs |  1.00 | 16000120936 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Optimal/ClassOptimalJob.cs)
______
### Struct:
Queue elements are structures.

|                     Method |    Size |            Mean | Ratio |  Allocated |
|--------------------------- |-------- |----------------:|------:|-----------:|
|     **StackMemoryCollections** |     **100** |        **549.4 ns** |  **0.47** |          **2400 B** |
| System.Collections.Generic |     100 |      1,166.5 ns |  1.00 |     3256 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |    **1000** |      **4,861.2 ns** |  **0.42** |          **24000 B** |
| System.Collections.Generic |    1000 |     11,453.0 ns |  1.00 |    32056 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |   **10000** |     **57,908.7 ns** |  **0.30** |          **240000 B** |
| System.Collections.Generic |   10000 |    194,387.6 ns |  1.00 |   320090 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **100000** |  **1,005,552.5 ns** |  **0.56** |          **2400000 B** |
| System.Collections.Generic |  100000 |  1,947,173.5 ns |  1.00 |  3200392 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **250000** |  **2,377,210.4 ns** |  **0.59** |        **6000002 B** |
| System.Collections.Generic |  250000 |  3,912,643.2 ns |  1.00 |  8000148 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** |  **500000** |  **4,819,554.6 ns** |  **0.67** |        **12000004 B** |
| System.Collections.Generic |  500000 |  7,602,517.4 ns |  1.00 | 16000156 B |
|                            |         |                 |       |            |
|     **StackMemoryCollections** | **1000000** | **10,078,648.6 ns** |  **0.53** |        **24000008 B** |
| System.Collections.Generic | 1000000 | 19,866,444.6 ns |  1.00 | 32000396 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Simple/StructSimpleJob.cs)
______

|                     Method |    Size |            Mean | Ratio |    Allocated |
|--------------------------- |-------- |----------------:|------:|-------------:|
|           **StackOfJobStruct** |     **100** |        **64.24 μs** |  **0.30** |            **2400 B** |
| System.Collections.Generic |     100 |       211.24 μs |  1.00 |     651200 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |    **1000** |       **618.33 μs** |  **0.31** |            **24000 B** |
| System.Collections.Generic |    1000 |     2,026.44 μs |  1.00 |    6411202 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |   **10000** |     **6,226.81 μs** |  **0.17** |          **240004 B** |
| System.Collections.Generic |   10000 |    36,371.81 μs |  1.00 |   64017930 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |  **100000** |    **64,389.84 μs** |  **0.18** |         **2400053 B** |
| System.Collections.Generic |  100000 |   354,953.00 μs |  1.00 |  640080288 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |  **250000** |   **160,951.43 μs** |  **0.22** |        **6000120 B** |
| System.Collections.Generic |  250000 |   729,754.85 μs |  1.00 | 1600030992 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** |  **500000** |   **325,906.75 μs** |  **0.24** |        **12000240 B** |
| System.Collections.Generic |  500000 | 1,394,296.83 μs |  1.00 | 3200034424 B |
|                            |         |                 |       |              |
|           **StackOfJobStruct** | **1000000** |   **660,610.88 μs** |  **0.18** |        **24000720 B** |
| System.Collections.Generic | 1000000 | 3,696,655.42 μs |  1.00 | 6400080288 B |

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Optimal/StructOptimalJob.cs)

______
