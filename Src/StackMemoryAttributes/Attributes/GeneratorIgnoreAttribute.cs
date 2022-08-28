using System;

namespace StackMemoryCollections.Attibutes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GeneratorIgnoreAttribute : Attribute
    {
    }
}