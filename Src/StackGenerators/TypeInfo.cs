using System.Collections.Generic;

namespace StackGenerators
{
    internal class TypeInfo
    {
        public List<MemberInfo> Members = new List<MemberInfo>();

        public bool HasIgnoredMembers;
        public bool IsPrimitive;
        public bool IsValueType;
        public bool IsUnmanagedType;
        public string ContainingNamespace;
    }
}