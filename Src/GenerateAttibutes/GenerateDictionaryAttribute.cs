﻿using System;

namespace GenerateAttibutes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class GenerateDictionaryAttribute : Attribute
    {
    }
}