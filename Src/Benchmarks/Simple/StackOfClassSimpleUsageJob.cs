using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    public class StackOfClassSimpleUsageJob
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
                    for (int j = 0; j < 100; j++)
                    {
                        {
                            using var stack = new Benchmark.Struct.StackOfJobClass((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                item.Int32 = i;
                                item.Int64 = i * 2;
                                stack.Push(in item);
                            }

                            if(j > 50)
                            {
                                stack.Clear();
                            }
                            else
                            {
                                while (!stack.IsEmpty)
                                {
                                    stack.Pop();
                                }
                            }
                        }

                        using var stack2 = new Benchmark.Struct.StackOfJobClass((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            item.Int32 = i;
                            item.Int64 = i * 2;
                            stack2.Push(in item);
                        }

                        if (j > 50)
                        {
                            stack2.Clear();
                        }
                        else
                        {
                            while (!stack2.IsEmpty)
                            {
                                stack2.Pop();
                            }
                        }
                    }
                }
            }
        }

        [Benchmark(Baseline = true, Description = "Using System.Collections.Generic.Stack<T>")]
        public void SystemCollectionsStack()
        {
            unsafe
            {
                for (int j = 0; j < 100; j++)
                {
                    {
                        var stack = new System.Collections.Generic.Stack<JobClass>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            stack.Push(new JobClass(i, i * 2));
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

                    var stack2 = new System.Collections.Generic.Stack<JobClass>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        stack2.Push(new JobClass(i, i * 2));
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