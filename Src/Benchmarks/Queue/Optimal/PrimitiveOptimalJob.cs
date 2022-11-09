using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Queue
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net70)]
    [HideColumns("Error", "StdDev", "Median", "Gen0", "Gen1", "Gen2", "Alloc Ratio", "RatioSD")]
    public class PrimitiveOptimalJob
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
                    for (int j = 0; j < 100; j++)
                    {
                        {
                            using var queue = new StackMemoryCollections.Struct.QueueOfInt32((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                *queue.BackFuture() = i;
                                queue.PushFuture();
                            }

                            if(j > 50)
                            {
                                queue.Clear();
                            }
                            else
                            {
                                while (queue.TryPop())
                                {
                                }
                            }
                        }

                        using var queue2 = new StackMemoryCollections.Struct.QueueOfInt32((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            *queue2.BackFuture() = i;
                            queue2.PushFuture();
                        }

                        if (j > 50)
                        {
                            queue2.Clear();
                        }
                        else
                        {
                            while (queue2.TryPop())
                            {
                            }
                        }
                    }
                }
            }
        }

        [Benchmark(Baseline = true, Description = "System.Collections.Generic")]
        public void SystemCollectionsStack()
        {
            unsafe
            {
                for (int j = 0; j < 100; j++)
                {
                    {
                        var queue = new System.Collections.Generic.Queue<int>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            queue.Enqueue(i);
                        }

                        if (j > 50)
                        {
                            queue.Clear();
                        }
                        else
                        {
                            while (queue.TryDequeue(out _))
                            {
                            }
                        }
                    }

                    var queue2 = new System.Collections.Generic.Queue<int>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        queue2.Enqueue(i);
                    }

                    if (j > 50)
                    {
                        queue2.Clear();
                    }
                    else
                    {
                        while (queue2.TryDequeue(out _))
                        {
                        }
                    }
                }
            }
        }
    }
}