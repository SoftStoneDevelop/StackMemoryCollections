using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System;
using System.Text;
using System.Globalization;

namespace TestGenerator
{
    [Generator]
    public partial class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var builder = new StringBuilder();
            GenerateWrapPrimitiveTest(
                in context,
                in builder
                );

            GenerateStackPrimitiveTest(
                in context,
                in builder
                );

            GenerateQueuePrimitiveTest(
                in context,
                in builder
                );

            GenerateListPrimitiveTest(
                in context,
                in builder
                );
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            Int32Values = new List<Int32> { 15, -45, 0, 34, -140 };
            Int32Convert = (val) => { return val.ToString().ToLowerInvariant(); };
            UInt32Values = new List<UInt32> { 15, 45, 0, 34, 140 };
            UInt32Convert = (val) => { return val.ToString().ToLowerInvariant(); };
            Int64Values = new List<Int64> { 15, -45, 0, 34, -140 };
            Int64Convert = (val) => { return val.ToString().ToLowerInvariant(); };
            UInt64Values = new List<UInt64> { 15, 45, 0, 34, 140 };
            UInt64Convert = (val) => { return val.ToString().ToLowerInvariant(); };
            SByteValues = new List<SByte> { 15, -45, 0, -120, 15 };
            SByteConvert = (val) => { return val.ToString().ToLowerInvariant(); };
            ByteValues = new List<Byte> { 15, 45, 0, 255, 78 };
            ByteConvert = (val) => { return val.ToString().ToLowerInvariant(); };
            Int16Values = new List<Int16> { 15, -45, 0, -255, 120 };
            Int16Convert = (val) => { return val.ToString().ToLowerInvariant(); };
            UInt16Values = new List<UInt16> { 15, 45, 0, 255, 15 };
            UInt16Convert = (val) => { return val.ToString().ToLowerInvariant(); };
            CharValues = new List<Char> { 's', 'a', 'q', ' ', '1' };
            CharConvert = (val) => { return "'" + val.ToString() + "'"; };
            DecimalValues = new List<Decimal> { 4.5m, 0.44m, -0.5m, 0m, 0.23m };
            DecimalConvert = (val) => { return val.ToString("G") + "m"; };
            DoubleValues = new List<Double> { 4.5d, 0.44d, -0.5d, 0d, 0.23d };
            DoubleConvert = (val) => { return val.ToString("G") + "d"; };
            BooleanValues = new List<Boolean> { true, false, true, false, true };
            BooleanConvert = (val) => { return val.ToString().ToLowerInvariant(); };
            SingleValues = new List<Single> { 4.5f, 0.44f, -0.5f, 0f, 0.23f };
            SingleConvert = (val) => { return val.ToString("F", CultureInfo.InvariantCulture) + "f"; };
        }

        List<Int32> Int32Values;
        Func<Int32, string> Int32Convert;
            
        List<UInt32> UInt32Values;
        Func<UInt32, string> UInt32Convert;

        List<Int64> Int64Values;
        Func<Int64, string> Int64Convert;

        List<UInt64> UInt64Values;
        Func<UInt64, string> UInt64Convert;

        List<SByte> SByteValues;
        Func<SByte, string> SByteConvert;

        List<Byte> ByteValues;
        Func<Byte, string> ByteConvert;

        List<Int16> Int16Values;
        Func<Int16, string> Int16Convert;

        List<UInt16> UInt16Values;
        Func<UInt16, string> UInt16Convert;

        List<Char> CharValues;
        Func<Char, string> CharConvert;

        List<Decimal> DecimalValues;
        Func<Decimal, string> DecimalConvert;

        List<Double> DoubleValues;
        Func<Double, string> DoubleConvert;

        List<Boolean> BooleanValues;
        Func<Boolean, string> BooleanConvert;

        List<Single> SingleValues;
        Func<Single, string> SingleConvert;
    }
}