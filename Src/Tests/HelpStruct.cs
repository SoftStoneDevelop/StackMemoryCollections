using StackMemoryAttributes.Attributes;
using StackMemoryCollections.Attibutes;
using System.Collections.Generic;

namespace Tests
{
    [GenerateHelper]
    [GenerateWrapper]
    public struct HelpStruct
    {
        public HelpStruct(
            int int32,
            long int64,
            HelpClass helpClass,
            HelpClass helpClass2
            )
        {
            Int32 = int32;
            Int64 = int64;
            HelpClass = helpClass;
            HelpClass2 = helpClass2;
        }

        public long Int64;
        public int Int32;

        public HelpClass HelpClass;

        [AsPointer]
        public HelpClass HelpClass2 { get; set; }
    }

    [GenerateHelper]
    [GenerateWrapper]
    public struct HelpStruct2
    {
        public HelpStruct2(
            int int32,
            long int64
            )
        {
            Int32 = int32;
            Int64 = int64;
        }

        public long Int64 { get; set; }
        public int Int32;
    }

    [GenerateHelper]
    [GenerateWrapper]
    public class HelpClass
    {
        public HelpClass()
        {
        }

        public HelpClass(
            int int32,
            long int64,
            HelpStruct2 helpStruct2
            )
        {
            Int32 = int32;
            Int64 = int64;
            HelpStruct2 = helpStruct2;
        }

        public long Int64;

        [AsPointer]
        public HelpClass HelpClass2 { get; set; }

        public int Int32 { get; set; }

        public HelpStruct2 HelpStruct2 { get; set; }

        [GeneratorIgnore]
        public Dictionary<int, string> Dictionary { get; set; }
    }
}