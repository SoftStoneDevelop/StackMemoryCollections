using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark
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
                using (var memory = new StackMemoryCollections.Struct.StackMemory(JobClassHelper.GetSize() + (JobClassHelper.GetSize() * (nuint)Size)))
                {
                    var item = new Benchmark.Struct.JobClassWrapper(&memory);
                    using var stack = new Benchmark.Struct.StackOfJobClass((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        item.Int32 = i;
                        item.Int64 = i * 2;
                        var jc2 = Benchmark.JobClassHelper.GetJobClass2Ptr(item.Ptr);
                        Benchmark.JobClass2Helper.SetInt32Value(in jc2, i + 3);
                        Benchmark.JobClass2Helper.SetInt64Value(in jc2, i * 3);
                        stack.Push(item.Ptr);
                    }

                    while (stack.TryPop())
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

                while (stack.TryPop(out _))
                {
                }
            }
        }
    }
}