using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using StackMemoryCollections;
using System.ComponentModel;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [Description("Simple usage StackMemory")]
    public class StackOfStructJob2
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"Using StackMemoryCollections.StackOfStruct<T>: unmanaged memory = Size * item size(~12) bytes")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemory(SimpleStructHelper.GetSize() * (nuint)Size))
                {
                    var item = new SimpleStruct(0, 0);
                    using var stack = new StackOfSimpleStruct((nuint)Size, &memory);
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
                var stack = new System.Collections.Generic.Stack<SimpleStruct>(Size);
                for (int i = 0; i < Size; i++)
                {
                    stack.Push(new SimpleStruct(i, i * 2));
                }

                while (stack.TryPop(out _))
                {
                }
            }
        }
    }
}