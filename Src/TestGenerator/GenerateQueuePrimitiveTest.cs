using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestGenerator
{
    public partial class Generator
    {
        private void GenerateQueuePrimitiveTest(
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            {
                QueuePrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Class",
                    Int32Convert
                    );

                QueuePrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Struct",
                    Int32Convert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Class",
                    UInt32Convert
                    );

                QueuePrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Struct",
                    UInt32Convert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Class",
                    Int64Convert
                    );

                QueuePrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Struct",
                    Int64Convert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Class",
                    UInt64Convert
                    );

                QueuePrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Struct",
                    UInt64Convert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Class",
                    SByteConvert
                    );

                QueuePrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Struct",
                    SByteConvert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Class",
                    ByteConvert
                    );

                QueuePrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Struct",
                    ByteConvert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Class",
                    Int16Convert
                    );

                QueuePrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Struct",
                    Int16Convert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Class",
                    UInt16Convert
                    );

                QueuePrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Struct",
                    UInt16Convert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Class",
                    CharConvert
                    );

                QueuePrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Struct",
                    CharConvert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Class",
                    DecimalConvert
                    );

                QueuePrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Struct",
                    DecimalConvert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Class",
                    DoubleConvert
                    );

                QueuePrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Struct",
                    DoubleConvert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Class",
                    BooleanConvert
                    );

                QueuePrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Struct",
                    BooleanConvert
                    );
            }

            {
                QueuePrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Class",
                    SingleConvert
                    );

                QueuePrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Struct",
                    SingleConvert
                    );
            }
        }

        private void QueuePrimitiveTest<T>(
            in GeneratorExecutionContext context,
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Clear();
            QueuePrimitiveStart<T>(in builder, in queueNamespace);

            QueuePrimitiveDispose(in values, in builder, in queueNamespace);
            QueuePrimitiveNotDispose(in values, in builder);
            QueuePrimitiveReseize(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitivePush(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitivePushFuture(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitivePushPtr(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveTryPush(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveTryPushPtr(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitiveClear(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveClearOwn(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitiveCopy(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitiveTrimExcess(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveTrimExcessOwn(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitiveExpandCapacity(in values, in builder, in queueNamespace);
            QueuePrimitiveExpandCapacityHeadAfterTail(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveExpandCapacityHeadAfterTailOwn(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveExpandCapacityHeadBeforeTail(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveExpandCapacityHeadBeforeTailOwn(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitiveReducingCapacity(in values, in builder, in queueNamespace);

            QueuePrimitiveSize(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveCapacity(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitiveIndex(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitivePop(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitiveFrontBack(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveFrontBackOut(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveFrontBackPtr(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveFrontBackRefValue(in values, in builder, in queueNamespace, in toStr);
            QueuePrimitiveGetFrontBackPtr(in values, in builder, in queueNamespace, in toStr);

            QueuePrimitiveEnd(in builder);
            
            context.AddSource($"QueueOf{typeof(T).Name}{queueNamespace}Fixture.g.cs", builder.ToString());
            File.WriteAllText($"E:\\Work\\OutTrash\\QueueOf{typeof(T).Name}{queueNamespace}Fixture.g.cs", builder.ToString());
        }

        private void QueuePrimitiveStart<T>(
            in StringBuilder builder,
            in string queueNamespace
            ) where T : unmanaged
        {
            builder.Append($@"
using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Tests
{{
    [TestFixture]
    public class QueueOf{typeof(T).Name}{queueNamespace}Fixture
    {{
                    
");
        }

        private void QueuePrimitiveDispose<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace
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
                        using var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    }}

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                }}
            }}
        }}

");
        }

        private void QueuePrimitiveNotDispose<T>(
            in List<T> values,
            in StringBuilder builder
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
                        var queue = new StackMemoryCollections.Struct.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    }}

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                }}
            }}
        }}

");
        }

        private void QueuePrimitiveReseize<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
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
                var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"

                queue.Push({toStr(values[i])});
");
            }

            builder.Append($@"
                Assert.That(queue.Size, Is.EqualTo((nuint)4));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)4));

                queue.Push({toStr(values[4])});
                Assert.That(queue.Size, Is.EqualTo((nuint)5));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)8));
");
            for (int i = 0; i < 5; i++)
            {
                builder.Append($@"
                Assert.That(queue.Front(), Is.EqualTo({toStr(values[i])}));
                Assert.That(queue.Back(), Is.EqualTo({toStr(values[4])}));
                queue.Pop();
");
            }

            if(queueNamespace == "Struct")
            {
                builder.Append($@"
            queue.Dispose();
");
            }

            builder.Append($@"
            }}
        }}
");
        }

        private void QueuePrimitivePush<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
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
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(queue.IsEmpty, Is.EqualTo(true));
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"

                    queue.Push({toStr(values[i])});
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                    Assert.That(queue.Size, Is.EqualTo((nuint){i + 1}));
");
            }

            builder.Append($@"

                    Assert.That(() => queue.Push({toStr(values[0])}),
                        Throws.Exception.TypeOf(typeof(ArgumentException))
                        .And.Message.EqualTo(""Can't allocate memory"")
                        );
                }}
            }}
        }}
");
        }

        private void QueuePrimitivePushFuture<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void PushFutureTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(queue.IsEmpty, Is.EqualTo(true));
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"

                    *queue.BackFuture() = {toStr(values[i])};
                    queue.PushFuture();
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                    Assert.That(queue.Size, Is.EqualTo((nuint){i + 1}));
                    Assert.That(queue.Front(), Is.EqualTo({toStr(values[0])}));
                    Assert.That(queue.Back(), Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                    Assert.That(() => queue.BackFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""Future element not available"")
                        );

                    Assert.That(() => queue.PushFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""Not enough memory to allocate queue element"")
                        );
                }}
            }}
        }}
");
        }

        private void QueuePrimitivePushPtr<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        [SkipLocalsInit]
        public void PushPtrTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(queue.IsEmpty, Is.EqualTo(true));

                    {typeof(T).Name} item;
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    item = {toStr(values[i])};
                    queue.Push(&item);
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                    Assert.That(queue.Size, Is.EqualTo((nuint){i + 1}));
                    Assert.That(queue.Front(), Is.EqualTo({toStr(values[0])}));
                    Assert.That(queue.Back(), Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"

                    Assert.That(
                        () => 
                        {{
                            {typeof(T).Name} temp = {toStr(values[0])};
                            queue.Push(&temp);
                        }},
                        Throws.Exception.TypeOf(typeof(ArgumentException))
                        .And.Message.EqualTo(""Can't allocate memory"")
                        );
                }}
            }}
        }}
");
        }

        private void QueuePrimitiveTryPush<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
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
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"

                    Assert.That(queue.TryPush({toStr(values[i])}),Is.EqualTo(true));
");
            }

            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    Assert.That(queue.TryPush({toStr(values[i])}), Is.EqualTo(false));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void QueuePrimitiveTryPushPtr<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        [SkipLocalsInit]
        public void TryPushPtrTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    {typeof(T).Name} item;
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    item = {toStr(values[i])};
                    Assert.That(queue.TryPush(&item),Is.EqualTo(true));
");
            }

            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    item = {toStr(values[i])};
                    Assert.That(queue.TryPush(&item), Is.EqualTo(false));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void QueuePrimitiveClear<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
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
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
");
            }

            builder.Append($@"

                    Assert.That(queue.Size, Is.EqualTo((nuint){values.Count}));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                    queue.Clear();
                    Assert.That(queue.Size, Is.EqualTo((nuint)0));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                }}
            }}
        }}
");
        }

        private void QueuePrimitiveClearOwn<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
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
                var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                queue.Push({toStr(values[i])});
");
            }

            builder.Append($@"

                Assert.That(queue.Size, Is.EqualTo((nuint)4));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)4));
                queue.Clear();
                Assert.That(queue.Size, Is.EqualTo((nuint)0));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)4));
");
            if (queueNamespace == "Struct")
            {
                builder.Append($@"
                queue.Dispose();
");
            }
            builder.Append($@"
            }}
        }}
");
        }

        private void QueuePrimitiveCopy<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            if(queueNamespace == "Class")
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
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    queue.Push({toStr(values[i])});
");
                }

                builder.Append($@"
                    var queue2 = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory2);
                    queue.Copy(in queue2);

                    Assert.That(queue.Size, Is.EqualTo(queue2.Size));
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    Assert.That(*queue[{i}], Is.EqualTo(*queue2[{i}]));
");
                }
                
                builder.Append($@"
                }}
            }}
        }}
");
            }
            else if (queueNamespace == "Struct")
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
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    queue.Push({toStr(values[i])});
");
                }

                builder.Append($@"
                    var queue2 = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory2);
                    queue.Copy(queue2.Start);

                    Assert.That(queue2.Size, Is.EqualTo((nuint)0));
                    queue.GetPositions(out var head, out var tail, out var size);
                    queue2.SetPositions(head, tail, size);
                    Assert.That(queue.Size, Is.EqualTo(queue2.Size));
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    Assert.That(*queue[{i}], Is.EqualTo(*queue[{i}]));
");
                }

                builder.Append($@"
                }}
            }}
        }}
");
            }
        }

        private void QueuePrimitiveTrimExcess<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void TrimExcessTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count - 2}, &memory);
");
            for (int i = 0; i < values.Count - 2; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
");
            }

            builder.Append($@"
                    queue.ExpandCapacity(2);
                    queue.Push({toStr(values[0])});

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(queue.Size, Is.EqualTo((nuint){values.Count - 1}));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                    queue.TrimExcess();
                    Assert.That(queue.Size, Is.EqualTo((nuint){values.Count - 1}));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count - 1}));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count - 1})));
                }}
            }}
        }}
");
        }

        private void QueuePrimitiveTrimExcessOwn<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void TrimExcessOwnTest()
        {{
            unsafe
            {{
                var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                queue.Push({toStr(values[i])});
");
            }

            builder.Append($@"
                queue.ExpandCapacity(6);
                queue.Push({toStr(values[0])});

                Assert.That(queue.Size, Is.EqualTo((nuint)5));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)10));
                queue.TrimExcess();
                Assert.That(queue.Size, Is.EqualTo((nuint)5));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
");
            if (queueNamespace == "Struct")
            {
                builder.Append($@"
                queue.Dispose();
");
            }
            builder.Append($@"
            }}
        }}
");
        }

        private void QueuePrimitiveExpandCapacity<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void ExpandCapacityTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count + 3}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                    queue.ExpandCapacity(3);
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count + 3}));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count + 3})));
                }}
            }}

            unsafe
            {{
                var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}();
                Assert.That(queue.Capacity, Is.EqualTo((nuint)4));
                queue.ExpandCapacity(6);
                Assert.That(queue.Capacity, Is.EqualTo((nuint)10));
");
            if (queueNamespace == "Struct")
            {
                builder.Append($@"
                queue.Dispose();
");
            }
            builder.Append($@"
            }}
        }}
");
        }

        private void QueuePrimitiveExpandCapacityHeadAfterTail<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void ExpandCapacityHeadAfterTailTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count + 3}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
");
            }
            builder.Append($@"
                    queue.Pop();
                    queue.Pop();
                    queue.Pop();
");
            for (int i = 0; i < 3; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
");
            }
            builder.Append($@"
                    queue.ExpandCapacity(3);
                    queue.Push({toStr(values[values.Count - 1])});
");
            for (int i = 3; i < values.Count; i++)
            {
                builder.Append($@"
                    Assert.That(queue.Front(), Is.EqualTo({toStr(values[i])}));
                    queue.Pop();
");
            }
            for (int i = 0; i < 3; i++)
            {
                builder.Append($@"
                    Assert.That(queue.Front(), Is.EqualTo({toStr(values[i])}));
                    queue.Pop();
");
            }

            builder.Append($@"
                    Assert.That(queue.Front(), Is.EqualTo({toStr(values[values.Count - 1])}));
                    queue.Pop();

                }}
            }}
        }}
");
        }

        private void QueuePrimitiveExpandCapacityHeadAfterTailOwn<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void ExpandCapacityHeadAfterTailOwnTest()
        {{
            unsafe
            {{
                var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                queue.Push({toStr(values[i])});
");
            }
            builder.Append($@"
                queue.Pop();
                queue.Pop();
");
            for (int i = 0; i < 2; i++)
            {
                builder.Append($@"
                queue.Push({toStr(values[i])});
");
            }
            builder.Append($@"
                queue.ExpandCapacity(3);
                queue.Push({toStr(values[values.Count - 1])});
");
            for (int i = 2; i < 4; i++)
            {
                builder.Append($@"
                Assert.That(queue.Front(), Is.EqualTo({toStr(values[i])}));
                queue.Pop();
");
            }
            for (int i = 0; i < 2; i++)
            {
                builder.Append($@"
                Assert.That(queue.Front(), Is.EqualTo({toStr(values[i])}));
                queue.Pop();
");
            }

            builder.Append($@"
                Assert.That(queue.Front(), Is.EqualTo({toStr(values[values.Count - 1])}));
                queue.Pop();
");
            if (queueNamespace == "Struct")
            {
                builder.Append($@"
                queue.Dispose();
");
            }
                builder.Append($@"
            }}
        }}
");
        }

        private void QueuePrimitiveExpandCapacityHeadBeforeTail<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void ExpandCapacityHeadBeforeTailTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count + 3}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
");
            }
            builder.Append($@"
                    queue.ExpandCapacity(3);
                    queue.Push({toStr(values[values.Count - 1])});
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    Assert.That(queue.Front(), Is.EqualTo({toStr(values[i])}));
                    queue.Pop();
");
            }
            builder.Append($@"
                    Assert.That(queue.Front(), Is.EqualTo({toStr(values[values.Count - 1])}));
                    queue.Pop();

                }}
            }}
        }}
");
        }

        private void QueuePrimitiveExpandCapacityHeadBeforeTailOwn<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void ExpandCapacityHeadBeforeTailOwnTest()
        {{
            unsafe
            {{
                var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                queue.Push({toStr(values[i])});
");
            }
            builder.Append($@"
                queue.ExpandCapacity(3);
                queue.Push({toStr(values[values.Count - 1])});
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                Assert.That(queue.Front(), Is.EqualTo({toStr(values[i])}));
                queue.Pop();
");
            }
            builder.Append($@"
                Assert.That(queue.Front(), Is.EqualTo({toStr(values[values.Count - 1])}));
                queue.Pop();
");
            if (queueNamespace == "Struct")
            {
                builder.Append($@"
                queue.Dispose();
");
            }
            builder.Append($@"
            }}
        }}
");
        }

        private void QueuePrimitiveReducingCapacity<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void ReducingCapacityTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                    queue.ReducingCapacity(1);
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count - 1}));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count - 1})));
                }}
            }}

            unsafe
            {{
                var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}();
                queue.ExpandCapacity(6);

                Assert.That(queue.Capacity, Is.EqualTo((nuint)10));
                queue.ReducingCapacity(1);
                Assert.That(queue.Capacity, Is.EqualTo((nuint)9));
");
            if (queueNamespace == "Struct")
            {
                builder.Append($@"
                queue.Dispose();
");
            }
            builder.Append($@"
            }}
        }}
");
        }

        private void QueuePrimitiveSize<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void SizeTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
                    Assert.That(queue.Size, Is.EqualTo((nuint){i + 1}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void QueuePrimitiveCapacity<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void CapacityTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
                    queue.Pop();
                    queue.Push({toStr(values[i])});
                    queue.Pop();
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void QueuePrimitiveIndex<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void IndexTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
");
            }

            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    Assert.That(new IntPtr(queue[{i}]), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {i})));
                    Assert.That(*queue[{i}], Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                    Assert.That(() => queue[{values.Count}],
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""Element outside the queue"")
                        );
                }}
            }}
        }}
");
        }

        private void QueuePrimitivePop<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void PopTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
");
            }

            for (int i = 0; i < values.Count - 1; i++)
            {
                builder.Append($@"
                    queue.Pop();
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                    Assert.That(queue.Size, Is.EqualTo((nuint){values.Count - 1 - i }));
");
            }

            builder.Append($@"
                    queue.Pop();
                    Assert.That(queue.IsEmpty, Is.EqualTo(true));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint){values.Count}));
                    Assert.That(queue.Size, Is.EqualTo((nuint)0));
                    
                    Assert.That(() => queue.Pop(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""There are no elements on the queue"")
                        );
                }}
            }}
        }}
");

        }

        private void QueuePrimitiveFrontBack<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void FrontBackTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(() => queue.Front(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""There are no elements on the queue"")
                        );
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
                    var item{i}Front = queue.Front();
                    var item{i}Back = queue.Back();
                    Assert.That(item{i}Front, Is.EqualTo({toStr(values[0])}));
                    Assert.That(item{i}Back, Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");

        }

        private void QueuePrimitiveFrontBackOut<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        [SkipLocalsInit]
        public void FrontBackOutTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(() => queue.FrontOut(out _),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""There are no elements on the queue"")
                        );
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
                    {typeof(T).Name} item{i}Front;
                    queue.FrontOut(out item{i}Front);
                    {typeof(T).Name} item{i}Back;
                    queue.BackOut(out item{i}Back);
                    Assert.That(item{i}Front, Is.EqualTo({toStr(values[0])}));
                    Assert.That(item{i}Back, Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");

        }

        private void QueuePrimitiveFrontBackPtr<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        [SkipLocalsInit]
        public void FrontBackPtrTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(
                        () => 
                        {{
                            {typeof(T).Name} temp = {toStr(values[0])};
                            {typeof(T).Name}* tempPtr = &temp;
                            queue.Front(in tempPtr);
                        }},
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""There are no elements on the queue"")
                        );
                    {typeof(T).Name} item;
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    item = {toStr(values[i])};
                    queue.Push(in item);
                    {typeof(T).Name} item{i}Front;
                    {typeof(T).Name}* itemPtr{i}Front = &item{i}Front;
                    {typeof(T).Name} item{i}Back;
                    {typeof(T).Name}* itemPtr{i}Back = &item{i}Back;
                    queue.Front(in itemPtr{i}Front);
                    queue.Back(in itemPtr{i}Back);
                    Assert.That(item{i}Front, Is.EqualTo({toStr(values[0])}));
                    Assert.That(item{i}Back, Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");

        }

        private void QueuePrimitiveFrontBackRefValue<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        [SkipLocalsInit]
        public void FrontBackRefValueTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(
                        () => 
                        {{
                            {typeof(T).Name} temp = {toStr(values[0])};
                            queue.Front(ref temp);
                        }},
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""There are no elements on the queue"")
                        );
                    {typeof(T).Name} item;
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    item = {toStr(values[i])};
                    queue.Push(in item);
                    {typeof(T).Name} item{i}Front = {toStr(values[0])};
                    queue.Front(ref item{i}Front);
                    {typeof(T).Name} item{i}Back = {toStr(values[0])};
                    queue.Back(ref item{i}Back);
                    Assert.That(item{i}Front, Is.EqualTo({toStr(values[0])}));
                    Assert.That(item{i}Back, Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");

        }

        private void QueuePrimitiveGetFrontBackPtr<T>(
            in List<T> values,
            in StringBuilder builder,
            in string queueNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void GetFrontBackPtrTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var queue = new StackMemoryCollections.{queueNamespace}.QueueOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(() => queue.FrontPtr(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""There are no elements on the queue"")
                        );
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    queue.Push({toStr(values[i])});
                    var itemPtr{i}Front = queue.FrontPtr();
                    var itemPtr{i}Back = queue.BackPtr();
                    Assert.That(new IntPtr(itemPtr{i}Back), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {i})));
                    Assert.That(new IntPtr(itemPtr{i}Front), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start)));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");

        }

        private void QueuePrimitiveEnd(
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