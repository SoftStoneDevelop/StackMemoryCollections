using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Stack
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [HideColumns("Error", "StdDev", "Median", "Gen0", "Gen1", "Gen2", "Alloc Ratio", "RatioSD")]
    public class StructOptimalJob
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"StackOfJobStruct")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.SizeOf * (nuint)Size))
                {
                    var item = new Benchmark.Struct.JobStructWrapper(memory.Start, false);
                    var js2W = new Benchmark.Struct.JobStruct2Wrapper(memory.Start, false);
                    for (int j = 0; j < 100; j++)
                    {
                        {
                            using var stack = new Benchmark.Struct.StackOfJobStruct((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                item.ChangePtr(stack.TopFuture());
                                item.Int32 = i;
                                item.Int64 = i * 2;
                                js2W.ChangePtr(item.JobStruct2Ptr);
                                js2W.Int32 = 15;
                                js2W.Int64 = 36;
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

                        using var stack2 = new Benchmark.Struct.StackOfJobStruct((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            item.ChangePtr(stack2.TopFuture());
                            item.Int32 = i;
                            item.Int64 = i * 2;
                            js2W.ChangePtr(item.JobStruct2Ptr);
                            js2W.Int32 = 15;
                            js2W.Int64 = 36;
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
                var item = new JobStruct(0, 0);
                for (int j = 0; j < 100; j++)
                {
                    {
                        var stack = new System.Collections.Generic.Stack<JobStruct>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            item.Int32 = i;
                            item.Int64 = i * 2;
                            item.JobStruct2.Int32 = 15;
                            item.JobStruct2.Int64 = 36;
                            stack.Push(item);
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

                    var stack2 = new System.Collections.Generic.Stack<JobStruct>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        item.Int32 = i;
                        item.Int64 = i * 2;
                        item.JobStruct2.Int32 = 15;
                        item.JobStruct2.Int64 = 36;
                        stack2.Push(item);
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