using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

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
                WrapPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Class",
                    Int32Convert
                    );

                WrapPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Struct",
                    Int32Convert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Class",
                    UInt32Convert
                    );

                WrapPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Struct",
                    UInt32Convert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Class",
                    Int64Convert
                    );

                WrapPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Struct",
                    Int64Convert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Class",
                    UInt64Convert
                    );

                WrapPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Struct",
                    UInt64Convert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Class",
                    SByteConvert
                    );

                WrapPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Struct",
                    SByteConvert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Class",
                    ByteConvert
                    );

                WrapPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Struct",
                    ByteConvert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Class",
                    Int16Convert
                    );

                WrapPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Struct",
                    Int16Convert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Class",
                    UInt16Convert
                    );

                WrapPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Struct",
                    UInt16Convert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Class",
                    CharConvert
                    );

                WrapPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Struct",
                    CharConvert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Class",
                    DecimalConvert
                    );

                WrapPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Struct",
                    DecimalConvert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Class",
                    DoubleConvert
                    );

                WrapPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Struct",
                    DoubleConvert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Class",
                    BooleanConvert
                    );

                WrapPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Struct",
                    BooleanConvert
                    );
            }

            {
                WrapPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Class",
                    SingleConvert
                    );

                WrapPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Struct",
                    SingleConvert
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
            WrapPrimitivePtr(in values, in builder, in wrapperNamespace, in toStr);
            WrapPrimitiveValueProperty(in values, in builder, in wrapperNamespace, in toStr);
            WrapPrimitiveChangePtr<T>(in builder, in wrapperNamespace);

            WrapPrimitiveEnd(in builder);
            
            context.AddSource($"{typeof(T).Name}Wrapper{wrapperNamespace}Fixture.g.cs", builder.ToString());
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
    public class {typeof(T).Name}Wrapper{wrapperNamespace}Fixture
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
                        using var {typeof(T).Name}W{i} = new {wrapperNamespace}.{typeof(T).Name}Wrapper(&memory);
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
                        var {typeof(T).Name}W{i} = new {wrapperNamespace}.{typeof(T).Name}Wrapper(&memory);
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

        private void WrapPrimitivePtr<T>(
            in List<T> values,
            in StringBuilder builder,
            in string wrapperNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            builder.Append($@"

        [Test]
        public void PtrTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
  
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    var {typeof(T).Name}W{i} = new {wrapperNamespace}.{typeof(T).Name}Wrapper(&memory);
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

        private void WrapPrimitiveValueProperty<T>(
            in List<T> values,
            in StringBuilder builder,
            in string wrapperNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            builder.Append($@"

        [Test]
        public void ValuePropertyTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
  
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    var {typeof(T).Name}W{i} = new {wrapperNamespace}.{typeof(T).Name}Wrapper(&memory);
                    {typeof(T).Name}W{i}.Value = {toStr(values[i])};
                    Assert.That({typeof(T).Name}W{i}.Value, Is.EqualTo(({typeof(T).Name})({toStr(values[i])})));
");
            }

            builder.Append($@"
                }}
            }}
        }}

");
        }

        private void WrapPrimitiveChangePtr<T>(
            in StringBuilder builder,
            in string wrapperNamespace
            ) where T : unmanaged
        {
            builder.Append($@"

        [Test]
        public void ChangePtrTest()
        {{
            unsafe
            {{

                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name})))
                {{
                    var {typeof(T).Name}W = new {wrapperNamespace}.{typeof(T).Name}Wrapper(({typeof(T).Name}*)memory.Start);
                    Assert.That(new IntPtr({typeof(T).Name}W.Ptr), Is.EqualTo(new IntPtr(memory.Start)));
                    using (var memory2 = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name})))
                    {{
                        {typeof(T).Name}W.ChangePtr(({typeof(T).Name}*)memory2.Start);
                        Assert.That(new IntPtr({typeof(T).Name}W.Ptr), Is.EqualTo(new IntPtr(memory2.Start)));
                    }}
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