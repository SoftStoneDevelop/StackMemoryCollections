using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using System;
using System.Globalization;

namespace TestGenerator
{
    public partial class Generator
    {
        private void GenerateWrapPrimitiveTest(
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            {
                var Int32Values = new List<Int32> { 15, -45, 0, 34, -140 };
                WrapPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                WrapPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var UInt32Values = new List<UInt32> { 15, 45, 0, 34, 140 };
                WrapPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                WrapPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var Int64Values = new List<Int64> { 15, -45, 0, 34, -140 };
                WrapPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                WrapPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var UInt64Values = new List<UInt64> { 15, 45, 0, 34, 140 };
                WrapPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                WrapPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var SByteValues = new List<SByte> { 15, -45, 0, -120, 15 };
                WrapPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                WrapPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var ByteValues = new List<Byte> { 15, 45, 0, 255, 78 };
                WrapPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                WrapPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var Int16Values = new List<Int16> { 15, -45, 0, -255, 120 };
                WrapPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                WrapPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var UInt16Values = new List<UInt16> { 15, 45, 0, 255, 15 };
                WrapPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                WrapPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var CharValues = new List<Char> { 's', 'a', 'q', ' ', '1' };
                WrapPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Class",
                    (val) => { return "'" + val.ToString() + "'"; }
                    );

                WrapPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Struct",
                    (val) => { return "'" + val.ToString() + "'"; }
                    );
            }

            {
                var DecimalValues = new List<Decimal> { 4.5m, 0.44m, -0.5m, 0m, 0.23m };
                WrapPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString("G") + "m"; }
                    );

                WrapPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString("G") + "m"; }
                    );
            }

            {
                var DoubleValues = new List<Double> { 4.5d, 0.44d, -0.5d, 0d, 0.23d };
                WrapPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString("G") + "d"; }
                    );

                WrapPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString("G") + "d"; }
                    );
            }

            {
                var BooleanValues = new List<Boolean> { true, false, true, false, true };
                WrapPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                WrapPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var SingleValues = new List<Single> { 4.5f, 0.44f, -0.5f, 0f, 0.23f };
                WrapPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString("F", CultureInfo.InvariantCulture) + "f"; }
                    );

                WrapPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString("F",CultureInfo.InvariantCulture) + "f"; }
                    );
            }
        }

        private void WrapPrimitiveTest<T>(
            in GeneratorExecutionContext context,
            in List<T> values,
            in StringBuilder builder,
            in string wrapperNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Clear();
            WrapPrimitiveStart<T>(in builder, in wrapperNamespace);

            //generate methods
            WrapPrimitiveDispose(in values, in builder, in wrapperNamespace);
            WrapPrimitiveNotDispose(in values, in builder, in wrapperNamespace);
            WrapPrimitiveValue(in values, in builder, in wrapperNamespace, in toStr);

            WrapPrimitiveEnd(in builder);
            
            context.AddSource($"Wrapper{wrapperNamespace}{typeof(T).Name}Fixture.g.cs", builder.ToString());
        }

        private void WrapPrimitiveStart<T>(
            in StringBuilder builder,
            in string wrapperNamespace
            ) where T : unmanaged
        {
            builder.Append($@"
using NUnit.Framework;
using System;

namespace Tests
{{
    [TestFixture]
    public class Wrapper{wrapperNamespace}{typeof(T).Name}Fixture
    {{
                    
");
        }

        private void WrapPrimitiveDispose<T>(
            in List<T> values,
            in StringBuilder builder,
            in string wrapperNamespace
            ) where T : unmanaged
        {
            builder.Append($@"
        [Test]
        public void DisposeTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    
");
            for (int i = 0; i < values.Count - 1; i++)
            {
                builder.Append($@"
                    {{
                        using var {typeof(T).Name}W{i} = new StackMemoryCollections.{wrapperNamespace}.Wrapper<{typeof(T).Name}>(&memory);
                    }}
");
            }

            builder.Append($@"

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                }}
            }}
        }}

");
        }

        private void WrapPrimitiveNotDispose<T>(
            in List<T> values,
            in StringBuilder builder,
            in string wrapperNamespace
            ) where T : unmanaged
        {
            builder.Append($@"

        [Test]
        public void NotDisposeTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    
");
            for (int i = 0; i < values.Count - 1; i++)
            {
                builder.Append($@"

                    {{
                        var {typeof(T).Name}W{i} = new StackMemoryCollections.{wrapperNamespace}.Wrapper<{typeof(T).Name}>(&memory);
                    }}
");
            }

            builder.Append($@"

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (sizeof({typeof(T).Name}) * {values.Count - 1}))));
                }}
            }}
        }}

");
        }

        private void WrapPrimitiveValue<T>(
            in List<T> values,
            in StringBuilder builder,
            in string wrapperNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            builder.Append($@"

        [Test]
        public void ValueTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
  
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    var {typeof(T).Name}W{i} = new StackMemoryCollections.{wrapperNamespace}.Wrapper<{typeof(T).Name}>(&memory);
                    *{typeof(T).Name}W{i}.Ptr = {toStr(values[i])};
                    Assert.That(*{typeof(T).Name}W{i}.Ptr, Is.EqualTo(({typeof(T).Name})({toStr(values[i])})));
");
            }

            builder.Append($@"
                }}
            }}
        }}

");
        }

        private void WrapPrimitiveEnd(in StringBuilder builder)
        {
            builder.Append($@"

    }}
}}
");
        }
    }
}