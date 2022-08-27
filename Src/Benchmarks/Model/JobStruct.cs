using GenerateAttibutes;
using System.Runtime.InteropServices;

namespace Benchmark
{
    [GenerateHelper]
    [GenerateStack]
    [GenerateWrapper]
    [StructLayout(LayoutKind.Sequential)]
    public struct JobStruct
    {
        public JobStruct(
            int int32,
            long int64
            )
        {
            Int32 = int32;
            Int64 = int64;
            JobStruct2 = default;
        }

        public long Int64;
        public int Int32;
        public JobStruct2 JobStruct2;
    }

    [GenerateHelper]
    [GenerateStack]
    [GenerateWrapper]
    [StructLayout(LayoutKind.Sequential)]
    public struct JobStruct2
    {
        public JobStruct2(
            int int32,
            long int64
            )
        {
            Int32 = int32;
            Int64 = int64;
        }

        public long Int64;
        public int Int32;
    }
}