using System;
using System.Collections.Generic;
using System.Linq;

namespace StackGenerators
{
    internal class TypeInfo
    {
        public List<MemberInfo> Members = new List<MemberInfo>();

        public bool HasIgnoredMembers;

        public bool IsStructLayoutSequential = true;

        public bool AllIsStructLayoutSequential(in Dictionary<string, TypeInfo> typeInfos)
        {
            var allMembers = new Queue<MemberInfo>(Members.Where(wh => !wh.IsPrimitive && wh.IsValueType));

            while(allMembers.Count != 0)
            {
                var member = allMembers.Dequeue();
                if (!typeInfos.TryGetValue(member.TypeName, out var typeInfo))
                {
                    throw new Exception($"Type information not found, types filling error. Type name: {member.TypeName}");
                }

                if (!typeInfo.IsStructLayoutSequential)
                {
                    return false;
                }

                foreach (var item in typeInfo.Members.Where(wh => !wh.IsPrimitive && wh.IsValueType))
                {
                    allMembers.Enqueue(item);
                }
            }

            return true;
        }
    }
}