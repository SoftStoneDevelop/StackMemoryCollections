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
            long int64,
            TestClass testClass
            )
        {
            Int32 = int32;
            Int64 = int64;
            TestClass = testClass;
        }

        public long Int64;
        public int Int32;
        public TestClass TestClass;
    }

    [GenerateHelper]
    [GenerateStack]
    [GenerateWrapper]
    public class TestClass
    {
        public TestClass()
        {
        }

        public TestClass(
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
