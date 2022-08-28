namespace StackMemoryCollections
{
    internal class MemberInfo
    {
        public int Offset;
        public int Size;

        /// <summary>
        /// If is primitive then TypeName = TypeName otherwise TypeName = ContainingNamspace.TypeName
        /// </summary>
        public string TypeName;

        public string MemberName;
        public bool IsPrimitive;
        public bool IsValueType;
        public bool IsUnmanagedType;
    }
}