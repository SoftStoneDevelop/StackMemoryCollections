using StackMemoryCollections.Attibutes;

namespace Benchmark
{
    [GenerateHelper]
    [GenerateStack]
    [GenerateWrapper]
    public class JobClass
    {
        public JobClass()
        {

        }

        public JobClass(
            int int32,
            long int64
            )
        {
            Int32 = int32;
            Int64 = int64;
        }

        public long Int64;
        public int Int32;

        public JobClass2 JobClass2;
    }

    [GenerateHelper]
    [GenerateStack]
    [GenerateWrapper]
    public class JobClass2
    {
        public JobClass2()
        {

        }

        public JobClass2(
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