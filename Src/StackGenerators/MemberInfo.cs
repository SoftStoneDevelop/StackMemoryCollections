namespace StackGenerators
{
    internal class MemberInfo
    {
        public int Offset;
        public int Size;
        public string TypeName;
        public string MemberName;
        public bool IsPrimitive;
        public bool IsValueType;
        public bool IsUnmanagedType;
    }
}