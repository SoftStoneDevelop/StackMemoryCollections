using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Queue
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
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
                            using var stack = new StackMemoryCollections.Struct.QueueOfInt32((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                *stack.BackFuture() = i;
                                stack.PushFuture();
                            }

                            if(j > 50)
                            {
                                stack.Clear();
                            }
                            else
                            {
                                while (stack.TryPop())
                                {
                                }
                            }
                        }

                        using var stack2 = new StackMemoryCollections.Struct.QueueOfInt32((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            *stack2.BackFuture() = i;
                            stack2.PushFuture();
                        }

                        if (j > 50)
                        {
                            stack2.Clear();
                        }
                        else
                        {
                            while (stack2.TryPop())
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
                        var stack = new System.Collections.Generic.Queue<int>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            stack.Enqueue(i);
                        }

                        if (j > 50)
                        {
                            stack.Clear();
                        }
                        else
                        {
                            while (stack.TryDequeue(out _))
                            {
                            }
                        }
                    }

                    var stack2 = new System.Collections.Generic.Queue<int>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        stack2.Enqueue(i);
                    }

                    if (j > 50)
                    {
                        stack2.Clear();
                    }
                    else
                    {
                        while (stack2.TryDequeue(out _))
                        {
                        }
                    }
                }
            }
        }
    }
}