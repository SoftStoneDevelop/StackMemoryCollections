# Readme:
Collections for memory reuse by stack type.

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

### Primitive:

[PrimitiveSimpleJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.PrimitiveSimpleJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/PrimitiveSimpleJob.cs)

[PrimitiveOptimalJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.PrimitiveOptimalJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/PrimitiveOptimalJob.cs)

______
### Class:

[ClassSimpleJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.ClassSimpleJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/ClassSimpleJob.cs)

[ClassOptimalJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.ClassOptimalJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/ClassOptimalJob.cs)

______
### Struct:

[StructSimpleJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.StructSimpleJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Simple/StructSimpleJob.cs)

[StructOptimalJob result](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/BenchmarkResults/Benchmark.StructOptimalJob-report-github.md):
[Code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Stack/Optimal/StructOptimalJob.cs)

______

</details>
