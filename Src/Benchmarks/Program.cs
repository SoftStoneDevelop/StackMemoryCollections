﻿using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<StackSimpleUsageJob>();
            BenchmarkRunner.Run<StackOfStructSimpleUsageJob>();
            BenchmarkRunner.Run<StackOfClassSimpleUsageJob>();

            BenchmarkRunner.Run<StackOptimalUsageJob>();
            BenchmarkRunner.Run<StackOfStructOptimalUsageJob>();
            BenchmarkRunner.Run<StackOfClassOptimalUsageJob>();
        }
    }
}