using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<StackJob1>();
            BenchmarkRunner.Run<StackOfStructJob1>();
            BenchmarkRunner.Run<StackJob2>();
            BenchmarkRunner.Run<StackOfStructJob2>();
        }
    }
}