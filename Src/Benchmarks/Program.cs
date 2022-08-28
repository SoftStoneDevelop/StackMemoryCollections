using BenchmarkDotNet.Running;
using System.Collections.Generic;
using System;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<PrimitiveSimpleJob>();// 4 byte * Size
            BenchmarkRunner.Run<StructSimpleJob>();// 24 byte * Size
            BenchmarkRunner.Run<ClassSimpleJob>();// 12 byte * Size

            BenchmarkRunner.Run<PrimitiveOptimalJob>();// 4 byte * Size
            BenchmarkRunner.Run<StructOptimalJob>();// 24 byte * Size
            BenchmarkRunner.Run<ClassOptimalJob>();// 12 byte * Size
        }
    }
}