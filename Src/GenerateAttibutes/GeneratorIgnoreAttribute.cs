using System;

namespace GenerateAttibutes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GeneratorIgnoreAttribute : Attribute
    {
    }
}