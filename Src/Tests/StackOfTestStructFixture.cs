using NUnit.Framework;
using StackMemoryAttributes.Attributes;
using StackMemoryCollections.Attibutes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Tests
{
    [TestFixture]
    public class StackOfTestStructFixture
    {
        [Test]
        public void DisposeTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    {
                        using var stack = new Tests.Struct.StackOfTestStruct(5, &memory);
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
                        var stack = new Tests.Struct.StackOfTestStruct(5, &memory);
                    }

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start +(TestStructHelper.SizeOf * 5))));
                }
            }
        }


        [Test]
        public void ReseizeTest()
        {
            unsafe
            {
                var stack = new Tests.Struct.StackOfTestStruct();

                stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));

                Assert.That(stack.Size, Is.EqualTo((nuint)4));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)4));

                stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                Assert.That(stack.Size, Is.EqualTo((nuint)5));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)8));
            }
        }


        [Test]
        public void PushTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));


                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));


                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));


                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)3));


                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)4));


                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)5));


                    Assert.That(() => stack.Push(new TestStruct(45, 23, new TestClass(78, 56))),
                        Throws.Exception.TypeOf(typeof(ArgumentException))
                        .And.Message.EqualTo("Can't allocate memory")
                        );
                }
            }
        }

        [Test]
        public void PushFutureTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));

                    Assert.That(new IntPtr(stack.TopFuture()), Is.EqualTo(new IntPtr((byte*)memory.Start)));
                    stack.PushFuture();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    Assert.That(new IntPtr(stack.TopFuture()), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 1))));
                    stack.PushFuture();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));

                    Assert.That(new IntPtr(stack.TopFuture()), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 2))));
                    stack.PushFuture();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)3));

                    Assert.That(new IntPtr(stack.TopFuture()), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 3))));
                    stack.PushFuture();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)4));


                    Assert.That(new IntPtr(stack.TopFuture()), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 4))));
                    stack.PushFuture();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)5));


                    Assert.That(() => stack.TopFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Future element not available")
                        );

                    Assert.That(() => stack.PushFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Not enough memory to allocate stack element")
                        );
                }
            }
        }

        [Test]
        public void PushNullTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var stack = new Tests.Struct.StackOfTestStruct(1, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + TestStructHelper.SizeOf)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));

                    Assert.That(stack.Capacity, Is.EqualTo((nuint)1));
                    stack.Push(new TestStruct(45, 23, null));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)1));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void PushPtrTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));

                    using var item = new Tests.Struct.TestStructWrapper();
                    item.Fill(new TestStruct(45, 23, new TestClass(78, 56)));

                    stack.Push(item.Ptr);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    stack.Push(item.Ptr);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));

                    stack.Push(item.Ptr);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)3));

                    stack.Push(item.Ptr);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)4));

                    stack.Push(item.Ptr);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)5));


                    Assert.That(
                        () =>
                        {
                            using var item = new Tests.Struct.TestStructWrapper();
                            item.Fill(new TestStruct(45, 23, new TestClass(78, 56)));
                            stack.Push(item.Ptr);
                        },
                        Throws.Exception.TypeOf(typeof(ArgumentException))
                        .And.Message.EqualTo("Can't allocate memory")
                        );
                }
            }
        }

        [Test]
        public void TryPushTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);

                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(true));
                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(true));
                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(true));
                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(true));
                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(true));
                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(false));
                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(false));
                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(false));
                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(false));
                    Assert.That(stack.TryPush(new TestStruct(45, 23, new TestClass(78, 56))), Is.EqualTo(false));

                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void TryPushPtrTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);
                    using var item = new Tests.Struct.TestStructWrapper();
                    item.Fill(new TestStruct(45, 23, new TestClass(78, 56)));

                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(true));
                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(true));
                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(true));
                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(true));
                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(true));
                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(false));
                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(false));
                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(false));
                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(false));
                    Assert.That(stack.TryPush(item.Ptr), Is.EqualTo(false));
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
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);

                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));

                    Assert.That(stack.Size, Is.EqualTo((nuint)5));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    stack.Clear();
                    Assert.That(stack.Size, Is.EqualTo((nuint)0));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                }
            }
        }

        [Test]
        public void ClearOwnTest()
        {
            unsafe
            {
                var stack = new Tests.Struct.StackOfTestStruct();

                stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                stack.Push(new TestStruct(45, 23, new TestClass(78, 56)));

                Assert.That(stack.Size, Is.EqualTo((nuint)4));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)4));
                stack.Clear();
                Assert.That(stack.Size, Is.EqualTo((nuint)0));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)4));
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
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);

                    var testClass2W = new Struct.TestClassWrapper();
                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    var testStructW = new Struct.TestStructWrapper(stack.TopPtr(), false);
                    testStructW.TestClass2 = new IntPtr(testClass2W.Ptr);

                    stack.Push(new TestStruct(13, 400, new TestClass(22, 5787)));
                    testStructW.ChangePtr(stack.TopPtr());
                    testStructW.TestClass2 = new IntPtr(testClass2W.Ptr);

                    stack.Push(new TestStruct(8, 85, new TestClass(711, 446)));
                    testStructW.ChangePtr(stack.TopPtr());
                    testStructW.TestClass2 = new IntPtr(testClass2W.Ptr);

                    stack.Push(new TestStruct(6, 84, new TestClass(756, 33)));
                    testStructW.ChangePtr(stack.TopPtr());
                    testStructW.TestClass2 = new IntPtr(testClass2W.Ptr);

                    stack.Push(new TestStruct(25, 46, new TestClass(7, 22)));
                    testStructW.ChangePtr(stack.TopPtr());

                    var stack2 = new Tests.Struct.StackOfTestStruct(5, &memory2);
                    stack.Copy(stack2.Start);

                    Assert.That(stack2.Size, Is.EqualTo((nuint)0));
                    stack2.Size = stack.Size;
                    Assert.That(stack.Size, Is.EqualTo(stack2.Size));

                    using var wrap = new Tests.Struct.TestStructWrapper(stack[0], false);
                    using var wrap2 = new Tests.Struct.TestStructWrapper(stack2[0], false);
                    Assert.That(wrap.Int32, Is.EqualTo(wrap2.Int32));
                    Assert.That(wrap.Int64, Is.EqualTo(wrap2.Int64));
                    Assert.That(wrap.TestClass.Int32, Is.EqualTo(wrap2.TestClass.Int32));
                    Assert.That(wrap.TestClass.Int64, Is.EqualTo(wrap2.TestClass.Int64));
                    Assert.That(wrap.TestClass2, Is.EqualTo(IntPtr.Zero));

                    wrap.ChangePtr(stack[1]);
                    wrap2.ChangePtr(stack2[1]);
                    Assert.That(wrap.Int32, Is.EqualTo(wrap2.Int32));
                    Assert.That(wrap.Int64, Is.EqualTo(wrap2.Int64));
                    Assert.That(wrap.TestClass.Int32, Is.EqualTo(wrap2.TestClass.Int32));
                    Assert.That(wrap.TestClass.Int64, Is.EqualTo(wrap2.TestClass.Int64));
                    Assert.That(wrap.TestClass2, Is.EqualTo(new IntPtr(testClass2W.Ptr)));

                    wrap.ChangePtr(stack[2]);
                    wrap2.ChangePtr(stack2[2]);
                    Assert.That(wrap.Int32, Is.EqualTo(wrap2.Int32));
                    Assert.That(wrap.Int64, Is.EqualTo(wrap2.Int64));
                    Assert.That(wrap.TestClass.Int32, Is.EqualTo(wrap2.TestClass.Int32));
                    Assert.That(wrap.TestClass.Int64, Is.EqualTo(wrap2.TestClass.Int64));
                    Assert.That(wrap.TestClass2, Is.EqualTo(new IntPtr(testClass2W.Ptr)));

                    wrap.ChangePtr(stack[3]);
                    wrap2.ChangePtr(stack2[3]);
                    Assert.That(wrap.Int32, Is.EqualTo(wrap2.Int32));
                    Assert.That(wrap.Int64, Is.EqualTo(wrap2.Int64));
                    Assert.That(wrap.TestClass.Int32, Is.EqualTo(wrap2.TestClass.Int32));
                    Assert.That(wrap.TestClass.Int64, Is.EqualTo(wrap2.TestClass.Int64));
                    Assert.That(wrap.TestClass2, Is.EqualTo(new IntPtr(testClass2W.Ptr)));

                    wrap.ChangePtr(stack[4]);
                    wrap2.ChangePtr(stack2[4]);
                    Assert.That(wrap.Int32, Is.EqualTo(wrap2.Int32));
                    Assert.That(wrap.Int64, Is.EqualTo(wrap2.Int64));
                    Assert.That(wrap.TestClass.Int32, Is.EqualTo(wrap2.TestClass.Int32));
                    Assert.That(wrap.TestClass.Int64, Is.EqualTo(wrap2.TestClass.Int64));
                    Assert.That(wrap.TestClass2, Is.EqualTo(new IntPtr(testClass2W.Ptr)));

                    testClass2W.Dispose();
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
                    var stack = new Tests.Struct.StackOfTestStruct(3, &memory);

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));

                    stack.ExpandCapacity(2);
                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(stack.Size, Is.EqualTo((nuint)4));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    stack.TrimExcess();
                    Assert.That(stack.Size, Is.EqualTo((nuint)4));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)4));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 4))));
                }
            }
        }

        [Test]
        public void TrimExcessOwnTest()
        {
            unsafe
            {
                var stack = new Tests.Struct.StackOfTestStruct();

                stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));

                stack.ExpandCapacity(6);
                stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));

                Assert.That(stack.Size, Is.EqualTo((nuint)5));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)10));
                stack.TrimExcess();
                Assert.That(stack.Size, Is.EqualTo((nuint)5));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
            }
        }

        [Test]
        public void ExpandCapacityTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 8))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    stack.ExpandCapacity(3);
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)8));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 8))));
                }
            }

            unsafe
            {
                var stack = new Tests.Struct.StackOfTestStruct();
                Assert.That(stack.Capacity, Is.EqualTo((nuint)4));
                stack.ExpandCapacity(6);
                Assert.That(stack.Capacity, Is.EqualTo((nuint)10));
            }
        }

        [Test]
        public void ReducingCapacityTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    stack.ReducingCapacity(1);
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)4));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 4))));
                }
            }

            unsafe
            {
                var stack = new Tests.Struct.StackOfTestStruct();
                stack.ExpandCapacity(6);

                Assert.That(stack.Capacity, Is.EqualTo((nuint)10));
                stack.ReducingCapacity(1);
                Assert.That(stack.Capacity, Is.EqualTo((nuint)9));
            }
        }

        [Test]
        public void SizeTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    Assert.That(stack.Size, Is.EqualTo((nuint)3));

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    Assert.That(stack.Size, Is.EqualTo((nuint)4));

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    Assert.That(stack.Size, Is.EqualTo((nuint)5));

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
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));

                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    stack.Push(new TestStruct(45, 27, new TestClass(123, 5776)));
                    stack.Pop();
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));

                }
            }
        }

        [Test]
        public void IndexTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 3))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(3, &memory);
                    var item1 = new TestStruct(78, 11, new TestClass(1, 5776));
                    stack.Push(item1);

                    var item2 = new TestStruct(26, 28, new TestClass(5, 5));
                    stack.Push(item2);

                    var item3 = new TestStruct(7, 21, new TestClass(6, 543));
                    stack.Push(item3);

                    Assert.That(new IntPtr(stack[0]), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 2))));
                    using var wrap = new Tests.Struct.TestStructWrapper(stack[0], false);
                    Assert.That(wrap.Int32, Is.EqualTo(7));
                    Assert.That(wrap.Int64, Is.EqualTo(21));
                    Assert.That(wrap.TestClass.Int32, Is.EqualTo(6));
                    Assert.That(wrap.TestClass.Int64, Is.EqualTo(543));
                    Assert.That(wrap.TestClass2, Is.EqualTo(IntPtr.Zero));

                    Assert.That(new IntPtr(stack[1]), Is.EqualTo(new IntPtr((byte*)memory.Start + TestStructHelper.SizeOf)));
                    wrap.ChangePtr(stack[1]);
                    Assert.That(wrap.Int32, Is.EqualTo(26));
                    Assert.That(wrap.Int64, Is.EqualTo(28));
                    Assert.That(wrap.TestClass.Int32, Is.EqualTo(5));
                    Assert.That(wrap.TestClass.Int64, Is.EqualTo(5));
                    Assert.That(wrap.TestClass2, Is.EqualTo(IntPtr.Zero));

                    Assert.That(new IntPtr(stack[2]), Is.EqualTo(new IntPtr((byte*)memory.Start)));
                    wrap.ChangePtr(stack[2]);
                    Assert.That(wrap.Int32, Is.EqualTo(78));
                    Assert.That(wrap.Int64, Is.EqualTo(11));
                    Assert.That(wrap.TestClass.Int32, Is.EqualTo(1));
                    Assert.That(wrap.TestClass.Int64, Is.EqualTo(5776));
                    Assert.That(wrap.TestClass2, Is.EqualTo(IntPtr.Zero));

                    Assert.That(() => stack[3],
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Element outside the stack")
                        );
                }
            }
        }

        [Test]
        public void PopTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(5, &memory);

                    stack.Push(new TestStruct(78, 11, new TestClass(1, 5776)));
                    stack.Push(new TestStruct(78, 11, new TestClass(1, 5776)));
                    stack.Push(new TestStruct(78, 11, new TestClass(1, 5776)));
                    stack.Push(new TestStruct(78, 11, new TestClass(1, 5776)));
                    stack.Push(new TestStruct(78, 11, new TestClass(1, 5776)));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)4));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)3));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(stack.Size, Is.EqualTo((nuint)0));

                    Assert.That(() => stack.Pop(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("There are no elements on the stack")
                        );
                }
            }
        }

        [Test]
        public void TopTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 2))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(2, &memory);
                    Assert.That(() => stack.Top(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("There are no elements on the stack")
                        );

                    stack.Push(new TestStruct(78, 11, new TestClass(1, 5776)));
                    var item0 = stack.Top();
                    Assert.That(item0.Int32, Is.EqualTo(78));
                    Assert.That(item0.Int64, Is.EqualTo(11));
                    Assert.That(item0.TestClass.Int32, Is.EqualTo(1));
                    Assert.That(item0.TestClass.Int64, Is.EqualTo(5776));

                    stack.Push(new TestStruct(1, 2, new TestClass(3, 897879789)));
                    var item1 = stack.Top();
                    Assert.That(item1.Int32, Is.EqualTo(1));
                    Assert.That(item1.Int64, Is.EqualTo(2));
                    Assert.That(item1.TestClass.Int32, Is.EqualTo(3));
                    Assert.That(item1.TestClass.Int64, Is.EqualTo(897879789));
                }
            }
        }

        [Test]
        public void TopNullTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(1, &memory);
                    stack.Push(new TestStruct(78, 11, null));
                    var item0 = stack.Top();
                    Assert.That(item0.Int32, Is.EqualTo(78));
                    Assert.That(item0.Int64, Is.EqualTo(11));
                    Assert.That(item0.TestClass, Is.EqualTo(null));
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void TopOutTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 2))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(2, &memory);
                    Assert.That(() => stack.TopOut(out _),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("There are no elements on the stack")
                        );

                    stack.Push(new TestStruct(78, 11, new TestClass(1, 5776)));
                    TestStruct item0;
                    stack.TopOut(out item0);
                    Assert.That(item0.Int32, Is.EqualTo(78));
                    Assert.That(item0.Int64, Is.EqualTo(11));
                    Assert.That(item0.TestClass.Int32, Is.EqualTo(1));
                    Assert.That(item0.TestClass.Int64, Is.EqualTo(5776));

                    stack.Push(new TestStruct(1, 2, new TestClass(3, 897879789)));
                    TestStruct item1;
                    stack.TopOut(out item1);
                    Assert.That(item1.Int32, Is.EqualTo(1));
                    Assert.That(item1.Int64, Is.EqualTo(2));
                    Assert.That(item1.TestClass.Int32, Is.EqualTo(3));
                    Assert.That(item1.TestClass.Int64, Is.EqualTo(897879789));

                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void TopInPtrTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 2))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(2, &memory);
                    Assert.That(
                        () =>
                        {
                            using var wrap = new Tests.Struct.TestStructWrapper();
                            stack.Top(wrap.Ptr);
                        },
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("There are no elements on the stack")
                        );


                    using var inItem = new Tests.Struct.TestStructWrapper();
                    inItem.Fill(new TestStruct(45, 23, new TestClass(78, 56)));
                    stack.Push(inItem.Ptr);

                    using var item0 = new Tests.Struct.TestStructWrapper();
                    stack.Top(item0.Ptr);
                    Assert.That(item0.Int32, Is.EqualTo(45));
                    Assert.That(item0.Int64, Is.EqualTo(23));
                    Assert.That(item0.TestClass.Int32, Is.EqualTo(78));
                    Assert.That(item0.TestClass.Int64, Is.EqualTo(56));

                    using var inItem2 = new Tests.Struct.TestStructWrapper();
                    inItem.Fill(new TestStruct(15, 10, new TestClass(8, 5)));
                    stack.Push(inItem.Ptr);

                    using var item2 = new Tests.Struct.TestStructWrapper();
                    stack.Top(item2.Ptr);
                    Assert.That(item2.Int32, Is.EqualTo(15));
                    Assert.That(item2.Int64, Is.EqualTo(10));
                    Assert.That(item2.TestClass.Int32, Is.EqualTo(8));
                    Assert.That(item2.TestClass.Int64, Is.EqualTo(5));
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void TopRefValueTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 2))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(2, &memory);
                    Assert.That(
                        () =>
                        {
                            var temp = new TestStruct();
                            stack.Top(ref temp);
                        },
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("There are no elements on the stack")
                        );

                    var wrapTestClass0 = new Struct.TestClassWrapper();
                    wrapTestClass0.Int32 = 88;
                    wrapTestClass0.Int64 = 112;
                    stack.Push(new TestStruct(15, 10, new TestClass(8, 5)));
                    var wrap0 = new Struct.TestStructWrapper(stack.TopPtr(), false);
                    wrap0.TestClass2 = new IntPtr(wrapTestClass0.Ptr);
                    var item0 = new TestStruct();
                    stack.Top(ref item0);
                    Assert.That(item0.Int32, Is.EqualTo(15));
                    Assert.That(item0.Int64, Is.EqualTo(10));
                    Assert.That(item0.TestClass.Int32, Is.EqualTo(8));
                    Assert.That(item0.TestClass.Int64, Is.EqualTo(5));
                    Assert.That(item0.TestClass2.Int32, Is.EqualTo(88));
                    Assert.That(item0.TestClass2.Int64, Is.EqualTo(112));
                    wrapTestClass0.Dispose();

                    stack.Push(new TestStruct(2, 1, new TestClass(1, 7)));
                    wrapTestClass0.Int32 = 14;
                    wrapTestClass0.Int64 = 45;
                    var wrap1 = new Struct.TestStructWrapper(stack.TopPtr(), false);
                    wrap1.TestClass2 = new IntPtr(wrapTestClass0.Ptr);
                    var item1 = new TestStruct();
                    stack.Top(ref item1);
                    Assert.That(item1.Int32, Is.EqualTo(2));
                    Assert.That(item1.Int64, Is.EqualTo(1));
                    Assert.That(item1.TestClass.Int32, Is.EqualTo(1));
                    Assert.That(item1.TestClass.Int64, Is.EqualTo(7));
                    Assert.That(item1.TestClass2.Int32, Is.EqualTo(14));
                    Assert.That(item1.TestClass2.Int64, Is.EqualTo(45));
                }
            }
        }

        [Test]
        public void TopPtrTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 2))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(2, &memory);
                    Assert.That(() => stack.TopPtr(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("There are no elements on the stack")
                        );

                    stack.Push(new TestStruct(2, 1, new TestClass(1, 7)));
                    var itemPtr0 = stack.TopPtr();
                    Assert.That(new IntPtr(itemPtr0), Is.EqualTo(new IntPtr((byte*)memory.Start + 0)));

                    stack.Push(new TestStruct(2, 1, new TestClass(1, 7)));
                    var itemPtr1 = stack.TopPtr();
                    Assert.That(new IntPtr(itemPtr1), Is.EqualTo(new IntPtr((byte*)memory.Start + TestStructHelper.SizeOf)));

                }
            }
        }
    }
}