using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using StackMemoryCollections;
using System.ComponentModel;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [Description("Simple usage StackMemory")]
    public class StackJob2
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"Using StackMemoryCollections.Stack<T>: unmanaged memory = Size*4 bytes")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemory(sizeof(int) * (nuint)Size))
                {
                    var stack = new Stack<int>((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        stack.Push(in i);
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
                var stack = new System.Collections.Generic.Stack<int>(Size);
                for (int i = 0; i < Size; i++)
                {
                    stack.Push(i);
                }

                while (stack.TryPop(out _))
                {
                }
            }
        }
    }
}