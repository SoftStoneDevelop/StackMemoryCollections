using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark.List
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [HideColumns("Error", "StdDev", "Median", "Gen0", "Gen1", "Gen2", "Alloc Ratio", "RatioSD")]
    public class PrimitiveSimpleJob
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
                    var list = new StackMemoryCollections.Struct.ListOfInt32((nuint)Size, &memory);
                    for (int i = 0; i < Size; i++)
                    {
                        list.Add(in i);
                    }

                    while (list.Size != 0)
                    {
                        list.Remove(list.Size - 1);
                    }
                }
            }
        }

        [Benchmark(Baseline = true, Description = "System.Collections.Generic")]
        public void SystemCollectionsStack()
        {
            unsafe
            {
                var list = new System.Collections.Generic.List<int>(Size);
                for (int i = 0; i < Size; i++)
                {
                    list.Add(i);
                }

                while (list.Count != 0)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
        }
    }
}