using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.List
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net70)]
    [HideColumns("Error", "StdDev", "Median", "Gen0", "Gen1", "Gen2", "Alloc Ratio", "RatioSD")]
    public class ClassSimpleJob
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"StackMemoryCollections")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(JobClassHelper.SizeOf + (JobClassHelper.SizeOf * (nuint)Size)))
                {
                    var item = new Benchmark.Struct.JobClassWrapper(&memory);
                    var item2 = new Benchmark.Struct.JobClassWrapper(item.JobClass2Ptr, false);
                    using var list = new Benchmark.Struct.ListOfJobClass((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        item.SetInt32(132);
                        item.SetInt64(248);
                        item2.SetInt32(15);
                        item2.SetInt64(36);
                        list.Add(item.Ptr);
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
                var list = new System.Collections.Generic.List<JobClass>(Size);
                for (int i = 0; i < Size; i++)
                {
                    list.Add(
                        new JobClass(132, 248)
                        {
                            JobClass2 = new JobClass2(15, 36)
                        }
                        );
                }

                while (list.Count != 0)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
        }
    }
}