﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using StackMemoryCollections;
using System.ComponentModel;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [Description("Optimal usage StackMemory")]
    public class StackJob1
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
                    for (int j = 0; j < 100; j++)
                    {
                        {
                            using var stack = new Stack<int>((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                stack.Push(in i);
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

                        using var stack2 = new Stack<int>((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            stack2.Push(in i);
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
                        var stack = new System.Collections.Generic.Stack<int>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            stack.Push(i);
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

                    var stack2 = new System.Collections.Generic.Stack<int>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        stack2.Push(i);
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