using StackMemoryCollections.Attibutes;

namespace Tests
{
    [GenerateHelper]
    [GenerateStack]
    [GenerateWrapper]
    public struct TestStruct
    {
        public TestStruct(
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
