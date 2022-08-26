``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
AMD Ryzen Threadripper 3970X, 1 CPU, 64 logical and 32 physical cores
.NET SDK=6.0.400
  [Host]   : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  .NET 6.0 : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0  

```
|                                                           Method |    Size |            Mean |         Error |        StdDev | Ratio |     Gen 0 |     Gen 1 |    Gen 2 |    Allocated |
|----------------------------------------------------------------- |-------- |----------------:|--------------:|--------------:|------:|----------:|----------:|---------:|-------------:|
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |     **100** |        **295.4 ns** |       **2.31 ns** |       **2.05 ns** |  **0.30** |    **0.0038** |         **-** |        **-** |         **32 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |     100 |        991.2 ns |      12.31 ns |      10.92 ns |  1.00 |    0.4845 |    0.0038 |        - |      4,056 B |
|                                                                  |         |                 |               |               |       |           |           |          |              |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |    **1000** |      **1,997.2 ns** |       **5.34 ns** |       **4.46 ns** |  **0.21** |    **0.0038** |         **-** |        **-** |         **32 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |    1000 |      9,459.7 ns |     185.34 ns |     198.31 ns |  1.00 |    4.7760 |    0.1984 |        - |     40,056 B |
|                                                                  |         |                 |               |               |       |           |           |          |              |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |   **10000** |     **19,270.2 ns** |      **29.05 ns** |      **27.17 ns** |  **0.20** |         **-** |         **-** |        **-** |         **32 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |   10000 |     94,141.2 ns |   1,846.89 ns |   1,896.62 ns |  1.00 |   47.6074 |    9.6436 |        - |    400,056 B |
|                                                                  |         |                 |               |               |       |           |           |          |              |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |  **100000** |    **410,873.8 ns** |   **2,279.12 ns** |   **2,131.89 ns** |  **0.21** |         **-** |         **-** |        **-** |         **32 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  100000 |  1,934,069.5 ns |  13,435.90 ns |  11,910.57 ns |  1.00 |  498.0469 |  248.0469 | 248.0469 |  4,000,223 B |
|                                                                  |         |                 |               |               |       |           |           |          |              |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |  **250000** |    **975,188.1 ns** |   **5,566.24 ns** |   **5,206.66 ns** |  **0.12** |         **-** |         **-** |        **-** |         **33 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  250000 |  8,421,141.3 ns | 167,198.48 ns | 192,546.01 ns |  1.00 | 1406.2500 |  937.5000 | 468.7500 | 10,000,284 B |
|                                                                  |         |                 |               |               |       |           |           |          |              |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |  **500000** |  **1,886,875.4 ns** |  **31,840.86 ns** |  **29,783.96 ns** |  **0.09** |         **-** |         **-** |        **-** |         **33 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  500000 | 20,565,679.7 ns | 402,784.61 ns | 705,444.76 ns |  1.00 | 2687.5000 | 1718.7500 | 906.2500 | 20,000,394 B |
|                                                                  |         |                 |               |               |       |           |           |          |              |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** | **1000000** |  **3,750,880.4 ns** |  **28,059.33 ns** |  **26,246.72 ns** |  **0.08** |         **-** |         **-** |        **-** |         **34 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; | 1000000 | 44,325,185.7 ns | 843,689.49 ns | 747,908.76 ns |  1.00 | 4500.0000 | 2583.3333 | 916.6667 | 40,000,422 B |
