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
                GenerateWrapPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var UInt32Values = new List<UInt32> { 15, 45, 0, 34, 140 };
                GenerateWrapPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var Int64Values = new List<Int64> { 15, -45, 0, 34, -140 };
                GenerateWrapPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var UInt64Values = new List<UInt64> { 15, 45, 0, 34, 140 };
                GenerateWrapPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var SByteValues = new List<SByte> { 15, -45, 0, -120, 15 };
                GenerateWrapPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var ByteValues = new List<Byte> { 15, 45, 0, 255, 78 };
                GenerateWrapPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var Int16Values = new List<Int16> { 15, -45, 0, -255, 120 };
                GenerateWrapPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var UInt16Values = new List<UInt16> { 15, 45, 0, 255, 15 };
                GenerateWrapPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var CharValues = new List<Char> { 's', 'a', 'q', ' ', '1' };
                GenerateWrapPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Class",
                    (val) => { return "'" + val.ToString() + "'"; }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Struct",
                    (val) => { return "'" + val.ToString() + "'"; }
                    );
            }

            {
                var DecimalValues = new List<Decimal> { 4.5m, 0.44m, -0.5m, 0m, 0.23m };
                GenerateWrapPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString("G") + "m"; }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString("G") + "m"; }
                    );
            }

            {
                var DoubleValues = new List<Double> { 4.5d, 0.44d, -0.5d, 0d, 0.23d };
                GenerateWrapPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString("G") + "d"; }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString("G") + "d"; }
                    );
            }

            {
                var BooleanValues = new List<Boolean> { true, false, true, false, true };
                GenerateWrapPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString().ToLowerInvariant(); }
                    );
            }

            {
                var SingleValues = new List<Single> { 4.5f, 0.44f, -0.5f, 0f, 0.23f };
                GenerateWrapPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Class",
                    (val) => { return val.ToString("F", CultureInfo.InvariantCulture) + "f"; }
                    );

                GenerateWrapPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Struct",
                    (val) => { return val.ToString("F",CultureInfo.InvariantCulture) + "f"; }
                    );
            }
        }

        private void GenerateWrapPrimitiveTest<T>(
            in GeneratorExecutionContext context,
            in List<T> values,
            in StringBuilder builder,
            in string wrapperNamespace,
            Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            var isChar = typeof(T) == typeof(char);

            builder.Clear();
            builder.Append($@"
using NUnit.Framework;
using System;

namespace Tests
{{
    [TestFixture]
    public class Wrapper{wrapperNamespace}{typeof(T).Name}Fixture
    {{
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
    }}
}}
");
            context.AddSource($"Wrapper{wrapperNamespace}{typeof(T).Name}Fixture.g.cs", builder.ToString());
        }
    }
}