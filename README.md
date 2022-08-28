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

[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/PrimitiveSimpleJob.cs)
______
  

  
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
