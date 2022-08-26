using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    public class ClassSimpleJob
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"Using StackOfJobClass: memory = (Size * 12) + Allocated column")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(JobClassHelper.GetSize() * (nuint)Size))
                {
                    var item = new JobClass(0, 0);
                    using var stack = new Benchmark.Struct.StackOfJobClass((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        item.Int32 = i;
                        item.Int64 = i * 2;
                        stack.Push(in item);
                    }

                    while (!stack.IsEmpty)
                    {
                        stack.Pop();
                    }
                }
            }
        }

        [Benchmark(Baseline = true, Description = "Using System.Collections.Generic.Stack<T>")]
        public void SystemCollectionsStack()
        {
            unsafe
            {
                var stack = new System.Collections.Generic.Stack<JobClass>(Size);
                for (int i = 0; i < Size; i++)
                {
                    stack.Push(new JobClass(i, i * 2));
                }

                while (stack.TryPop(out _))
                {
                }
            }
        }
    }
}