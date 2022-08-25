namespace StackMemoryCollections
{
    internal static class TypeHelper
    {
        public static bool IsPrimitive<T>()
        {
            var type = typeof(T);
            if(type.Namespace != nameof(System))
            {
                return false;
            }

            switch (type.Name)
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

                case "System.Char":
                {
                    return true;
                }

                case "System.Double":
                {
                    return true;
                }

                case "System.Boolean":
                {
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}