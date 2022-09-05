using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Queue
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
                            using var queue = new Benchmark.Struct.QueueOfJobStruct((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                item.ChangePtr(queue.BackFuture());
                                item.SetInt32(in i);
                                item.SetInt64(i * 2);
                                js2W.ChangePtr(item.JobStruct2Ptr);
                                js2W.SetInt32(15);
                                js2W.SetInt64(36);
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

                        using var queue2 = new Benchmark.Struct.QueueOfJobStruct((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            item.ChangePtr(queue2.BackFuture());
                            item.SetInt32(in i);
                            item.SetInt64(i * 2);
                            js2W.ChangePtr(item.JobStruct2Ptr);
                            js2W.SetInt32(15);
                            js2W.SetInt64(36);
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
                var item = new JobStruct(0, 0);
                for (int j = 0; j < 100; j++)
                {
                    {
                        var queue = new System.Collections.Generic.Queue<JobStruct>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            item.Int32 = i;
                            item.Int64 = i * 2;
                            item.JobStruct2.Int32 = 15;
                            item.JobStruct2.Int64 = 36;
                            queue.Enqueue(item);
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

                    var queue2 = new System.Collections.Generic.Queue<JobStruct>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        item.Int32 = i;
                        item.Int64 = i * 2;
                        item.JobStruct2.Int32 = 15;
                        item.JobStruct2.Int64 = 36;
                        queue2.Enqueue(item);
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