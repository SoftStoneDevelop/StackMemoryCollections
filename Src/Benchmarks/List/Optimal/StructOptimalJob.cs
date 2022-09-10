﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.List
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
                    var item = new Benchmark.Struct.JobClassWrapper(memory.Start, false);
                    var js2W = new Benchmark.Struct.JobClass2Wrapper(memory.Start, false);
                    for (int j = 0; j < 100; j++)
                    {
                        {
                            using var list = new Benchmark.Struct.ListOfJobStruct((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                item.ChangePtr(list.GetFuture());
                                item.SetInt32(132);
                                item.SetInt64(248);
                                js2W.ChangePtr(item.JobClass2Ptr);
                                js2W.SetInt32(15);
                                js2W.SetInt64(36);
                                list.AddFuture();
                            }

                            if(j > 50)
                            {
                                list.Clear();
                                list.TrimExcess();
                            }
                            else
                            {
                                while (list.Size != 0)
                                {
                                    list.Remove(list.Size - 1);
                                }
                            }
                        }

                        using var list2 = new Benchmark.Struct.ListOfJobStruct((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            item.ChangePtr(list2.GetFuture());
                            item.SetInt32(132);
                            item.SetInt64(248);
                            js2W.ChangePtr(item.JobClass2Ptr);
                            js2W.SetInt32(15);
                            js2W.SetInt64(36);
                            list2.AddFuture();
                        }

                        if (j > 50)
                        {
                            list2.Clear();
                        }
                        else
                        {
                            while (list2.Size != 0)
                            {
                                list2.Remove(list2.Size - 1);
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
                        var list = new System.Collections.Generic.List<JobStruct>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            item.Int32 = 132;
                            item.Int64 = 248;
                            item.JobStruct2.Int32 = 15;
                            item.JobStruct2.Int64 = 36;
                            list.Add(item);
                        }

                        if (j > 50)
                        {
                            list.Clear();
                            list.TrimExcess();
                        }
                        else
                        {
                            while (list.Count != 0)
                            {
                                list.RemoveAt(list.Count - 1);
                            }
                        }
                    }

                    var list2 = new System.Collections.Generic.List<JobStruct>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        item.Int32 = 132;
                        item.Int64 = 248;
                        item.JobStruct2.Int32 = 15;
                        item.JobStruct2.Int64 = 36;
                        list2.Add(item);
                    }

                    if (j > 50)
                    {
                        list2.Clear();
                    }
                    else
                    {
                        while (list2.Count != 0)
                        {
                            list2.RemoveAt(list2.Count - 1);
                        }
                    }
                }
            }
        }
    }
}