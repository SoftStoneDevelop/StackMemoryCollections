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

### Stack where the elements of a collection are primitive types
StackSimpleUsageJob([code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Simple/StackSimpleUsageJob.cs))
![StackSimpleUsageJob](/BenchmarkResults/StackSimpleUsageJob.png)

StackOptimalUsageJob([code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Optimal/StackOptimalUsageJob.cs))
![StackOptimalUsageJob](/BenchmarkResults/StackOptimalUsageJob.png)
_______________________________________________

### Stack where the elements of a collection are structures
StackOfStructSimpleUsageJob([code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Simple/StackOfStructSimpleUsageJob.cs))
![example](/BenchmarkResults/StackOfStructSimpleUsageJob.png)

StackOfStructOptimalUsageJob([code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Optimal/StackOfStructOptimalUsageJob.cs))
![example](/BenchmarkResults/StackOfStructOptimalUsageJob.png)

_______________________________________________
### Stack where collection elements are classes
StackOfClassSimpleUsageJob([code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Simple/StackOfClassSimpleUsageJob.cs))
![example](/BenchmarkResults/StackOfClassSimpleUsageJob.png)

StackOfClassOptimalUsageJob([code](https://github.com/SoftStoneDevelop/StackMemoryCollections/blob/main/Src/Benchmarks/Optimal/StackOfClassOptimalUsageJob.cs))
![example](/BenchmarkResults/StackOfClassOptimalUsageJob.png)
  
</details>
