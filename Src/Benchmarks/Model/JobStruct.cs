using GenerateAttibutes;

namespace Benchmark
{
    [GenerateHelper]
    [GenerateStack]
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
        }

        public long Int64;
        public int Int32;
    }
}