namespace StackMemoryCollections
{
    internal class MemberInfo
    {
        public bool IsRuntimeOffsetCalculated;
        public int Offset;
        public string OffsetStr;

        /// <summary>
        /// If is primitive then TypeName = TypeName otherwise TypeName = ContainingNamspace.TypeName
        /// </summary>
        public string TypeName;

        public string MemberName;
        public bool IsPrimitive;
        public bool IsValueType;
        public bool IsUnmanagedType;
        public bool AsPointer;
    }
}