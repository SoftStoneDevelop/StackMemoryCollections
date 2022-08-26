``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
AMD Ryzen Threadripper 3970X, 1 CPU, 64 logical and 32 physical cores
.NET SDK=6.0.400
  [Host]   : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  .NET 6.0 : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0  

```
|                                                            Method |    Size |           Mean |        Error |       StdDev | Ratio |    Gen 0 |    Gen 1 |    Gen 2 |    Allocated |
|------------------------------------------------------------------ |-------- |---------------:|-------------:|-------------:|------:|---------:|---------:|---------:|-------------:|
| **&#39;Using StackOfJobStruct: memory = (Size * 12) + Allocated column&#39;** |     **100** |       **234.4 ns** |      **1.64 ns** |      **1.37 ns** |  **0.57** |        **-** |        **-** |        **-** |            **-** |
|                       &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |     100 |       414.3 ns |      3.51 ns |      3.11 ns |  1.00 |   0.1979 |   0.0010 |        - |      1,656 B |
|                                                                   |         |                |              |              |       |          |          |          |              |
| **&#39;Using StackOfJobStruct: memory = (Size * 12) + Allocated column&#39;** |    **1000** |     **1,844.5 ns** |     **15.90 ns** |     **13.27 ns** |  **0.46** |        **-** |        **-** |        **-** |            **-** |
|                       &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |    1000 |     3,984.6 ns |     51.10 ns |     47.80 ns |  1.00 |   1.9150 |   0.0992 |        - |     16,056 B |
|                                                                   |         |                |              |              |       |          |          |          |              |
| **&#39;Using StackOfJobStruct: memory = (Size * 12) + Allocated column&#39;** |   **10000** |    **17,944.6 ns** |    **112.23 ns** |    **104.98 ns** |  **0.23** |        **-** |        **-** |        **-** |            **-** |
|                       &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |   10000 |    79,386.0 ns |  1,573.58 ns |  1,683.71 ns |  1.00 |  49.9268 |  49.9268 |  49.9268 |    160,073 B |
|                                                                   |         |                |              |              |       |          |          |          |              |
| **&#39;Using StackOfJobStruct: memory = (Size * 12) + Allocated column&#39;** |  **100000** |   **393,652.3 ns** |  **2,276.58 ns** |  **1,901.05 ns** |  **0.50** |        **-** |        **-** |        **-** |            **-** |
|                       &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  100000 |   780,144.2 ns |  7,387.57 ns |  6,548.89 ns |  1.00 | 499.0234 | 499.0234 | 499.0234 |  1,600,224 B |
|                                                                   |         |                |              |              |       |          |          |          |              |
| **&#39;Using StackOfJobStruct: memory = (Size * 12) + Allocated column&#39;** |  **250000** |   **933,104.2 ns** |  **6,734.40 ns** |  **6,299.36 ns** |  **0.48** |        **-** |        **-** |        **-** |          **1 B** |
|                       &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  250000 | 1,964,762.8 ns | 38,872.17 ns | 46,274.55 ns |  1.00 | 996.0938 | 996.0938 | 996.0938 |  4,000,393 B |
|                                                                   |         |                |              |              |       |          |          |          |              |
| **&#39;Using StackOfJobStruct: memory = (Size * 12) + Allocated column&#39;** |  **500000** | **1,773,504.9 ns** |  **8,516.49 ns** |  **7,966.33 ns** |  **0.58** |        **-** |        **-** |        **-** |          **1 B** |
|                       &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  500000 | 3,069,834.0 ns | 46,839.78 ns | 43,813.96 ns |  1.00 | 265.6250 | 265.6250 | 265.6250 |  8,000,141 B |
|                                                                   |         |                |              |              |       |          |          |          |              |
| **&#39;Using StackOfJobStruct: memory = (Size * 12) + Allocated column&#39;** | **1000000** | **3,553,606.3 ns** | **35,840.88 ns** | **33,525.58 ns** |  **0.61** |        **-** |        **-** |        **-** |          **2 B** |
|                       &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; | 1000000 | 5,801,691.0 ns | 76,822.63 ns | 71,859.93 ns |  1.00 | 273.4375 | 273.4375 | 273.4375 | 16,000,145 B |
