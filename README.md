# Readme:
Fast unsafe collections(generated at build time with Roslyn Generator) for memory reuse by stack type. Can also be used in as classic collection with resizing or on a custom memory allocator(constructor with `void*`), then it's your responsibility to make sure the pointer is correct.

Allows you to allocate memory for a method / class and place all sets of variables in it.
Avoid repeated copying of structures when placing them in collections.
And other use cases.

See [Documentation](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Documentation/Readme.md) for details.

Supported collections:
- Stack
- List TODO
- Queue TODO

## Benchmarks:

<details><summary>Stack</summary>

### Primitive types:
Stack elements are primitives: `byte`, `float`, `int`, `short`, `decimal`... .
  
|                     Method |    Size |           Mean | Ratio | Allocated |
|--------------------------- |-------- |---------------:|------:|----------:|
|     **StackMemoryCollections** |     **100** |       **508.6 ns** |  **1.45** |         **400 B** |
| System.Collections.Generic |     100 |       349.8 ns |  1.00 |     456 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |    **1000** |     **4,156.1 ns** |  **1.22** |         **4000 B** |
| System.Collections.Generic |    1000 |     3,397.4 ns |  1.00 |    4056 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |   **10000** |    **41,159.0 ns** |  **1.18** |         **40000 B** |
| System.Collections.Generic |   10000 |    34,840.5 ns |  1.00 |   40056 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **100000** |   **426,671.1 ns** |  **0.91** |       **400002 B** |
| System.Collections.Generic |  100000 |   470,070.8 ns |  1.00 |  400098 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **250000** | **1,075,245.9 ns** |  **0.92** |       **1000008 B** |
| System.Collections.Generic |  250000 | 1,171,422.0 ns |  1.00 | 1000140 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** |  **500000** | **2,399,918.5 ns** |  **1.03** |       **2000003 B** |
| System.Collections.Generic |  500000 | 2,343,048.8 ns |  1.00 | 2000225 B |
|                            |         |                |       |           |
|     **StackMemoryCollections** | **1000000** | **4,838,296.6 ns** |  **1.03** |       **4000007 B** |
| System.Collections.Generic | 1000000 | 4,679,167.4 ns |  1.00 | 4000393 B |

[Bench Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/PrimitiveSimpleJob.cs)
______
  
|                     Method |    Size |          Mean | Ratio |   Allocated |
|--------------------------- |-------- |--------------:|------:|------------:|
|     **StackMemoryCollections** |     **100** |      **69.06 μs** |  **1.24** |           **400** |
| System.Collections.Generic |     100 |      55.78 μs |  1.00 |     91200 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |    **1000** |     **618.52 μs** |  **1.10** |         **4001 B** |
| System.Collections.Generic |    1000 |     545.27 μs |  1.00 |    811200 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |   **10000** |   **6,352.77 μs** |  **1.18** |         **40007 B** |
| System.Collections.Generic |   10000 |   5,396.62 μs |  1.00 |   8011204 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **100000** |  **63,338.15 μs** |  **0.84** |       **400198 B** |
| System.Collections.Generic |  100000 |  76,073.95 μs |  1.00 |  80019641 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **250000** | **158,500.56 μs** |  **0.86** |       **1000368 B** |
| System.Collections.Generic |  250000 | 183,712.84 μs |  1.00 | 200029085 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** |  **500000** | **311,823.57 μs** |  **0.86** |      **2005860 B** |
| System.Collections.Generic |  500000 | 363,499.57 μs |  1.00 | 400046688 B |
|                            |         |               |       |             |
|     **StackMemoryCollections** | **1000000** | **625,418.21 μs** |  **0.85** |      **4003792 B** |
| System.Collections.Generic | 1000000 | 739,717.81 μs |  1.00 | 800078544 B |
  
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/PrimitiveOptimalJob.cs)
______
### Class:
Stack elements are classes.

[ClassSimpleJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.ClassSimpleJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/ClassSimpleJob.cs)

[ClassOptimalJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.ClassOptimalJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/ClassOptimalJob.cs)

______
### Struct:
Stack elements are structures.

[StructSimpleJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.StructSimpleJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/StructSimpleJob.cs)

[StructOptimalJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.StructOptimalJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/StructOptimalJob.cs)

______

</details>
