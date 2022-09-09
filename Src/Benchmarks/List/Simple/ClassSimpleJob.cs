using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.List
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
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
                    using var list = new Benchmark.Struct.ListOfJobClass((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        item.SetInt32(in i);
                        item.SetInt64(i * 2);
                        var jc2 = Benchmark.JobClassHelper.GetJobClass2Ptr(item.Ptr);
                        Benchmark.JobClass2Helper.SetInt32Value(in jc2, i + 3);
                        Benchmark.JobClass2Helper.SetInt64Value(in jc2, i * 3);
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
                        new JobClass(i, i * 2)
                        {
                            JobClass2 = new JobClass2(i + 3, i * 3)
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