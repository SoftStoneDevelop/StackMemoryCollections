using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using StackMemoryCollections;
using System.Runtime.InteropServices;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    public class StackOfStructJob
    {
        private struct SimpleStruct
        {
            public SimpleStruct(
                int int32,
                long int64
                )
            {
                Int32 = int32;
                Int64 = int64;
            }

            public long Int64;
            public int Int32;
        }

        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"Using StackMemoryCollections.StackOfStruct<T>: unmanaged memory = Size * item size(~12) bytes")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemory((nuint)Marshal.SizeOf<SimpleStruct>() * (nuint)Size))
                {
                    for (int j = 0; j < 100; j++)
                    {
                        {
                            using var stack = new StackOfStruct<SimpleStruct>((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                stack.Push(new SimpleStruct(i, i * 2));
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

                        using var stack2 = new StackOfStruct<SimpleStruct>((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            stack2.Push(new SimpleStruct(i, i * 2));
                        }

                        while (!stack2.IsEmpty)
                        {
                            stack2.Pop();
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
                        var stack = new System.Collections.Generic.Stack<SimpleStruct>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            stack.Push(new SimpleStruct(i, i * 2));
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

                    var stack2 = new System.Collections.Generic.Stack<SimpleStruct>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        stack2.Push(new SimpleStruct(i, i * 2));
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