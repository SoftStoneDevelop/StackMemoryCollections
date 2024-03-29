﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.List
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net70)]
    [HideColumns("Error", "StdDev", "Median", "Gen0", "Gen1", "Gen2", "Alloc Ratio", "RatioSD")]
    public class StructSimpleJob
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"StackMemoryCollections")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(JobStructHelper.SizeOf * (nuint)Size))
                {
                    var item = new JobStruct(0, 0);
                    using var list = new Benchmark.Struct.ListOfJobStruct((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        item.Int32 = 132;
                        item.Int64 = 248;
                        item.JobStruct2.Int32 = 15;
                        item.JobStruct2.Int64 = 36;
                        list.Add(in item);
                    }

                    while (list.Size != 0)
                    {
                        list.Remove(list.Size - 1);
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
                var list = new System.Collections.Generic.List<JobStruct>(Size);
                for (int i = 0; i < Size; i++)
                {
                    item.Int32 = 132;
                    item.Int64 = 248;
                    item.JobStruct2.Int32 = 15;
                    item.JobStruct2.Int64 = 36;
                    list.Add(item);
                }

                while (list.Count != 0)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
        }
    }
}