``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
AMD Ryzen Threadripper 3970X, 1 CPU, 64 logical and 32 physical cores
.NET SDK=6.0.400
  [Host]   : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  .NET 6.0 : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0  

```
|                                                                  Method |    Size |          Mean |        Error |       StdDev | Ratio |       Gen 0 |       Gen 1 |       Gen 2 |     Allocated |
|------------------------------------------------------------------------ |-------- |--------------:|-------------:|-------------:|------:|------------:|------------:|------------:|--------------:|
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |     **100** |      **71.63 μs** |     **0.592 μs** |     **0.525 μs** |  **1.36** |           **-** |           **-** |           **-** |             **-** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |     100 |      52.53 μs |     0.127 μs |     0.106 μs |  1.00 |     10.8643 |           - |           - |      91,200 B |
|                                                                         |         |               |              |              |       |             |             |             |               |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |    **1000** |     **638.98 μs** |     **4.568 μs** |     **3.814 μs** |  **1.23** |           **-** |           **-** |           **-** |           **1 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |    1000 |     520.24 μs |     4.454 μs |     4.166 μs |  1.00 |     96.6797 |      0.9766 |           - |     811,200 B |
|                                                                         |         |               |              |              |       |             |             |             |               |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |   **10000** |   **6,342.54 μs** |    **54.272 μs** |    **48.111 μs** |  **1.20** |           **-** |           **-** |           **-** |          **21 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |   10000 |   5,274.14 μs |    26.227 μs |    24.533 μs |  1.00 |    945.3125 |    117.1875 |           - |   8,011,204 B |
|                                                                         |         |               |              |              |       |             |             |             |               |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |  **100000** |  **63,596.95 μs** |   **168.363 μs** |   **149.249 μs** |  **0.86** |           **-** |           **-** |           **-** |          **88 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  100000 |  73,648.18 μs |   911.753 μs |   852.854 μs |  1.00 |  24857.1429 |  24857.1429 |  24857.1429 |  80,020,113 B |
|                                                                         |         |               |              |              |       |             |             |             |               |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |  **250000** | **159,569.25 μs** |   **491.859 μs** |   **436.020 μs** |  **0.87** |           **-** |           **-** |           **-** |       **1,036 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  250000 | 182,911.68 μs | 2,693.506 μs | 2,519.507 μs |  1.00 |  49666.6667 |  49666.6667 |  49666.6667 | 200,029,205 B |
|                                                                         |         |               |              |              |       |             |             |             |               |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** |  **500000** | **318,020.79 μs** | **1,211.675 μs** | **1,133.401 μs** |  **0.87** |           **-** |           **-** |           **-** |         **704 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; |  500000 | 365,538.65 μs | 5,551.763 μs | 5,193.123 μs |  1.00 |  99000.0000 |  99000.0000 |  99000.0000 | 400,044,944 B |
|                                                                         |         |               |              |              |       |             |             |             |               |
| **&#39;StackMemoryCollections.Stack&lt;T&gt;: memory = (Size*4) + Allocated column&#39;** | **1000000** | **635,504.95 μs** | **3,044.783 μs** | **2,848.092 μs** |  **0.88** |           **-** |           **-** |           **-** |       **4,248 B** |
|                             &#39;Using System.Collections.Generic.Stack&lt;T&gt;&#39; | 1000000 | 725,005.55 μs | 5,792.729 μs | 5,418.522 μs |  1.00 | 199000.0000 | 199000.0000 | 199000.0000 | 800,079,696 B |
