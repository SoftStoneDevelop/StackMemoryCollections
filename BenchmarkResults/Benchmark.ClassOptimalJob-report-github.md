``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
AMD Ryzen Threadripper 3970X, 1 CPU, 64 logical and 32 physical cores
.NET SDK=6.0.400
  [Host]   : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  .NET 6.0 : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0  

```
|                                                           Method |    Size |            Mean |         Error |        StdDev | Ratio |       Gen 0 |       Gen 1 |       Gen 2 |       Allocated |
|----------------------------------------------------------------- |-------- |----------------:|--------------:|--------------:|------:|------------:|------------:|------------:|----------------:|
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |     **100** |        **38.12 μs** |      **0.086 μs** |      **0.080 μs** |  **0.23** |           **-** |           **-** |           **-** |            **32 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |     100 |       166.99 μs |      1.837 μs |      1.628 μs |  1.00 |     96.9238 |      0.7324 |           - |       811,200 B |
|                                                                  |         |                 |               |               |       |             |             |             |                 |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |    **1000** |       **336.29 μs** |      **1.387 μs** |      **1.297 μs** |  **0.21** |           **-** |           **-** |           **-** |            **32 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |    1000 |     1,567.58 μs |     30.730 μs |     32.880 μs |  1.00 |    955.0781 |     41.0156 |           - |     8,011,201 B |
|                                                                  |         |                 |               |               |       |             |             |             |                 |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |   **10000** |     **3,354.68 μs** |      **9.638 μs** |      **9.016 μs** |  **0.22** |           **-** |           **-** |           **-** |            **34 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |   10000 |    15,214.80 μs |    303.418 μs |    337.248 μs |  1.00 |   9515.6250 |   1921.8750 |           - |    80,011,208 B |
|                                                                  |         |                 |               |               |       |             |             |             |                 |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |  **100000** |    **33,622.04 μs** |    **257.251 μs** |    **240.633 μs** |  **0.11** |           **-** |           **-** |           **-** |            **66 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  100000 |   316,783.71 μs |  4,409.228 μs |  3,681.905 μs |  1.00 |  99000.0000 |  49000.0000 |  49000.0000 |   800,044,272 B |
|                                                                  |         |                 |               |               |       |             |             |             |                 |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |  **250000** |    **84,261.38 μs** |    **863.884 μs** |    **808.077 μs** |  **0.05** |           **-** |           **-** |           **-** |           **639 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  250000 | 1,618,548.75 μs | 30,485.382 μs | 28,516.045 μs |  1.00 | 281000.0000 | 187000.0000 |  91000.0000 | 2,000,048,072 B |
|                                                                  |         |                 |               |               |       |             |             |             |                 |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** |  **500000** |   **169,605.64 μs** |    **871.102 μs** |    **814.829 μs** |  **0.04** |           **-** |           **-** |           **-** |         **1,253 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  500000 | 4,273,696.83 μs | 57,583.870 μs | 53,863.988 μs |  1.00 | 561000.0000 | 367000.0000 | 197000.0000 | 4,000,083,760 B |
|                                                                  |         |                 |               |               |       |             |             |             |                 |
| **&#39;Using StackOfJobClass: memory = (Size * 12) + Allocated column&#39;** | **1000000** |   **341,566.02 μs** |  **1,969.334 μs** |  **3,180.107 μs** |  **0.04** |           **-** |           **-** |           **-** |           **512 B** |
|                      &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; | 1000000 | 8,379,610.78 μs | 36,288.522 μs | 33,944.306 μs |  1.00 | 864000.0000 | 464000.0000 | 166000.0000 | 8,000,090,160 B |
