using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [HideColumns("Error", "StdDev", "Median", "Gen0", "Gen1", "Gen2", "Alloc Ratio", "RatioSD")]
    public class PrimitiveSimpleJob
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"StackMemoryCollections")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * (nuint)Size))
                {
                    var stack = new Benchmark.Struct.StackOfInt32((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        stack.Push(in i);
                    }

                    while (stack.TryPop())
                    {
                    }
                }
            }
        }

        [Benchmark(Baseline = true, Description = "System.Collections.Generic")]
        public void SystemCollectionsStack()
        {
            unsafe
            {
                var stack = new System.Collections.Generic.Stack<int>(Size);
                for (int i = 0; i < Size; i++)
                {
                    stack.Push(i);
                }

                while (stack.TryPop(out _))
                {
                }
            }
        }
    }
}