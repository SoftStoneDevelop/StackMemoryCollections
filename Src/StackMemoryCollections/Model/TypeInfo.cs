using System.Collections.Generic;

namespace StackMemoryCollections.Model
{
    internal class TypeInfo
    {
        public List<MemberInfo> Members = new List<MemberInfo>();

        public bool HasIgnoredMembers;
        public bool IsPrimitive;
        public bool IsValueType;
        public bool IsUnmanagedType;
        public string ContainingNamespace;
        public string TypeName;

        public string SizeOf;
        public int Size;
        public int AllPointersCount;
        public bool IsRuntimeCalculatedSize => AllPointersCount > 0;
    }
}