﻿using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<StackJob>();
            BenchmarkRunner.Run<StackOfStructJob>();
        }
    }
}