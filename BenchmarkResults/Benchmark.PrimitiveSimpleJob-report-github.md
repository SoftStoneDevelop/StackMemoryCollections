``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
AMD Ryzen Threadripper 3970X, 1 CPU, 64 logical and 32 physical cores
.NET SDK=6.0.400
  [Host]   : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  .NET 6.0 : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0  

```
|                                                                  Method |    Size |           Mean |        Error |       StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 |    Gen 2 |   Allocated |
|------------------------------------------------------------------------ |-------- |---------------:|-------------:|-------------:|------:|--------:|---------:|---------:|---------:|------------:|
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |     **100** |       **527.7 ns** |      **4.36 ns** |      **4.08 ns** |  **1.40** |    **0.01** |        **-** |        **-** |        **-** |           **-** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |     100 |       375.8 ns |      2.69 ns |      2.52 ns |  1.00 |    0.00 |   0.0544 |        - |        - |       456 B |
|                                                                         |         |                |              |              |       |         |          |          |          |             |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |    **1000** |     **4,236.5 ns** |     **18.20 ns** |     **16.13 ns** |  **1.16** |    **0.01** |        **-** |        **-** |        **-** |           **-** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |    1000 |     3,666.0 ns |     21.36 ns |     18.93 ns |  1.00 |    0.00 |   0.4845 |   0.0038 |        - |     4,056 B |
|                                                                         |         |                |              |              |       |         |          |          |          |             |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |   **10000** |    **41,277.4 ns** |    **174.37 ns** |    **163.11 ns** |  **1.12** |    **0.01** |        **-** |        **-** |        **-** |           **-** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |   10000 |    36,803.7 ns |    270.13 ns |    252.68 ns |  1.00 |    0.00 |   4.7607 |   0.5493 |        - |    40,056 B |
|                                                                         |         |                |              |              |       |         |          |          |          |             |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |  **100000** |   **428,872.8 ns** |  **1,178.61 ns** |  **1,044.81 ns** |  **0.91** |    **0.01** |        **-** |        **-** |        **-** |         **1 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  100000 |   471,078.7 ns |  5,945.06 ns |  5,561.01 ns |  1.00 |    0.00 | 124.5117 | 124.5117 | 124.5117 |   400,098 B |
|                                                                         |         |                |              |              |       |         |          |          |          |             |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |  **250000** | **1,038,286.4 ns** |  **6,591.29 ns** |  **6,165.50 ns** |  **0.90** |    **0.01** |        **-** |        **-** |        **-** |         **1 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  250000 | 1,154,430.9 ns |  6,367.06 ns |  5,955.75 ns |  1.00 |    0.00 | 248.0469 | 248.0469 | 248.0469 | 1,000,140 B |
|                                                                         |         |                |              |              |       |         |          |          |          |             |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |  **500000** | **2,384,174.8 ns** | **11,835.84 ns** |  **9,883.46 ns** |  **1.03** |    **0.01** |        **-** |        **-** |        **-** |         **3 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  500000 | 2,319,695.8 ns | 17,854.86 ns | 16,701.44 ns |  1.00 |    0.00 | 496.0938 | 496.0938 | 496.0938 | 2,000,225 B |
|                                                                         |         |                |              |              |       |         |          |          |          |             |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** | **1000000** | **4,672,641.4 ns** | **26,874.74 ns** | **25,138.65 ns** |  **1.00** |    **0.02** |        **-** |        **-** |        **-** |         **6 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; | 1000000 | 4,675,551.2 ns | 57,613.76 ns | 53,891.95 ns |  1.00 |    0.00 | 992.1875 | 992.1875 | 992.1875 | 4,000,393 B |
