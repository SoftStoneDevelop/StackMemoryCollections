using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Queue
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
                    using var queue = new Benchmark.Struct.QueueOfJobClass((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        item.SetInt32(in i);
                        item.SetInt64(i * 2);
                        var jc2 = Benchmark.JobClassHelper.GetJobClass2Ptr(item.Ptr);
                        Benchmark.JobClass2Helper.SetInt32Value(in jc2, i + 3);
                        Benchmark.JobClass2Helper.SetInt64Value(in jc2, i * 3);
                        queue.Push(item.Ptr);
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
                var queue = new System.Collections.Generic.Queue<JobClass>(Size);
                for (int i = 0; i < Size; i++)
                {
                    queue.Enqueue(
                        new JobClass(i, i * 2)
                        {
                            JobClass2 = new JobClass2(i + 3, i * 3)
                        }
                        );
                }

                while (queue.TryDequeue(out _))
                {
                }
            }
        }
    }
}