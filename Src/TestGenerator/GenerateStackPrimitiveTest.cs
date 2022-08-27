using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestGenerator
{
    public partial class Generator
    {
        private void GenerateStackPrimitiveTest(
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            {
                StackPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Class",
                    Int32Convert
                    );

                StackPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Struct",
                    Int32Convert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Class",
                    UInt32Convert
                    );

                StackPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Struct",
                    UInt32Convert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Class",
                    Int64Convert
                    );

                StackPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Struct",
                    Int64Convert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Class",
                    UInt64Convert
                    );

                StackPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Struct",
                    UInt64Convert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Class",
                    SByteConvert
                    );

                StackPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Struct",
                    SByteConvert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Class",
                    ByteConvert
                    );

                StackPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Struct",
                    ByteConvert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Class",
                    Int16Convert
                    );

                StackPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Struct",
                    Int16Convert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Class",
                    UInt16Convert
                    );

                StackPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Struct",
                    UInt16Convert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Class",
                    CharConvert
                    );

                StackPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Struct",
                    CharConvert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Class",
                    DecimalConvert
                    );

                StackPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Struct",
                    DecimalConvert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Class",
                    DoubleConvert
                    );

                StackPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Struct",
                    DoubleConvert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Class",
                    BooleanConvert
                    );

                StackPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Struct",
                    BooleanConvert
                    );
            }

            {
                StackPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Class",
                    SingleConvert
                    );

                StackPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Struct",
                    SingleConvert
                    );
            }
        }

        private void StackPrimitiveTest<T>(
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
            StackPrimitiveStart<T>(in builder, in wrapperNamespace);

            StackPrimitiveDispose(in values, in builder, in wrapperNamespace);
            StackPrimitiveNotDispose(in values, in builder, in wrapperNamespace);
            StackPrimitiveReseize(in values, in builder, in wrapperNamespace, in toStr);
            StackPrimitivePush(in values, in builder, in wrapperNamespace, in toStr);
            StackPrimitiveTryPush(in values, in builder, in wrapperNamespace, in toStr);
            StackPrimitiveClear(in values, in builder, in wrapperNamespace, in toStr);
            StackPrimitiveClearOwn(in values, in builder, in wrapperNamespace, in toStr);
            StackPrimitiveCopy(in values, in builder, in wrapperNamespace, in toStr);

            StackPrimitiveEnd(in builder);
            
            context.AddSource($"Stack{wrapperNamespace}{typeof(T).Name}Fixture.g.cs", builder.ToString());
        }

        private void StackPrimitiveStart<T>(
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
    public class Stack{wrapperNamespace}{typeof(T).Name}Fixture
    {{
                    
");
        }

        private void StackPrimitiveDispose<T>(
            in List<T> values,
            in StringBuilder builder,
            in string wrapperNamespace
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void DisposeTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    {{
                        using var stack = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>({values.Count}, &memory);
                    }}

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                }}
            }}
        }}

");
        }

        private void StackPrimitiveNotDispose<T>(
            in List<T> values,
            in StringBuilder builder,
            in string wrapperNamespace
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void NotDisposeTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    {{
                        var stack = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>({values.Count}, &memory);
                    }}

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                }}
            }}
        }}

");
        }

        private void StackPrimitiveReseize<T>(
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

            builder.Append($@"
        [Test]
        public void ReseizeTest()
        {{
            unsafe
            {{
                var stack = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"

                stack.Push({toStr(values[i])});
");
            }

            builder.Append($@"
                Assert.That(stack.Size, Is.EqualTo((nuint)4));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)4));

                stack.Push({toStr(values[0])});
                Assert.That(stack.Size, Is.EqualTo((nuint)5));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)8));
            }}
        }}

");
        }

        private void StackPrimitivePush<T>(
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

            builder.Append($@"
        [Test]
        public void PushTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var stack = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>({values.Count}, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"

                    stack.Push({toStr(values[i])});
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint){values.Count}));
                    Assert.That(stack.Size, Is.EqualTo((nuint){i + 1}));
");
            }

            builder.Append($@"

                    Assert.That(() => stack.Push({toStr(values[0])}),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""Not enough memory to allocate stack element"")
                        );
                }}
            }}
        }}
");
        }

        private void StackPrimitiveTryPush<T>(
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

            builder.Append($@"
        [Test]
        public void TryPushTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var stack = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"

                    Assert.That(stack.TryPush({toStr(values[i])}),Is.EqualTo(true));
");
            }

            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    Assert.That(stack.TryPush({toStr(values[i])}), Is.EqualTo(false));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void StackPrimitiveClear<T>(
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

            builder.Append($@"
        [Test]
        public void ClearTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var stack = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    stack.Push({toStr(values[i])});
");
            }

            builder.Append($@"

                    Assert.That(stack.Size, Is.EqualTo((nuint){values.Count}));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint){values.Count}));
                    stack.Clear();
                    Assert.That(stack.Size, Is.EqualTo((nuint)0));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint){values.Count}));
                }}
            }}
        }}
");
        }

        private void StackPrimitiveClearOwn<T>(
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

            builder.Append($@"
        [Test]
        public void ClearOwnTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var stack = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                    stack.Push({toStr(values[i])});
");
            }

            builder.Append($@"

                    Assert.That(stack.Size, Is.EqualTo((nuint)4));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)4));
                    stack.Clear();
                    Assert.That(stack.Size, Is.EqualTo((nuint)0));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)4));
                }}
            }}
        }}
");
        }

        private void StackPrimitiveCopy<T>(
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

            if(wrapperNamespace == "Class")
            {
                builder.Append($@"
        [Test]
        public void CopyTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                using (var memory2 = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var stack = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>({values.Count}, &memory);
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    stack.Push({toStr(values[i])});
");
                }

                builder.Append($@"
                    var stack2 = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>({values.Count}, &memory2);
                    stack.Copy(in stack2);

                    Assert.That(stack.Size, Is.EqualTo(stack2.Size));
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    Assert.That(*stack[{i}], Is.EqualTo(*stack[{i}]));
");
                }
                
                builder.Append($@"
                }}
            }}
        }}
");
            }
            else if (wrapperNamespace == "Struct")
            {
                builder.Append($@"
        [Test]
        public void CopyTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                using (var memory2 = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var stack = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>({values.Count}, &memory);
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    stack.Push({toStr(values[i])});
");
                }

                builder.Append($@"
                    var stack2 = new StackMemoryCollections.{wrapperNamespace}.Stack<{typeof(T).Name}>({values.Count}, &memory2);
                    stack.Copy(stack2.Start);

                    Assert.That(stack2.Size, Is.EqualTo((nuint)0));
                    stack2.Size = stack.Size;
                    Assert.That(stack.Size, Is.EqualTo(stack2.Size));
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    Assert.That(*stack[{i}], Is.EqualTo(*stack[{i}]));
");
                }

                builder.Append($@"
                }}
            }}
        }}
");
            }
        }

        private void StackPrimitiveEnd(
            in StringBuilder builder
            )
        {
            builder.Append($@"

    }}
}}                
");
        }
    }
}