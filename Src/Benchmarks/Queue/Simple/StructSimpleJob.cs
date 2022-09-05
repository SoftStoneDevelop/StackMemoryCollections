using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.Queue
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
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
                    using var queue = new Benchmark.Struct.QueueOfJobStruct((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        item.Int32 = i;
                        item.Int64 = i * 2;
                        item.JobStruct2.Int32 = 15;
                        item.JobStruct2.Int64 = 36;
                        queue.Push(in item);
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
                var item = new JobStruct(0, 0);
                var queue = new System.Collections.Generic.Queue<JobStruct>(Size);
                for (int i = 0; i < Size; i++)
                {
                    item.Int32 = i;
                    item.Int64 = i * 2;
                    item.JobStruct2.Int32 = 15;
                    item.JobStruct2.Int64 = 36;
                    queue.Enqueue(item);
                }

                while (queue.TryDequeue(out _))
                {
                }
            }
        }
    }
}