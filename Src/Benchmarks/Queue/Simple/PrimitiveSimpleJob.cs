using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Queue
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net70)]
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
                    var queue = new StackMemoryCollections.Struct.QueueOfInt32((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        queue.Push(in i);
                    }

                    while (queue.TryPop())
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
                var queue = new System.Collections.Generic.Queue<int>(Size);
                for (int i = 0; i < Size; i++)
                {
                    queue.Enqueue(i);
                }

                while (queue.TryDequeue(out _))
                {
                }
            }
        }
    }
}