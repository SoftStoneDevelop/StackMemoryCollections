using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<PrimitiveSimpleJob>();
            BenchmarkRunner.Run<StructSimpleJob>();
            BenchmarkRunner.Run<ClassSimpleJob>();

            BenchmarkRunner.Run<PrimitiveOptimalJob>();
            BenchmarkRunner.Run<StructOptimalJob>();
            BenchmarkRunner.Run<ClassOptimalJob>();
        }
    }
}