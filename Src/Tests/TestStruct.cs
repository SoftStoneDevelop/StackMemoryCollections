using StackMemoryAttributes.Attributes;
using StackMemoryCollections.Attibutes;
using System.Collections.Generic;

namespace Tests
{
    [GenerateStack]
    [GenerateQueue]
    [GenerateList]
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
            TestClass2 = null;
        }

        public long Int64;
        public int Int32;
        public TestClass TestClass;

        [AsPointer]
        public TestClass TestClass2;
    }

    [GenerateHelper]
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

        [GeneratorIgnore]
        public Dictionary<int, string> Dictionary { get; set; }
    }
}
