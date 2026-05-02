# Stack

### Primitive types:
Stack elements are primitives: `byte`, `float`, `int`, `short`, `decimal`... .
  
| Method                     | Job            | Runtime        | Size | Mean        | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |    **278.6 ns** |  **0.72** |         **400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    385.9 ns |  1.00 |     456 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    296.9 ns |  0.78 |     400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    378.5 ns |  1.00 |     456 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |  **2,182.2 ns** |  **0.59** |         **4000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |  3,707.0 ns |  1.00 |    4056 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  1,719.4 ns |  0.47 |    4000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  3,666.4 ns |  1.00 |    4056 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** | **10,246.5 ns** |  **0.55** |         **20000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 18,509.1 ns |  1.00 |   20056 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |  8,703.3 ns |  0.49 |   20000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 17,769.9 ns |  1.00 |   20056 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/PrimitiveSimpleJob.cs)
______
  
| Method                     | Job            | Runtime        | Size | Mean        | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |    **22.61 μs** |  **0.38** |         **400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    59.50 μs |  1.00 |   91200 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    22.38 μs |  0.39 |     400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    57.50 μs |  1.00 |   91200 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |   **217.65 μs** |  **0.38** |         **4000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |   574.88 μs |  1.00 |  811200 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   190.75 μs |  0.34 |    4000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   563.65 μs |  1.00 |  811200 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** | **1,054.74 μs** |  **0.37** |         **20000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 2,841.91 μs |  1.00 | 4011200 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |   939.51 μs |  0.35 |   20000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 2,718.04 μs |  1.00 | 4011200 B | 

  
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/PrimitiveOptimalJob.cs)
______
### Class:
Stack elements are classes.
| Method                     | Job            | Runtime        | Size | Mean        | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |    **988.7 ns** |  **0.76** |         **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |  1,297.2 ns |  1.00 |    8056 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    948.4 ns |  0.66 |    2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |  1,447.5 ns |  1.00 |    8056 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |  **8,842.7 ns** |  **0.74** |         **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 | 12,144.9 ns |  1.01 |   80056 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  8,810.9 ns |  0.60 |   24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 | 14,749.1 ns |  1.00 |   80056 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** | **41,412.4 ns** |  **0.66** |         **12000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 62,345.4 ns |  1.00 |  400056 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 41,577.8 ns |  0.59 |   12000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 70,237.5 ns |  1.00 |  400056 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/ClassSimpleJob.cs)
______

| Method                     | Job            | Runtime        | Size | Mean         | Ratio | Allocated  | 
|--------------------------- |--------------- |--------------- |----- |-------------:|------:|-----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |     **44.69 μs** |  **0.19** |          **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    239.90 μs |  1.00 |  1611200 B | 
|                            |                |                |      |              |       |            | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     43.04 μs |  0.16 |     2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    277.39 μs |  1.00 |  1611200 B | 
|                            |                |                |      |              |       |            | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |    **417.37 μs** |  **0.17** |          **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |  2,444.36 μs |  1.00 | 16011200 B | 
|                            |                |                |      |              |       |            | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |    410.99 μs |  0.15 |    24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  2,745.45 μs |  1.00 | 16011202 B | 
|                            |                |                |      |              |       |            | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** |  **2,084.63 μs** |  **0.17** |          **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 12,175.40 μs |  1.00 | 80011200 B | 
|                            |                |                |      |              |       |            | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |  2,063.54 μs |  0.15 |   120000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 13,864.16 μs |  1.00 | 80011206 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/ClassOptimalJob.cs)
______
### Struct:
Stack elements are structures.

| Method                     | Job            | Runtime        | Size | Mean         | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |-------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |     **356.7 ns** |  **0.67** |         **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |     531.6 ns |  1.00 |    3256 B | 
|                            |                |                |      |              |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     345.8 ns |  0.35 |    2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     979.8 ns |  1.00 |    3256 B | 
|                            |                |                |      |              |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |   **3,218.2 ns** |  **0.67** |         **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |   4,802.4 ns |  1.00 |   32056 B | 
|                            |                |                |      |              |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   2,767.0 ns |  0.29 |   24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   9,492.0 ns |  1.00 |   32056 B | 
|                            |                |                |      |              |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** |  **13,041.2 ns** |  **0.18** |         **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 |  72,262.1 ns |  1.00 |  160073 B | 
|                            |                |                |      |              |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |  12,952.9 ns |  0.12 |  120000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 104,881.5 ns |  1.00 |  160090 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/StructSimpleJob.cs)
______

| Method                     | Job            | Runtime        | Size | Mean         | Ratio | Allocated  | 
|--------------------------- |--------------- |--------------- |----- |-------------:|------:|-----------:|
| **StackOfJobStruct**           | **.NET 10.0**      | **.NET 10.0**      | **100**  |     **37.34 μs** |  **0.42** |          **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |     88.92 μs |  1.00 |   651200 B | 
|                            |                |                |      |              |       |            | 
| StackOfJobStruct           | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     35.54 μs |  0.18 |     2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    193.93 μs |  1.00 |   651200 B | 
|                            |                |                |      |              |       |            | 
| **StackOfJobStruct**           | **.NET 10.0**      | **.NET 10.0**      | **1000** |    **350.38 μs** |  **0.45** |          **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |    787.05 μs |  1.00 |  6411200 B | 
|                            |                |                |      |              |       |            | 
| StackOfJobStruct           | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |    326.62 μs |  0.18 |    24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  1,810.63 μs |  1.00 |  6411201 B | 
|                            |                |                |      |              |       |            | 
| **StackOfJobStruct**           | **.NET 10.0**      | **.NET 10.0**      | **5000** |  **1,726.77 μs** |  **0.13** |          **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 13,665.30 μs |  1.00 | 32014555 B | 
|                            |                |                |      |              |       |            | 
| StackOfJobStruct           | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |  1,641.40 μs |  0.08 |    12000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 19,940.04 μs |  1.00 | 32017920 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/StructOptimalJob.cs)

______

# Queue

### Primitive types:
Queue elements are primitives: `byte`, `float`, `int`, `short`, `decimal`... .
| Method                     | Job            | Runtime        | Size | Mean        | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |    **401.8 ns** |  **0.83** |         **400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    481.9 ns |  1.00 |     464 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    583.7 ns |  1.20 |     400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    488.7 ns |  1.00 |     464 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |  **3,916.6 ns** |  **0.82** |         **4000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |  4,763.2 ns |  1.00 |    4064 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  4,685.5 ns |  0.97 |    4000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  4,842.4 ns |  1.00 |    4064 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** | **17,151.5 ns** |  **0.73** |         **20000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 23,658.0 ns |  1.00 |   20064 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 23,196.0 ns |  0.98 |   20000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 23,582.0 ns |  1.00 |   20064 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Simple/PrimitiveSimpleJob.cs)
______

| Method                     | Job            | Runtime        | Size | Mean        | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |    **38.38 μs** |  **0.50** |         **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    76.07 μs |  1.00 |   92800 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    69.70 μs |  0.90 |    2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    77.88 μs |  1.00 |   92800 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |   **400.58 μs** |  **0.55** |         **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |   732.61 μs |  1.00 |  812800 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   659.36 μs |  0.89 |   24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   740.95 μs |  1.00 |  812800 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** | **1,568.41 μs** |  **0.39** |         **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 4,065.96 μs |  1.00 | 4012800 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 3,243.53 μs |  0.89 |  120002 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 3,637.90 μs |  1.00 | 4012800 B | 


[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Optimal/PrimitiveOptimalJob.cs)
______
### Class:
Queue elements are classes.
| Method                     | Job            | Runtime        | Size | Mean      | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |----------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |  **1.013 μs** |  **0.72** |         **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |  1.404 μs |  1.00 |    8064 B | 
|                            |                |                |      |           |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |  1.069 μs |  0.70 |    2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |  1.537 μs |  1.00 |    8064 B | 
|                            |                |                |      |           |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** | **10.563 μs** |  **0.71** |         **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 | 14.898 μs |  1.00 |   80064 B | 
|                            |                |                |      |           |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 | 10.930 μs |  0.72 |   24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 | 15.292 μs |  1.00 |   80064 B | 
|                            |                |                |      |           |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** | **50.897 μs** |  **0.71** |         **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 71.977 μs |  1.00 |  400064 B | 
|                            |                |                |      |           |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 47.591 μs |  0.63 |  120000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 75.973 μs |  1.00 |  400064 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Simple/ClassSimpleJob.cs)
______

| Method                     | Job            | Runtime        | Size | Mean         | Ratio | Allocated  | 
|--------------------------- |--------------- |--------------- |----- |-------------:|------:|-----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |     **53.42 μs** |  **0.20** |          **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    263.67 μs |  1.00 |  1612800 B | 
|                            |                |                |      |              |       |            | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     87.77 μs |  0.31 |     2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    286.25 μs |  1.00 |  1612800 B | 
|                            |                |                |      |              |       |            | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |    **491.95 μs** |  **0.18** |          **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |  2,669.16 μs |  1.00 | 16012800 B | 
|                            |                |                |      |              |       |            | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |    872.89 μs |  0.31 |    24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  2,859.36 μs |  1.00 | 16012802 B | 
|                            |                |                |      |              |       |            | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** |  **2,446.57 μs** |  **0.18** |          **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 13,369.48 μs |  1.00 | 80012800 B | 
|                            |                |                |      |              |       |            | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |  4,322.25 μs |  0.30 |   120002 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 14,211.53 μs |  1.00 | 80012800 B | 


[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Optimal/ClassOptimalJob.cs)
______
### Struct:
Queue elements are structures.

| Method                     | Job            | Runtime        | Size | Mean        | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |    **687.8 ns** |  **1.06** |         **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    651.9 ns |  1.00 |    3264 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    877.2 ns |  1.37 |    2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    641.1 ns |  1.00 |    3264 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |  **7,032.9 ns** |  **1.20** |         **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |  5,886.5 ns |  1.00 |   32064 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  7,939.3 ns |  1.39 |   24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  5,731.5 ns |  1.00 |   32064 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** | **34,635.4 ns** |  **0.44** |         **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 78,919.7 ns |  1.01 |  160081 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 38,622.9 ns |  0.45 |  120000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 85,605.6 ns |  1.00 |  160098 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Simple/StructSimpleJob.cs)
______

| Method                     | Job            | Runtime        | Size | Mean         | Ratio | Allocated  | 
|--------------------------- |--------------- |--------------- |----- |-------------:|------:|-----------:|
| **StackOfJobStruct**           | **.NET 10.0**      | **.NET 10.0**      | **100**  |     **47.85 μs** |  **0.44** |          **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    108.22 μs |  1.00 |   652800 B | 
|                            |                |                |      |              |       |            | 
| StackOfJobStruct           | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     93.93 μs |  0.89 |     2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    106.22 μs |  1.00 |   652800 B | 
|                            |                |                |      |              |       |            | 
| **StackOfJobStruct**           | **.NET 10.0**      | **.NET 10.0**      | **1000** |    **442.46 μs** |  **0.45** |          **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |    976.34 μs |  1.00 |  6412800 B | 
|                            |                |                |      |              |       |            | 
| StackOfJobStruct           | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |    875.66 μs |  0.93 |    24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |    940.62 μs |  1.00 |  6412800 B | 
|                            |                |                |      |              |       |            | 
| **StackOfJobStruct**           | **.NET 10.0**      | **.NET 10.0**      | **5000** |  **2,073.50 μs** |  **0.14** |          **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 14,910.34 μs |  1.01 | 32016155 B | 
|                            |                |                |      |              |       |            | 
| StackOfJobStruct           | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |  4,418.43 μs |  0.27 |   120002 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 16,604.33 μs |  1.00 | 32019516 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Queue/Optimal/StructOptimalJob.cs)

______

# List

### Primitive types:
List elements are primitives: `byte`, `float`, `int`, `short`, `decimal`... .
| Method                     | Job            | Runtime        | Size | Mean        | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |    **284.1 ns** |  **0.76** |         **400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    372.6 ns |  1.00 |     456 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    449.5 ns |  1.18 |     400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    381.8 ns |  1.00 |     456 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |  **1,785.4 ns** |  **0.47** |         **4000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |  3,837.9 ns |  1.00 |    4056 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  3,933.9 ns |  1.04 |    4000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  3,788.2 ns |  1.00 |    4056 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** |  **8,755.2 ns** |  **0.50** |         **20000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 17,560.8 ns |  1.00 |   20056 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 18,804.2 ns |  1.04 |   20000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 18,093.9 ns |  1.00 |   20056 B | 
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Simple/PrimitiveSimpleJob.cs)
______

| Method                     | Job            | Runtime        | Size | Mean        | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |    **17.03 μs** |  **0.31** |         **400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    55.75 μs |  1.00 |   91200 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    56.89 μs |  1.03 |     400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    55.30 μs |  1.00 |   91200 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |   **286.78 μs** |  **0.50** |         **4000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |   578.87 μs |  1.00 |  811200 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   649.61 μs |  1.12 |    4000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   578.93 μs |  1.00 |  811200 B | 
|                            |                |                |      |             |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** | **1,379.77 μs** |  **0.48** |         **20000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 2,884.50 μs |  1.00 | 4011200 B | 
|                            |                |                |      |             |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 3,005.55 μs |  1.06 |   20002 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 2,846.96 μs |  1.00 | 4011203 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Optimal/PrimitiveOptimalJob.cs)
______
### Class:
List elements are classes.
| Method                     | Job            | Runtime        | Size | Mean      | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |----------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |  **1.042 μs** |  **0.80** |         **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |  1.299 μs |  1.00 |    8056 B | 
|                            |                |                |      |           |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |  1.282 μs |  0.93 |    2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |  1.381 μs |  1.00 |    8056 B | 
|                            |                |                |      |           |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |  **9.042 μs** |  **0.70** |         **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 | 12.949 μs |  1.00 |   80056 B | 
|                            |                |                |      |           |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 | 11.795 μs |  0.87 |   24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 | 13.595 μs |  1.00 |   80056 B | 
|                            |                |                |      |           |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** | **43.972 μs** |  **0.68** |         **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 64.899 μs |  1.00 |  400056 B | 
|                            |                |                |      |           |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 58.072 μs |  0.85 |  120000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 68.062 μs |  1.00 |  400056 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Simple/ClassSimpleJob.cs)
______

| Method                     | Job            | Runtime        | Size | Mean         | Ratio | Allocated  | 
|--------------------------- |--------------- |--------------- |----- |-------------:|------:|-----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |     **46.47 μs** |  **0.19** |          **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |    251.04 μs |  1.00 |  1611200 B | 
|                            |                |                |      |              |       |            | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     90.03 μs |  0.34 |     2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    264.28 μs |  1.00 |  1611200 B | 
|                            |                |                |      |              |       |            | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |    **542.95 μs** |  **0.22** |          **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |  2,514.90 μs |  1.01 | 16011200 B | 
|                            |                |                |      |              |       |            | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |    817.60 μs |  0.32 |    24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  2,538.32 μs |  1.00 | 16011200 B | 
|                            |                |                |      |              |       |            | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** |  **2,535.02 μs** |  **0.21** |          **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 12,030.99 μs |  1.00 | 80011200 B | 
|                            |                |                |      |              |       |            | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |  4,061.35 μs |  0.33 |   120000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 12,338.56 μs |  1.00 | 80011206 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Optimal/ClassOptimalJob.cs)
______
### Struct:
List elements are structures.

| Method                     | Job            | Runtime        | Size | Mean         | Ratio | Allocated | 
|--------------------------- |--------------- |--------------- |----- |-------------:|------:|----------:|
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **100**  |     **453.7 ns** |  **0.85** |         **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |     535.7 ns |  1.00 |    3256 B | 
|                            |                |                |      |              |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     675.4 ns |  0.71 |    2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     945.0 ns |  1.00 |    3256 B | 
|                            |                |                |      |              |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **1000** |   **3,020.7 ns** |  **0.61** |         **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |   4,917.6 ns |  1.00 |   32056 B | 
|                            |                |                |      |              |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   5,910.8 ns |  0.60 |   24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |   9,905.8 ns |  1.00 |   32056 B | 
|                            |                |                |      |              |       |           | 
| **StackMemoryCollections**     | **.NET 10.0**      | **.NET 10.0**      | **5000** |  **13,729.5 ns** |  **0.19** |         **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 |  74,165.9 ns |  1.00 |  160073 B | 
|                            |                |                |      |              |       |           | 
| StackMemoryCollections     | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |  29,329.6 ns |  0.27 |  120000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 107,837.7 ns |  1.00 |  160090 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Simple/StructSimpleJob.cs)
______

| Method                     | Job            | Runtime        | Size | Mean         | Ratio | Allocated  | 
|--------------------------- |--------------- |--------------- |----- |-------------:|------:|-----------:|
| **StackOfJobStruct**           | **.NET 10.0**      | **.NET 10.0**      | **100**  |     **50.23 μs** |  **0.56** |          **2400 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 100  |     89.32 μs |  1.00 |   651200 B | 
|                            |                |                |      |              |       |            | 
| StackOfJobStruct           | NativeAOT 10.0 | NativeAOT 10.0 | 100  |     93.05 μs |  0.51 |     2400 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 100  |    181.96 μs |  1.00 |   651200 B | 
|                            |                |                |      |              |       |            | 
| **StackOfJobStruct**           | **.NET 10.0**      | **.NET 10.0**      | **1000** |    **538.41 μs** |  **0.63** |          **24000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 1000 |    862.28 μs |  1.02 |  6411200 B | 
|                            |                |                |      |              |       |            | 
| StackOfJobStruct           | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |    890.83 μs |  0.52 |    24000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 1000 |  1,722.80 μs |  1.00 |  6411200 B | 
|                            |                |                |      |              |       |            | 
| **StackOfJobStruct**           | **.NET 10.0**      | **.NET 10.0**      | **5000** |  **2,707.76 μs** |  **0.19** |          **120000 B** | 
| System.Collections.Generic | .NET 10.0      | .NET 10.0      | 5000 | 14,680.33 μs |  1.01 | 32014555 B | 
|                            |                |                |      |              |       |            | 
| StackOfJobStruct           | NativeAOT 10.0 | NativeAOT 10.0 | 5000 |  4,375.81 μs |  0.21 |   120000 B | 
| System.Collections.Generic | NativeAOT 10.0 | NativeAOT 10.0 | 5000 | 20,765.75 μs |  1.00 | 32017920 B | 

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/List/Optimal/StructOptimalJob.cs)

______
