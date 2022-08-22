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

        public int Int32Val;
        public long Int64Val;
        public bool BoolVal;

        private int _int32Val;
        private long _int64Val;
        private bool _boolVal;
    }
}