
using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Tests
{
    [TestFixture]
    public class ListOfTestStructFixture
    {


        [Test]
        public void DisposeTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    {
                        using var list = new Tests.Struct.ListOfTestStruct(5, &memory);
                    }

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                }
            }
        }

        [Test]
        public void NotDisposeTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    {
                        var list = new Tests.Struct.ListOfTestStruct(5, &memory);
                    }

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                }
            }
        }

        [Test]
        public void ReseizeTest()
        {
            unsafe
            {
                using var list = new Tests.Struct.ListOfTestStruct();

                list.Add(new TestStruct(45, 23, new TestClass(78, 56)));
                list.Add(new TestStruct(45, 23, new TestClass(78, 56)));
                list.Add(new TestStruct(45, 23, new TestClass(78, 56)));
                list.Add(new TestStruct(45, 23, new TestClass(78, 56)));

                Assert.That(list.Size, Is.EqualTo((nuint)4));
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));

                list.Add(new TestStruct(45, 23, new TestClass(78, 56)));
                Assert.That(list.Size, Is.EqualTo((nuint)5));
                Assert.That(list.Capacity, Is.EqualTo((nuint)8));
            }
        }

        [Test]
        public void AddTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(list.IsEmpty, Is.EqualTo(true));


                    list.Add(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)1));


                    list.Add(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)2));


                    list.Add(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)3));


                    list.Add(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)4));


                    list.Add(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)5));


                    Assert.That(() => list.Add(new TestStruct(45, 23, new TestClass(78, 56))),
                        Throws.Exception.TypeOf(typeof(ArgumentException))
                        .And.Message.EqualTo("Can't allocate memory")
                        );
                }
            }
        }

        [Test]
        public void AddFutureTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(list.IsEmpty, Is.EqualTo(true));

                    var wrap = new Tests.Struct.TestStructWrapper(list.GetFuture(), false);
                    wrap.Fill(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.AddFuture();
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)1));

                    var item0 = list.GetByIndex(0);
                    Assert.That(item0.Int32, Is.EqualTo(1));
                    Assert.That(item0.Int64, Is.EqualTo(1));
                    Assert.That(item0.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(item0.TestClass.Int64, Is.EqualTo(11));

                    wrap.ChangePtr(list.GetFuture());
                    wrap.Fill(new TestStruct(2, 2, new TestClass(22, 22)));
                    list.AddFuture();
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)2));
                    var item1 = list.GetByIndex(1);
                    Assert.That(item1.Int32, Is.EqualTo(2));
                    Assert.That(item1.Int64, Is.EqualTo(2));
                    Assert.That(item1.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(item1.TestClass.Int64, Is.EqualTo(22));

                    wrap.ChangePtr(list.GetFuture());
                    wrap.Fill(new TestStruct(3, 3, new TestClass(33, 33)));
                    list.AddFuture();
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)3));
                    var item2 = list.GetByIndex(2);
                    Assert.That(item2.Int32, Is.EqualTo(3));
                    Assert.That(item2.Int64, Is.EqualTo(3));
                    Assert.That(item2.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(item2.TestClass.Int64, Is.EqualTo(33));

                    wrap.ChangePtr(list.GetFuture());
                    wrap.Fill(new TestStruct(4, 4, new TestClass(44, 44)));
                    list.AddFuture();
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)4));
                    var item3 = list.GetByIndex(3);
                    Assert.That(item3.Int32, Is.EqualTo(4));
                    Assert.That(item3.Int64, Is.EqualTo(4));
                    Assert.That(item3.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(item3.TestClass.Int64, Is.EqualTo(44));

                    wrap.ChangePtr(list.GetFuture());
                    wrap.Fill(new TestStruct(5, 5, new TestClass(55, 55)));
                    list.AddFuture();
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)5));
                    var item4 = list.GetByIndex(4);
                    Assert.That(item4.Int32, Is.EqualTo(5));
                    Assert.That(item4.Int64, Is.EqualTo(5));
                    Assert.That(item4.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(item4.TestClass.Int64, Is.EqualTo(55));

                    Assert.That(() => list.GetFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Future element not available")
                        );

                    Assert.That(() => list.AddFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Not enough memory to allocate list element")
                        );
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void AddPtrTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(list.IsEmpty, Is.EqualTo(true));

                    using var wrap = new Tests.Struct.TestStructWrapper();
                    wrap.Fill(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Add(wrap.Ptr);
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)1));
                    var item = list.GetByIndex(0);
                    Assert.That(item.Int32, Is.EqualTo(1));
                    Assert.That(item.Int64, Is.EqualTo(1));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(11));

                    wrap.Fill(new TestStruct(2, 2, new TestClass(22, 22)));
                    list.Add(wrap.Ptr);
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)2));
                    item = list.GetByIndex(1);
                    Assert.That(item.Int32, Is.EqualTo(2));
                    Assert.That(item.Int64, Is.EqualTo(2));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(22));

                    wrap.Fill(new TestStruct(3, 3, new TestClass(33, 33)));
                    list.Add(wrap.Ptr);
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)3));
                    item = list.GetByIndex(2);
                    Assert.That(item.Int32, Is.EqualTo(3));
                    Assert.That(item.Int64, Is.EqualTo(3));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                    wrap.Fill(new TestStruct(4, 4, new TestClass(44, 44)));
                    list.Add(wrap.Ptr);
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)4));
                    item = list.GetByIndex(3);
                    Assert.That(item.Int32, Is.EqualTo(4));
                    Assert.That(item.Int64, Is.EqualTo(4));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(44));

                    wrap.Fill(new TestStruct(5, 5, new TestClass(55, 55)));
                    list.Add(wrap.Ptr);
                    Assert.That(list.IsEmpty, Is.EqualTo(false));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(list.Size, Is.EqualTo((nuint)5));
                    item = list.GetByIndex(4);
                    Assert.That(item.Int32, Is.EqualTo(5));
                    Assert.That(item.Int64, Is.EqualTo(5));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(55));


                    Assert.That(
                        () =>
                        {
                            list.Add(wrap.Ptr);
                        },
                        Throws.Exception.TypeOf(typeof(ArgumentException))
                        .And.Message.EqualTo("Can't allocate memory")
                        );
                }
            }
        }

        [Test]
        public void TryAddTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);

                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));
                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));
                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));
                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));
                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));

                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
                    Assert.That(list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void TryAddPtrTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);
                    using var wrap = new Tests.Struct.TestStructWrapper();
                    wrap.Fill(new TestStruct(1, 1, new TestClass(11, 11)));

                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(true));

                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(false));
                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(false));
                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(false));
                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(false));
                    Assert.That(list.TryAdd(wrap.Ptr), Is.EqualTo(false));
                }
            }
        }

        [Test]
        public void ClearTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);

                    list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11)));

                    Assert.That(list.Size, Is.EqualTo((nuint)5));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    list.Clear();
                    Assert.That(list.Size, Is.EqualTo((nuint)0));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                }
            }
        }

        [Test]
        public void ClearOwnTest()
        {
            unsafe
            {
                using var list = new Tests.Struct.ListOfTestStruct();

                list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11)));
                list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11)));
                list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11)));
                list.TryAdd(new TestStruct(1, 1, new TestClass(11, 11)));

                Assert.That(list.Size, Is.EqualTo((nuint)4));
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));
                list.Clear();
                Assert.That(list.Size, Is.EqualTo((nuint)0));
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));
            }
        }

        [Test]
        public void CopyTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                using (var memory2 = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Add(new TestStruct(2, 2, new TestClass(22, 22)));
                    list.Add(new TestStruct(3, 3, new TestClass(33, 33)));
                    list.Add(new TestStruct(4, 4, new TestClass(44, 44)));
                    list.Add(new TestStruct(5, 5, new TestClass(55, 55)));

                    var list2 = new Tests.Class.ListOfTestStruct(5, &memory2);
                    list.Copy(in list2);

                    Assert.That(list.Size, Is.EqualTo(list2.Size));

                    var item = list2.GetByIndex(0);
                    Assert.That(item.Int32, Is.EqualTo(1));
                    Assert.That(item.Int64, Is.EqualTo(1));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(11));

                    item = list2.GetByIndex(1);
                    Assert.That(item.Int32, Is.EqualTo(2));
                    Assert.That(item.Int64, Is.EqualTo(2));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(22));

                    item = list2.GetByIndex(2);
                    Assert.That(item.Int32, Is.EqualTo(3));
                    Assert.That(item.Int64, Is.EqualTo(3));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                    item = list2.GetByIndex(3);
                    Assert.That(item.Int32, Is.EqualTo(4));
                    Assert.That(item.Int64, Is.EqualTo(4));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(44));

                    item = list2.GetByIndex(4);
                    Assert.That(item.Int32, Is.EqualTo(5));
                    Assert.That(item.Int64, Is.EqualTo(5));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(55));
                }
            }
        }

        [Test]
        public void TrimExcessTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(3, &memory);

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));

                    list.ExpandCapacity(2);

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(list.Size, Is.EqualTo((nuint)4));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                    list.TrimExcess();
                    Assert.That(list.Size, Is.EqualTo((nuint)4));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)4));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 4))));
                }
            }
        }

        [Test]
        public void TrimExcessOwnTest()
        {
            unsafe
            {
                using var list = new Tests.Struct.ListOfTestStruct();

                list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                list.Add(new TestStruct(1, 1, new TestClass(11, 11)));

                list.ExpandCapacity(6);
                list.Add(new TestStruct(1, 1, new TestClass(11, 11)));

                Assert.That(list.Size, Is.EqualTo((nuint)5));
                Assert.That(list.Capacity, Is.EqualTo((nuint)10));
                list.TrimExcess();
                Assert.That(list.Size, Is.EqualTo((nuint)5));
                Assert.That(list.Capacity, Is.EqualTo((nuint)5));
            }
        }

        [Test]
        public void ExpandCapacityTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 8))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Add(new TestStruct(2, 2, new TestClass(22, 22)));
                    list.Add(new TestStruct(3, 3, new TestClass(33, 33)));
                    list.Add(new TestStruct(4, 4, new TestClass(44, 44)));
                    list.Add(new TestStruct(5, 5, new TestClass(55, 55)));

                    list.ExpandCapacity(3);
                    Assert.That(list.Capacity, Is.EqualTo((nuint)8));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 8))));

                    var item = list.GetByIndex(0);
                    Assert.That(item.Int32, Is.EqualTo(1));
                    Assert.That(item.Int64, Is.EqualTo(1));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(11));

                    item = list.GetByIndex(1);
                    Assert.That(item.Int32, Is.EqualTo(2));
                    Assert.That(item.Int64, Is.EqualTo(2));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(22));

                    item = list.GetByIndex(2);
                    Assert.That(item.Int32, Is.EqualTo(3));
                    Assert.That(item.Int64, Is.EqualTo(3));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                    item = list.GetByIndex(3);
                    Assert.That(item.Int32, Is.EqualTo(4));
                    Assert.That(item.Int64, Is.EqualTo(4));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(44));

                    item = list.GetByIndex(4);
                    Assert.That(item.Int32, Is.EqualTo(5));
                    Assert.That(item.Int64, Is.EqualTo(5));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(55));

                    list.Add(new TestStruct(6, 6, new TestClass(66, 66)));
                    list.Add(new TestStruct(7, 7, new TestClass(77, 77)));
                    list.Add(new TestStruct(8, 8, new TestClass(88, 88)));

                    item = list.GetByIndex(5);
                    Assert.That(item.Int32, Is.EqualTo(6));
                    Assert.That(item.Int64, Is.EqualTo(6));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(66));

                    item = list.GetByIndex(6);
                    Assert.That(item.Int32, Is.EqualTo(7));
                    Assert.That(item.Int64, Is.EqualTo(7));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(77));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(77));

                    item = list.GetByIndex(7);
                    Assert.That(item.Int32, Is.EqualTo(8));
                    Assert.That(item.Int64, Is.EqualTo(8));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(88));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(88));
                }
            }
        }

        [Test]
        public void ExpandCapacityOwnTest()
        {
            unsafe
            {
                using var list = new Tests.Struct.ListOfTestStruct();
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));

                list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                list.Add(new TestStruct(2, 2, new TestClass(22, 22)));
                list.Add(new TestStruct(3, 3, new TestClass(33, 33)));
                list.Add(new TestStruct(4, 4, new TestClass(44, 44)));

                list.ExpandCapacity(4);
                Assert.That(list.Capacity, Is.EqualTo((nuint)8));

                var item = list.GetByIndex(0);
                Assert.That(item.Int32, Is.EqualTo(1));
                Assert.That(item.Int64, Is.EqualTo(1));
                Assert.That(item.TestClass.Int32, Is.EqualTo(11));
                Assert.That(item.TestClass.Int64, Is.EqualTo(11));

                item = list.GetByIndex(1);
                Assert.That(item.Int32, Is.EqualTo(2));
                Assert.That(item.Int64, Is.EqualTo(2));
                Assert.That(item.TestClass.Int32, Is.EqualTo(22));
                Assert.That(item.TestClass.Int64, Is.EqualTo(22));

                item = list.GetByIndex(2);
                Assert.That(item.Int32, Is.EqualTo(3));
                Assert.That(item.Int64, Is.EqualTo(3));
                Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                item = list.GetByIndex(3);
                Assert.That(item.Int32, Is.EqualTo(4));
                Assert.That(item.Int64, Is.EqualTo(4));
                Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                Assert.That(item.TestClass.Int64, Is.EqualTo(44));

                list.Add(new TestStruct(5, 5, new TestClass(55, 55)));
                list.Add(new TestStruct(6, 6, new TestClass(66, 66)));
                list.Add(new TestStruct(7, 7, new TestClass(77, 77)));
                list.Add(new TestStruct(8, 8, new TestClass(88, 88)));

                item = list.GetByIndex(4);
                Assert.That(item.Int32, Is.EqualTo(5));
                Assert.That(item.Int64, Is.EqualTo(5));
                Assert.That(item.TestClass.Int32, Is.EqualTo(55));
                Assert.That(item.TestClass.Int64, Is.EqualTo(55));

                item = list.GetByIndex(5);
                Assert.That(item.Int32, Is.EqualTo(6));
                Assert.That(item.Int64, Is.EqualTo(6));
                Assert.That(item.TestClass.Int32, Is.EqualTo(66));
                Assert.That(item.TestClass.Int64, Is.EqualTo(66));

                item = list.GetByIndex(6);
                Assert.That(item.Int32, Is.EqualTo(7));
                Assert.That(item.Int64, Is.EqualTo(7));
                Assert.That(item.TestClass.Int32, Is.EqualTo(77));
                Assert.That(item.TestClass.Int64, Is.EqualTo(77));

                item = list.GetByIndex(7);
                Assert.That(item.Int32, Is.EqualTo(8));
                Assert.That(item.Int64, Is.EqualTo(8));
                Assert.That(item.TestClass.Int32, Is.EqualTo(88));
                Assert.That(item.TestClass.Int64, Is.EqualTo(88));
            }
        }

        [Test]
        public void ReducingCapacityTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Add(new TestStruct(2, 2, new TestClass(22, 22)));
                    list.Add(new TestStruct(3, 3, new TestClass(33, 33)));
                    list.Add(new TestStruct(4, 4, new TestClass(44, 44)));

                    list.ReducingCapacity(1);
                    Assert.That(list.Capacity, Is.EqualTo((nuint)4));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 4))));

                    var item = list.GetByIndex(0);
                    Assert.That(item.Int32, Is.EqualTo(1));
                    Assert.That(item.Int64, Is.EqualTo(1));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(11));

                    item = list.GetByIndex(1);
                    Assert.That(item.Int32, Is.EqualTo(2));
                    Assert.That(item.Int64, Is.EqualTo(2));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(22));

                    item = list.GetByIndex(2);
                    Assert.That(item.Int32, Is.EqualTo(3));
                    Assert.That(item.Int64, Is.EqualTo(3));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                    item = list.GetByIndex(3);
                    Assert.That(item.Int32, Is.EqualTo(4));
                    Assert.That(item.Int64, Is.EqualTo(4));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(44));
                }
            }
        }

        [Test]
        public void ReducingCapacityOwnTest()
        {
            unsafe
            {
                using var list = new Tests.Struct.ListOfTestStruct();

                list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                list.Add(new TestStruct(2, 2, new TestClass(22, 22)));
                list.Add(new TestStruct(3, 3, new TestClass(33, 33)));
                list.Add(new TestStruct(4, 4, new TestClass(44, 44)));

                list.ExpandCapacity(1);
                Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                list.ReducingCapacity(1);
                Assert.That(list.Capacity, Is.EqualTo((nuint)4));

                var item = list.GetByIndex(0);
                Assert.That(item.Int32, Is.EqualTo(1));
                Assert.That(item.Int64, Is.EqualTo(1));
                Assert.That(item.TestClass.Int32, Is.EqualTo(11));
                Assert.That(item.TestClass.Int64, Is.EqualTo(11));

                item = list.GetByIndex(1);
                Assert.That(item.Int32, Is.EqualTo(2));
                Assert.That(item.Int64, Is.EqualTo(2));
                Assert.That(item.TestClass.Int32, Is.EqualTo(22));
                Assert.That(item.TestClass.Int64, Is.EqualTo(22));

                item = list.GetByIndex(2);
                Assert.That(item.Int32, Is.EqualTo(3));
                Assert.That(item.Int64, Is.EqualTo(3));
                Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                item = list.GetByIndex(3);
                Assert.That(item.Int32, Is.EqualTo(4));
                Assert.That(item.Int64, Is.EqualTo(4));
                Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                Assert.That(item.TestClass.Int64, Is.EqualTo(44));
            }
        }

        [Test]
        public void SizeTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    Assert.That(list.Size, Is.EqualTo((nuint)1));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    Assert.That(list.Size, Is.EqualTo((nuint)2));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    Assert.That(list.Size, Is.EqualTo((nuint)3));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    Assert.That(list.Size, Is.EqualTo((nuint)4));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    Assert.That(list.Size, Is.EqualTo((nuint)5));
                }
            }
        }

        [Test]
        public void CapacityTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Remove(0);
                    Assert.That(list.Capacity, Is.EqualTo((nuint)5));
                }
            }
        }

        [Test]
        public void IndexTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Add(new TestStruct(2, 2, new TestClass(22, 22)));
                    list.Add(new TestStruct(3, 3, new TestClass(33, 33)));
                    list.Add(new TestStruct(4, 4, new TestClass(44, 44)));
                    list.Add(new TestStruct(5, 5, new TestClass(55, 55)));

                    var ptr0 = list[0];
                    Assert.That(new IntPtr(ptr0), Is.EqualTo(new IntPtr((byte*)memory.Start)));

                    var ptr1 = list[1];
                    Assert.That(new IntPtr(list[1]), Is.EqualTo(new IntPtr((byte*)memory.Start + TestStructHelper.SizeOf)));

                    var ptr2 = list[2];
                    Assert.That(new IntPtr(list[2]), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 2))));

                    var ptr3 = list[3];
                    Assert.That(new IntPtr(list[3]), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 3))));

                    var ptr4 = list[4];
                    Assert.That(new IntPtr(list[4]), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf  * 4))));

                    Assert.That(() => list[5],
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo("Element outside the list")
                        );
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void GetOutByIndexTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);
                    Assert.That(() => list.GetOutByIndex(0, out _),
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo("Element outside the list")
                        );

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    TestStruct item;
                    list.GetOutByIndex(0, out item);
                    Assert.That(item.Int32, Is.EqualTo(1));
                    Assert.That(item.Int64, Is.EqualTo(1));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(11));

                    list.Add(new TestStruct(2, 2, new TestClass(22, 22)));
                    list.GetOutByIndex(1, out item);
                    Assert.That(item.Int32, Is.EqualTo(2));
                    Assert.That(item.Int64, Is.EqualTo(2));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(22));

                    list.Add(new TestStruct(3, 3, new TestClass(33, 33)));
                    list.GetOutByIndex(2, out item);
                    Assert.That(item.Int32, Is.EqualTo(3));
                    Assert.That(item.Int64, Is.EqualTo(3));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                    list.Add(new TestStruct(4, 4, new TestClass(44, 44)));
                    list.GetOutByIndex(3, out item);
                    Assert.That(item.Int32, Is.EqualTo(4));
                    Assert.That(item.Int64, Is.EqualTo(4));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(44));

                    list.Add(new TestStruct(5, 5, new TestClass(55, 55)));
                    list.GetOutByIndex(4, out item);
                    Assert.That(item.Int32, Is.EqualTo(5));
                    Assert.That(item.Int64, Is.EqualTo(5));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(55));
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void GetByIndexRefTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);
                    Assert.That(
                        () =>
                        {
                            TestStruct temp = new TestStruct();
                            list.GetByIndex(0, ref temp);
                        },
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo("Element outside the list")
                        );

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    TestStruct item = new TestStruct();
                    list.GetByIndex(0, ref item);
                    Assert.That(item.Int32, Is.EqualTo(1));
                    Assert.That(item.Int64, Is.EqualTo(1));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(11));

                    list.Add(new TestStruct(2, 2, new TestClass(22, 22)));
                    list.GetByIndex(1, ref item);
                    Assert.That(item.Int32, Is.EqualTo(2));
                    Assert.That(item.Int64, Is.EqualTo(2));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(22));

                    list.Add(new TestStruct(3, 3, new TestClass(33, 33)));
                    list.GetByIndex(2, ref item);
                    Assert.That(item.Int32, Is.EqualTo(3));
                    Assert.That(item.Int64, Is.EqualTo(3));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                    list.Add(new TestStruct(4, 4, new TestClass(44, 44)));
                    list.GetByIndex(3, ref item);
                    Assert.That(item.Int32, Is.EqualTo(4));
                    Assert.That(item.Int64, Is.EqualTo(4));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(44));

                    list.Add(new TestStruct(5, 5, new TestClass(55, 55)));
                    list.GetByIndex(4, ref item);
                    Assert.That(item.Int32, Is.EqualTo(5));
                    Assert.That(item.Int64, Is.EqualTo(5));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(55));
                }
            }
        }

        [Test]
        public void InsertTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Add(new TestStruct(2, 2, new TestClass(22, 22)));

                    list.Insert(new TestStruct(3, 3, new TestClass(33, 33)), 1);
                    list.Insert(new TestStruct(4, 4, new TestClass(44, 44)), 3);
                    list.Insert(new TestStruct(5, 5, new TestClass(55, 55)), 0);

                    var item = list.GetByIndex(0);
                    Assert.That(item.Int32, Is.EqualTo(5));
                    Assert.That(item.Int64, Is.EqualTo(5));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(55));

                    item = list.GetByIndex(1);
                    Assert.That(item.Int32, Is.EqualTo(1));
                    Assert.That(item.Int64, Is.EqualTo(1));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(11));

                    item = list.GetByIndex(2);
                    Assert.That(item.Int32, Is.EqualTo(3));
                    Assert.That(item.Int64, Is.EqualTo(3));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                    item = list.GetByIndex(3);
                    Assert.That(item.Int32, Is.EqualTo(2));
                    Assert.That(item.Int64, Is.EqualTo(2));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(22));

                    item = list.GetByIndex(4);
                    Assert.That(item.Int32, Is.EqualTo(4));
                    Assert.That(item.Int64, Is.EqualTo(4));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(44));

                    Assert.That(list.Size, Is.EqualTo((nuint)5));

                    Assert.That(() => list.Insert(new TestStruct(), 0),
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo("Element outside the list")
                        );
                }
            }
        }

        [Test]
        public void RemoveTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var list = new Tests.Struct.ListOfTestStruct(5, &memory);
                    Assert.That(() => list.Remove(0),
                        Throws.Exception.TypeOf(typeof(IndexOutOfRangeException))
                        .And.Message.EqualTo("Element outside the list")
                        );

                    list.Add(new TestStruct(1, 1, new TestClass(11, 11)));
                    list.Add(new TestStruct(2, 2, new TestClass(22, 22)));
                    list.Add(new TestStruct(3, 3, new TestClass(33, 33)));
                    list.Add(new TestStruct(4, 4, new TestClass(44, 44)));
                    list.Add(new TestStruct(5, 5, new TestClass(55, 55)));

                    list.Remove(1);
                    list.Remove(3);
                    list.Remove(0);

                    var item = list.GetByIndex(0);
                    Assert.That(item.Int32, Is.EqualTo(3));
                    Assert.That(item.Int64, Is.EqualTo(3));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(33));

                    item = list.GetByIndex(1);
                    Assert.That(item.Int32, Is.EqualTo(4));
                    Assert.That(item.Int64, Is.EqualTo(4));
                    Assert.That(item.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(item.TestClass.Int64, Is.EqualTo(44));

                    Assert.That(list.Size, Is.EqualTo((nuint)2));
                }
            }
        }


    }
}
