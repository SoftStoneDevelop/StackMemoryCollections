using System;
using System.Collections.Generic;
using StackMemoryCollections.Model;

namespace StackMemoryCollections.Helpers
{
    internal static class TypeInfoHelper
    {
        internal static void CalculateSize(
            TypeInfo typeInfo,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            typeInfo.AllPointersCount = 0;
            if (typeInfo.IsPrimitive)
            {
                typeInfo.Size = TypeToSize(typeInfo.TypeName);
                return;
            }

            typeInfo.Size = typeInfo.IsValueType ? 0 : 1;

            foreach (MemberInfo member in typeInfo.Members)
            {
                if (IsPrimitive(member.TypeName))
                {
                    typeInfo.Size += TypeToSize(member.TypeName);
                    continue;
                }

                if (member.AsPointer)
                {
                    typeInfo.AllPointersCount++;
                    continue;
                }

                if (!typeInfos.TryGetValue(member.TypeName, out var memberTypeInfo))
                {
                    throw new Exception($"{nameof(CalculateSize)}: Member Type information not found(for type '{typeInfo.TypeName}'), types filling error. Type name: {member.TypeName}");
                }

                typeInfo.AllPointersCount += memberTypeInfo.AllPointersCount;
                typeInfo.Size += memberTypeInfo.Size;
            }

            if (typeInfo.IsRuntimeCalculatedSize)
            {
                if (typeInfo.AllPointersCount > 1)
                {
                    typeInfo.SizeOf =
                        $"({(typeInfo.Size > 0 ? $"{typeInfo.Size}" : "")} + {(typeInfo.AllPointersCount > 1 ? $"(sizeof(IntPtr) * {typeInfo.AllPointersCount})" : "sizeof(IntPtr)")})";
                }
                else
                {
                    typeInfo.SizeOf = $"({typeInfo.Size} + sizeof(IntPtr))";
                }
            }
            else
            {
                typeInfo.SizeOf = $"({typeInfo.Size})";
            }
        }

        internal static void CalculateOffset(
            MemberInfo newMember,
            in TypeInfo containTypeInfo,
            in Dictionary<string, TypeInfo> typeInfos
            )
        {
            newMember.Offset = !containTypeInfo.IsValueType ? 1 : 0;

            var pointersCount = 0;
            foreach (MemberInfo member in containTypeInfo.Members)
            {
                if (member.IsPrimitive)
                {
                    newMember.Offset += TypeToSize(member.TypeName);
                    continue;
                }

                if (member.AsPointer)
                {
                    pointersCount++;
                    continue;
                }

                if (!typeInfos.TryGetValue(member.TypeName, out var memberTypeInfo))
                {
                    throw new Exception($"{nameof(CalculateOffset)}: Member Type information not found(for type '{containTypeInfo.TypeName}'), types filling error. Type name: {member.TypeName}");
                }

                newMember.Offset += memberTypeInfo.Size;
                pointersCount += memberTypeInfo.AllPointersCount;
            }

            if (pointersCount > 0)
            {
                newMember.IsRuntimeOffsetCalculated = true;
                newMember.OffsetStr =
                    $"({(newMember.Offset > 0 ? $"{newMember.Offset}" : "")} + {(pointersCount > 1 ? $"(sizeof(IntPtr) * {pointersCount})" : "sizeof(IntPtr)")})";
            }
            else
            {
                newMember.IsRuntimeOffsetCalculated = false;
                newMember.OffsetStr = $"({newMember.Offset})";
            }
        }

        internal static bool IsPrimitive(string typeName)
        {
            switch (typeName)
            {
                case "Int32":
                    {
                        return true;
                    }

                case "UInt32":
                    {
                        return true;
                    }

                case "Int64":
                    {
                        return true;
                    }

                case "UInt64":
                    {
                        return true;
                    }

                case "SByte":
                    {
                        return true;
                    }

                case "Byte":
                    {
                        return true;
                    }

                case "Int16":
                    {
                        return true;
                    }

                case "UInt16":
                    {
                        return true;
                    }

                case "Char":
                    {
                        return true;
                    }

                case "Decimal":
                    {
                        return true;
                    }

                case "Double":
                    {
                        return true;
                    }

                case "Boolean":
                    {
                        return true;
                    }

                case "Single":
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        private static int TypeToSize(string typeName)
        {
            switch (typeName)
            {
                case "Int32":
                    {
                        return sizeof(int);
                    }

                case "UInt32":
                    {
                        return sizeof(uint);
                    }

                case "Int64":
                    {
                        return sizeof(long);
                    }

                case "UInt64":
                    {
                        return sizeof(ulong);
                    }

                case "SByte":
                    {
                        return sizeof(sbyte);
                    }

                case "Byte":
                    {
                        return sizeof(byte);
                    }

                case "Int16":
                    {
                        return sizeof(short);
                    }

                case "UInt16":
                    {
                        return sizeof(ushort);
                    }

                case "Char":
                    {
                        return sizeof(char);
                    }

                case "Decimal":
                    {
                        return sizeof(decimal);
                    }

                case "Double":
                    {
                        return sizeof(double);
                    }

                case "Boolean":
                    {
                        return sizeof(bool);
                    }

                case "Single":
                    {
                        return sizeof(float);
                    }

                default:
                    {
                        throw new Exception($"TypeToSize: unknown type {typeName}");
                    }
            }
        }
    }
}