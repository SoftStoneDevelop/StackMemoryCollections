using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.List
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net70)]
    [HideColumns("Error", "StdDev", "Median", "Gen0", "Gen1", "Gen2", "Alloc Ratio", "RatioSD")]
    public class PrimitiveOptimalJob
    {
        [Params(100, 1000, 10000, 100000, 250000, 500000, 1000000)]
        public int Size;

        [Benchmark(Description = $"StackMemoryCollections")]
        public void StackMemory()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * (nuint)Size))
                {
                    for (int j = 0; j < 100; j++)
                    {
                        {
                            using var list = new StackMemoryCollections.Struct.ListOfInt32((nuint)Size, &memory);
                            for (int i = 0; i < Size; i++)
                            {
                                *list.GetFuture() = i;
                                list.AddFuture();
                            }

                            if(j > 50)
                            {
                                list.Clear();
                                list.TrimExcess();
                            }
                            else
                            {
                                while (list.Size != 0)
                                {
                                    list.Remove(list.Size - 1);
                                }
                            }
                        }

                        using var list2 = new StackMemoryCollections.Struct.ListOfInt32((nuint)Size, &memory);
                        for (int i = 0; i < Size; i++)
                        {
                            *list2.GetFuture() = i;
                            list2.AddFuture();
                        }

                        if (j > 50)
                        {
                            list2.Clear();
                        }
                        else
                        {
                            while (list2.Size != 0)
                            {
                                list2.Remove(list2.Size - 1);
                            }
                        }
                    }
                }
            }
        }

        [Benchmark(Baseline = true, Description = "System.Collections.Generic")]
        public void SystemCollectionsStack()
        {
            unsafe
            {
                for (int j = 0; j < 100; j++)
                {
                    {
                        var list = new System.Collections.Generic.List<int>(Size);
                        for (int i = 0; i < Size; i++)
                        {
                            list.Add(i);
                        }

                        if (j > 50)
                        {
                            list.Clear();
                            list.TrimExcess();
                        }
                        else
                        {
                            while (list.Count != 0)
                            {
                                list.RemoveAt(list.Count - 1);
                            }
                        }
                    }

                    var list2 = new System.Collections.Generic.List<int>(Size);
                    for (int i = 0; i < Size; i++)
                    {
                        list2.Add(i);
                    }

                    if (j > 50)
                    {
                        list2.Clear();
                    }
                    else
                    {
                        while (list2.Count != 0)
                        {
                            list2.RemoveAt(list2.Count - 1);
                        }
                    }
                }
            }
        }
    }
}