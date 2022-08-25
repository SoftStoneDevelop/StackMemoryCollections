using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //simple usage custom stack
            BenchmarkRunner.Run<StackJob1>();
            BenchmarkRunner.Run<StackOfStructJob1>();
            BenchmarkRunner.Run<StackOfClassJob1>();

            //optimal usage custom stack
            BenchmarkRunner.Run<StackJob2>();
            BenchmarkRunner.Run<StackOfStructJob2>();
            BenchmarkRunner.Run<StackOfClassJob2>();
        }
    }
}