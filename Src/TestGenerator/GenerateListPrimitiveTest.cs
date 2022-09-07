using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestGenerator
{
    public partial class Generator
    {
        private void GenerateListPrimitiveTest(
            in GeneratorExecutionContext context,
            in StringBuilder builder
            )
        {
            {
                ListPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Class",
                    Int32Convert
                    );

                ListPrimitiveTest(
                    in context,
                    Int32Values,
                    in builder,
                    "Struct",
                    Int32Convert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Class",
                    UInt32Convert
                    );

                ListPrimitiveTest(
                    in context,
                    UInt32Values,
                    in builder,
                    "Struct",
                    UInt32Convert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Class",
                    Int64Convert
                    );

                ListPrimitiveTest(
                    in context,
                    Int64Values,
                    in builder,
                    "Struct",
                    Int64Convert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Class",
                    UInt64Convert
                    );

                ListPrimitiveTest(
                    in context,
                    UInt64Values,
                    in builder,
                    "Struct",
                    UInt64Convert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Class",
                    SByteConvert
                    );

                ListPrimitiveTest(
                    in context,
                    SByteValues,
                    in builder,
                    "Struct",
                    SByteConvert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Class",
                    ByteConvert
                    );

                ListPrimitiveTest(
                    in context,
                    ByteValues,
                    in builder,
                    "Struct",
                    ByteConvert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Class",
                    Int16Convert
                    );

                ListPrimitiveTest(
                    in context,
                    Int16Values,
                    in builder,
                    "Struct",
                    Int16Convert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Class",
                    UInt16Convert
                    );

                ListPrimitiveTest(
                    in context,
                    UInt16Values,
                    in builder,
                    "Struct",
                    UInt16Convert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Class",
                    CharConvert
                    );

                ListPrimitiveTest(
                    in context,
                    CharValues,
                    in builder,
                    "Struct",
                    CharConvert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Class",
                    DecimalConvert
                    );

                ListPrimitiveTest(
                    in context,
                    DecimalValues,
                    in builder,
                    "Struct",
                    DecimalConvert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Class",
                    DoubleConvert
                    );

                ListPrimitiveTest(
                    in context,
                    DoubleValues,
                    in builder,
                    "Struct",
                    DoubleConvert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Class",
                    BooleanConvert
                    );

                ListPrimitiveTest(
                    in context,
                    BooleanValues,
                    in builder,
                    "Struct",
                    BooleanConvert
                    );
            }

            {
                ListPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Class",
                    SingleConvert
                    );

                ListPrimitiveTest(
                    in context,
                    SingleValues,
                    in builder,
                    "Struct",
                    SingleConvert
                    );
            }
        }

        private void ListPrimitiveTest<T>(
            in GeneratorExecutionContext context,
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Clear();
            ListPrimitiveStart<T>(in builder, in listNamespace);

            ListPrimitiveDispose(in values, in builder, in listNamespace);
            ListPrimitiveNotDispose(in values, in builder, in listNamespace);
            ListPrimitiveReseize(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveAdd(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveAddFuture(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveAddPtr(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveTryAdd(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveTryAddPtr(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveClear(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveClearOwn(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveCopy(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveTrimExcess(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveTrimExcessOwn(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveExpandCapacity(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveExpandCapacityOwn(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveReducingCapacity(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveReducingCapacityOwn(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveSize(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveCapacity(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveIndex(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveGetByIndex(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveGetOutByIndex(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveGetByIndexRef(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveInsert(in values, in builder, in listNamespace, in toStr);
            ListPrimitiveRemove(in values, in builder, in listNamespace, in toStr);

            ListPrimitiveEnd(in builder);
            
            context.AddSource($"ListOf{typeof(T).Name}{listNamespace}Fixture.g.cs", builder.ToString());
        }

        private void ListPrimitiveStart<T>(
            in StringBuilder builder,
            in string listNamespace
            ) where T : unmanaged
        {
            builder.Append($@"
using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Tests
{{
    [TestFixture]
    public class ListOf{typeof(T).Name}{listNamespace}Fixture
    {{
                    
");
        }

        private void ListPrimitiveDispose<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace
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
                        using var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    }}

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                }}
            }}
        }}
");
        }

        private void ListPrimitiveNotDispose<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace
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
                        var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    }}

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                }}
            }}
        }}
");
        }

        private void ListPrimitiveReseize<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
                var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"

                list.Add({toStr(values[i])});
");
            }

            builder.Append($@"
                Assert.That(list.Size, Is.EqualTo((nuint)4));
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));

                list.Add({toStr(values[4])});
                Assert.That(list.Size, Is.EqualTo((nuint)5));
                Assert.That(list.Capacity, Is.EqualTo((nuint)8));
");
            for (int i = 0; i < 5; i++)
            {
                builder.Append($@"
                Assert.That(*list[{i}], Is.EqualTo({toStr(values[i])}));
");
            }

            if(listNamespace == "Struct")
            {
                builder.Append($@"
            list.Dispose();
");
            }

            builder.Append($@"
            }}
        }}
");
        }

        private void ListPrimitiveAdd<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void AddTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(list.IsEmpty, Is.EqualTo(true));
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"

                    list.Add({toStr(values[i])});
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count}));
                    Assert.That(list.Size, Is.EqualTo((nuint){i + 1}));
");
            }

            builder.Append($@"

                    Assert.That(() => list.Add({toStr(values[0])}),
                        Throws.Exception.TypeOf(typeof(ArgumentException))
                        .And.Message.EqualTo(""Can't allocate memory"")
                        );
                }}
            }}
        }}
");
        }

        private void ListPrimitiveAddFuture<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void AddFutureTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(list.IsEmpty, Is.EqualTo(true));
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"

                    *list.GetFuture() = {toStr(values[i])};
                    list.AddFuture();
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count}));
                    Assert.That(list.Size, Is.EqualTo((nuint){i + 1}));
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                    Assert.That(() => list.GetFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""Future element not available"")
                        );

                    Assert.That(() => list.AddFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo(""Not enough memory to allocate list element"")
                        );
                }}
            }}
        }}
");
        }

        private void ListPrimitiveAddPtr<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
        public void AddPtrTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(list.IsEmpty, Is.EqualTo(true));

                    {typeof(T).Name} item;
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    item = {toStr(values[i])};
                    list.Add(&item);
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count}));
                    Assert.That(list.Size, Is.EqualTo((nuint){i + 1}));
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"

                    Assert.That(
                        () => 
                        {{
                            {typeof(T).Name} temp = {toStr(values[0])};
                            list.Add(&temp);
                        }},
                        Throws.Exception.TypeOf(typeof(ArgumentException))
                        .And.Message.EqualTo(""Can't allocate memory"")
                        );
                }}
            }}
        }}
");
        }

        private void ListPrimitiveTryAdd<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void TryAddTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"

                    Assert.That(list.TryAdd({toStr(values[i])}),Is.EqualTo(true));
");
            }

            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    Assert.That(list.TryAdd({toStr(values[i])}), Is.EqualTo(false));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void ListPrimitiveTryAddPtr<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
        public void TryAddPtrTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    {typeof(T).Name} item;
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    item = {toStr(values[i])};
                    Assert.That(list.TryAdd(&item),Is.EqualTo(true));
");
            }

            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    item = {toStr(values[i])};
                    Assert.That(list.TryAdd(&item), Is.EqualTo(false));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void ListPrimitiveClear<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
");
            }
            builder.Append($@"

                    Assert.That(list.Size, Is.EqualTo((nuint){values.Count}));
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count}));
                    list.Clear();
                    Assert.That(list.Size, Is.EqualTo((nuint)0));
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count}));
                }}
            }}
        }}
");
        }

        private void ListPrimitiveClearOwn<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
                using var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                list.Add({toStr(values[i])});
");
            }
            builder.Append($@"

                Assert.That(list.Size, Is.EqualTo((nuint)4));
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));
                list.Clear();
                Assert.That(list.Size, Is.EqualTo((nuint)0));
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));
            }}
        }}
");
        }

        private void ListPrimitiveCopy<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            if(listNamespace == "Class")
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
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    list.Add({toStr(values[i])});
");
                }

                builder.Append($@"
                    var list2 = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory2);
                    list.Copy(in list2);

                    Assert.That(list.Size, Is.EqualTo(list2.Size));
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    Assert.That(*list[{i}], Is.EqualTo(*list2[{i}]));
");
                }
                
                builder.Append($@"
                }}
            }}
        }}
");
            }
            else if (listNamespace == "Struct")
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
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    list.Add({toStr(values[i])});
");
                }

                builder.Append($@"
                    var list2 = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory2);
                    list.Copy(list2.Start);

                    Assert.That(list2.Size, Is.EqualTo((nuint)0));
                    list2.SetSize(list.Size);
                    Assert.That(list.Size, Is.EqualTo(list2.Size));
");
                for (int i = 0; i < values.Count; i++)
                {
                    builder.Append($@"
                    Assert.That(*list[{i}], Is.EqualTo(*list[{i}]));
");
                }

                builder.Append($@"
                }}
            }}
        }}
");
            }
        }

        private void ListPrimitiveTrimExcess<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count - 2}, &memory);
");
            for (int i = 0; i < values.Count - 2; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
");
            }

            builder.Append($@"
                    list.ExpandCapacity(2);
                    list.Add({toStr(values[0])});

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(list.Size, Is.EqualTo((nuint){values.Count - 1}));
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count}));
                    list.TrimExcess();
                    Assert.That(list.Size, Is.EqualTo((nuint){values.Count - 1}));
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count - 1}));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count - 1})));
                }}
            }}
        }}
");
        }

        private void ListPrimitiveTrimExcessOwn<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
                using var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                list.Add({toStr(values[i])});
");
            }

            builder.Append($@"
                list.ExpandCapacity(6);
                list.Add({toStr(values[0])});

                Assert.That(list.Size, Is.EqualTo((nuint)5));
                Assert.That(list.Capacity, Is.EqualTo((nuint)10));
                list.TrimExcess();
                Assert.That(list.Size, Is.EqualTo((nuint)5));
                Assert.That(list.Capacity, Is.EqualTo((nuint)5));
            }}
        }}
");
        }

        private void ListPrimitiveExpandCapacity<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
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
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count}));
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
");
            }
            builder.Append($@"
                    list.ExpandCapacity(3);
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count + 3}));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count + 3})));
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[i])}));
");
            }
            builder.Append($@"
                    list.Add({toStr(values[values.Count - 1])});
                    list.Add({toStr(values[values.Count - 2])});
                    list.Add({toStr(values[values.Count - 3])});
                    Assert.That(*list[{values.Count}], Is.EqualTo({toStr(values[values.Count - 1])}));
                    Assert.That(*list[{values.Count + 1}], Is.EqualTo({toStr(values[values.Count - 2])}));
                    Assert.That(*list[{values.Count + 2}], Is.EqualTo({toStr(values[values.Count - 3])}));
                }}
            }}
        }}
");
        }

        private void ListPrimitiveExpandCapacityOwn<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void ExpandCapacityOwnTest()
        {{
            unsafe
            {{
                using var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}();
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
");
            }
            builder.Append($@"
                list.ExpandCapacity(4);
                Assert.That(list.Capacity, Is.EqualTo((nuint)8));
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[i])}));
");
            }
            builder.Append($@"
                    list.Add({toStr(values[values.Count - 1])});
                    list.Add({toStr(values[values.Count - 2])});
                    list.Add({toStr(values[values.Count - 3])});
                    list.Add({toStr(values[values.Count - 4])});
                    Assert.That(*list[{4}], Is.EqualTo({toStr(values[values.Count - 1])}));
                    Assert.That(*list[{5}], Is.EqualTo({toStr(values[values.Count - 2])}));
                    Assert.That(*list[{6}], Is.EqualTo({toStr(values[values.Count - 3])}));
                    Assert.That(*list[{7}], Is.EqualTo({toStr(values[values.Count - 4])}));
            }}
        }}
");
        }

        private void ListPrimitiveReducingCapacity<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
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
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count})));
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count}));

");
            for (int i = 0; i < values.Count - 1; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
");
            }
            builder.Append($@"
                    list.ReducingCapacity(1);
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count - 1}));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {values.Count - 1})));

");
            for (int i = 0; i < values.Count - 1; i++)
            {
                builder.Append($@"
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[i])}));
");
            }
            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void ListPrimitiveReducingCapacityOwn<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void ReducingCapacityOwnTest()
        {{
            unsafe
            {{
                using var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}();
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
");
            }
            builder.Append($@"
                list.ExpandCapacity(1);
                Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                list.ReducingCapacity(1);
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));
");
            for (int i = 0; i < 4; i++)
            {
                builder.Append($@"
                Assert.That(*list[{i}], Is.EqualTo({toStr(values[i])}));
");
            }
            builder.Append($@"
            }}
        }}
");
        }

        private void ListPrimitiveSize<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
                    Assert.That(list.Size, Is.EqualTo((nuint){i + 1}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void ListPrimitiveCapacity<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
                    list.Remove(0);
                    list.Add({toStr(values[i])});
                    list.Remove(0);
                    Assert.That(list.Capacity, Is.EqualTo((nuint){values.Count}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");
        }

        private void ListPrimitiveIndex<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
");
            }

            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    Assert.That(new IntPtr(list[{i}]), Is.EqualTo(new IntPtr(({typeof(T).Name}*)memory.Start + {i})));
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                    Assert.That(() => list[{values.Count}],
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo(""Element outside the list"")
                        );
                }}
            }}
        }}
");
        }

        private void ListPrimitiveRemove<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void RemoveTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(() => list.Remove(0),
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo(""Element outside the list"")
                        );
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
");
            }
            builder.Append($@"
                    list.Remove(1);
                    list.Remove({values.Count - 2});
                    list.Remove(0);
");
            int j = 0;
            for (int i = 0; i < values.Count; i++)
            {
                if (i == 0 || i == 1 || i == values.Count - 1)
                {
                    continue;
                }
                builder.Append($@"
                    Assert.That(*list[{j}], Is.EqualTo({toStr(values[i])}));
");
                j += 1;
            }

            builder.Append($@"
                    Assert.That(list.Size, Is.EqualTo((nuint){values.Count - 3}));
                }}
            }}
        }}
");

        }

        private void ListPrimitiveInsert<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void InsertTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
");
            for (int i = 0; i < values.Count - 3; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
");
            }
            builder.Append($@"
                    list.Insert({toStr(values[values.Count - 3])} , 1);
                    list.Insert({toStr(values[values.Count - 2])} , {values.Count - 2});
                    list.Insert({toStr(values[values.Count - 1])} , 0);
");
            int j = 0;
            for (int i = 0; i < values.Count; i++)
            {
                if (i == 0)
                {
                    builder.Append($@"
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[values.Count - 1])}));
");
                    continue;
                }
                else if (i == 2)
                {
                    builder.Append($@"
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[values.Count - 3])}));
");
                    continue;
                }
                else if (i == values.Count - 1)
                {
                    builder.Append($@"
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[values.Count - 2])}));
");
                    continue;
                }
                else
                {
                    builder.Append($@"
                    Assert.That(*list[{i}], Is.EqualTo({toStr(values[j])}));
");
                }
                j += 1;
            }

            builder.Append($@"
                    Assert.That(list.Size, Is.EqualTo((nuint){values.Count}));
                    
                    Assert.That(() => list.Insert({toStr(values[values.Count - 1])}, 0),
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo(""Element outside the list"")
                        );
                }}
            }}
        }}
");

        }

        private void ListPrimitiveGetByIndex<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
            in Func<T, string> toStr
            ) where T : unmanaged
        {
            if (values.Count < 5)
            {
                throw new ArgumentException($"{nameof(values)} Must have minimum 5 values to generate tests");
            }

            builder.Append($@"
        [Test]
        public void GetByIndexTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(() => list.GetByIndex(0),
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo(""Element outside the list"")
                        );
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
                    var item{i} = list.GetByIndex({i});
                    Assert.That(item{i}, Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");

        }

        private void ListPrimitiveGetOutByIndex<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
        public void GetOutByIndexTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(() => list.GetOutByIndex(0, out _),
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo(""Element outside the list"")
                        );
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    list.Add({toStr(values[i])});
                    {typeof(T).Name} item{i};
                    list.GetOutByIndex({i},out item{i});
                    Assert.That(item{i}, Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");

        }

        private void ListPrimitiveGetByIndexRef<T>(
            in List<T> values,
            in StringBuilder builder,
            in string listNamespace,
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
        public void GetByIndexRefTest()
        {{
            unsafe
            {{
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof({typeof(T).Name}) * {values.Count}))
                {{
                    var list = new StackMemoryCollections.{listNamespace}.ListOf{typeof(T).Name}({values.Count}, &memory);
                    Assert.That(
                        () => 
                        {{
                            {typeof(T).Name} temp = {toStr(values[0])};
                            list.GetByIndex(0, ref temp);
                        }},
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo(""Element outside the list"")
                        );
                    {typeof(T).Name} item;
");
            for (int i = 0; i < values.Count; i++)
            {
                builder.Append($@"
                    item = {toStr(values[i])};
                    list.Add(in item);
                    {typeof(T).Name} item{i} = {toStr(values[0])};
                    list.GetByIndex({i}, ref item{i});
                    Assert.That(item{i}, Is.EqualTo({toStr(values[i])}));
");
            }

            builder.Append($@"
                }}
            }}
        }}
");

        }

        private void ListPrimitiveEnd(
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