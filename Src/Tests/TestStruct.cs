using System.Runtime.InteropServices;

namespace Tests
{
    internal struct TestStruct
    {
        public TestStruct(
            int int32Val,
            long int64Val,
            bool boolVal
            )
        {
            Int32Val = int32Val;
            Int64Val = int64Val;
            BoolVal = boolVal;

            _int32Val = int32Val + 1;
            _int64Val = int64Val + 1;
            _boolVal = !boolVal;
        }

        public long Int64Val;
        private long _int64Val;
        public int Int32Val;
        private int _int32Val;
        public bool BoolVal;
        private bool _boolVal;
    }
}