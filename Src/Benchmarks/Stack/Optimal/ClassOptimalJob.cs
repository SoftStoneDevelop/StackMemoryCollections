using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Stack
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [HideColumns("Error", "StdDev", "Median", "Gen0", "Gen1", "Gen2", "Alloc Ratio", "RatioSD")]
    public class ClassOptimalJob
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"StackMemoryCollections")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(JobClassHelper.SizeOf * (nuint)Size))
                {
                    var item = new Benchmark.Struct.JobClassWrapper(memory.Start, false);
                    var js2W = new Benchmark.Struct.JobClass2Wrapper(memory.Start, false);
                    for (int j = 0; j < 100; j++)
                    {
                        {
                            using var stack = new Benchmark.Struct.StackOfJobClass((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                item.ChangePtr(stack.TopFuture());
                                item.SetInt32(in i);
                                item.SetInt64(i * 2);
                                js2W.ChangePtr(item.JobClass2Ptr);
                                js2W.SetInt32(i + 3);
                                js2W.SetInt64(i * 3);
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

                        using var stack2 = new Benchmark.Struct.StackOfJobClass((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            item.ChangePtr(stack2.TopFuture());
                            item.SetInt32(in i);
                            item.SetInt64(i * 2);
                            js2W.ChangePtr(item.JobClass2Ptr);
                            js2W.SetInt32(i + 3);
                            js2W.SetInt64(i * 3);
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
                        var stack = new System.Collections.Generic.Stack<JobClass>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            stack.Push(
                                new JobClass(i, i * 2)
                                {
                                    JobClass2 = new JobClass2(i + 3, i * 3)
                                }
                                );
                        }

                        if (j > 50)
                        {
                            stack.Clear();
                        }
                        else
                        {
                            while (stack.TryPop(out _))
                            {
                            }
                        }
                    }

                    var stack2 = new System.Collections.Generic.Stack<JobClass>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        stack2.Push(
                            new JobClass(i, i * 2)
                            {
                                JobClass2 = new JobClass2(i + 3, i * 3)
                            }
                            );
                    }

                    if (j > 50)
                    {
                        stack2.Clear();
                    }
                    else
                    {
                        while (stack2.TryPop(out _))
                        {
                        }
                    }
                }
            }
        }
    }
}