using System.Collections.Generic;

namespace StackGenerators
{
    internal class TypeInfo
    {
        public List<MemberInfo> Members = new List<MemberInfo>();

        public bool HasIgnoredMembers;
    }
}