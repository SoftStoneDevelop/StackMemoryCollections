using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //Queue
            BenchmarkRunner.Run<Queue.PrimitiveSimpleJob>();// 4 byte * Size
            BenchmarkRunner.Run<Queue.PrimitiveOptimalJob>();// 4 byte * Size

            BenchmarkRunner.Run<Queue.StructSimpleJob>();// 24 byte * Size
            BenchmarkRunner.Run<Queue.StructOptimalJob>();// 24 byte * Size

            BenchmarkRunner.Run<Queue.ClassSimpleJob>();// 24 byte * Size
            BenchmarkRunner.Run<Queue.ClassOptimalJob>();// 24 byte * Size

            //Stack
            BenchmarkRunner.Run<Stack.PrimitiveSimpleJob>();// 4 byte * Size
            BenchmarkRunner.Run<Stack.PrimitiveOptimalJob>();// 4 byte * Size

            BenchmarkRunner.Run<Stack.StructSimpleJob>();// 24 byte * Size
            BenchmarkRunner.Run<Stack.StructOptimalJob>();// 24 byte * Size

            BenchmarkRunner.Run<Stack.ClassSimpleJob>();// 24 byte * Size
            BenchmarkRunner.Run<Stack.ClassOptimalJob>();// 24 byte * Size
        }
    }
}