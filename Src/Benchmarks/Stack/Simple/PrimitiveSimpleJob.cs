using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    public class PrimitiveSimpleJob
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"StackMemoryCollections.Stack<T>: memory = (Size*4) + Allocated column")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * (nuint)Size))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>((nuint)Size, &memory);
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