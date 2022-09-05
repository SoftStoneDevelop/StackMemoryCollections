using StackMemoryCollections.Attibutes;

namespace Benchmark
{
    [GenerateHelper]
    [GenerateStack]
    [GenerateQueue]
    [GenerateWrapper]
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
    [GenerateQueue]
    [GenerateWrapper]
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