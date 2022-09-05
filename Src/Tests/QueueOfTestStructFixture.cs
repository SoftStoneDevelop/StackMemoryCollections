
using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Tests
{
    [TestFixture]
    public class QueueOfTestStructFixture
    {


        [Test]
        public void DisposeTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    {
                        using var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);
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
                        var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);
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
                var queue = new Tests.Struct.QueueOfTestStruct();

                queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));

                Assert.That(queue.Size, Is.EqualTo((nuint)4));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)4));

                queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                Assert.That(queue.Size, Is.EqualTo((nuint)5));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)8));

                queue.Dispose();

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
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(queue.IsEmpty, Is.EqualTo(true));


                    queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)1));


                    queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)2));


                    queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)3));


                    queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)4));


                    queue.Push(new TestStruct(45, 23, new TestClass(78, 56)));
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)5));


                    Assert.That(() => queue.Push(new TestStruct(45, 23, new TestClass(78, 56))),
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
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(queue.IsEmpty, Is.EqualTo(true));

                    var wrap = new Tests.Struct.TestStructWrapper(queue.BackFuture(), false);
                    wrap.Fill(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.PushFuture();
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)1));

                    var front = queue.Front();
                    var back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(1));
                    Assert.That(back.Int64, Is.EqualTo(1));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(11));

                    wrap.ChangePtr(queue.BackFuture());
                    wrap.Fill(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.PushFuture();
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)2));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(2));
                    Assert.That(back.Int64, Is.EqualTo(2));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(22));


                    wrap.ChangePtr(queue.BackFuture());
                    wrap.Fill(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.PushFuture();

                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)3));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(3));
                    Assert.That(back.Int64, Is.EqualTo(3));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(33));


                    wrap.ChangePtr(queue.BackFuture());
                    wrap.Fill(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.PushFuture();

                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)4));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(4));
                    Assert.That(back.Int64, Is.EqualTo(4));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(44));


                    wrap.ChangePtr(queue.BackFuture());
                    wrap.Fill(new TestStruct(5, 5, null));
                    queue.PushFuture();
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)5));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass, Is.EqualTo(null));

                    Assert.That(() => queue.BackFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Future element not available")
                        );

                    Assert.That(() => queue.PushFuture(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Not enough memory to allocate queue element")
                        );
                }
            }
        }

        [Test]
        public void PushPtrTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(queue.IsEmpty, Is.EqualTo(true));

                    using var wrap = new Tests.Class.TestStructWrapper();
                    wrap.Fill(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(wrap.Ptr);
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)1));

                    var front = queue.Front();
                    var back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(1));
                    Assert.That(back.Int64, Is.EqualTo(1));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(11));

                    wrap.Fill(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(wrap.Ptr);
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)2));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(2));
                    Assert.That(back.Int64, Is.EqualTo(2));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(22));

                    wrap.Fill(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(wrap.Ptr);

                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)3));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(3));
                    Assert.That(back.Int64, Is.EqualTo(3));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(33));

                    wrap.Fill(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.Push(wrap.Ptr);

                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)4));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(4));
                    Assert.That(back.Int64, Is.EqualTo(4));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(44));

                    wrap.Fill(new TestStruct(5, 5, null));
                    queue.Push(wrap.Ptr);
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)5));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass, Is.EqualTo(null));

                    Assert.That(() => 
                    {
                        queue.Push(wrap.Ptr); 
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
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));
                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));
                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));
                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));
                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(true));

                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
                    Assert.That(queue.TryPush(new TestStruct(1, 1, new TestClass(11, 11))), Is.EqualTo(false));
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
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);
                    using var wrap = new Tests.Class.TestStructWrapper();
                    wrap.Fill(new TestStruct(1, 1, new TestClass(11, 11)));

                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));

                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(false));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(false));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(false));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(false));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(false));
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
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);
                    using var wrap = new Tests.Class.TestStructWrapper();
                    wrap.Fill(new TestStruct(1, 1, new TestClass(11, 11)));

                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                    Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));

                    Assert.That(queue.Size, Is.EqualTo((nuint)5));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    queue.Clear();
                    Assert.That(queue.Size, Is.EqualTo((nuint)0));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                }
            }
        }

        [Test]
        public void ClearOwnTest()
        {
            unsafe
            {
                using var queue = new Tests.Struct.QueueOfTestStruct();
                using var wrap = new Tests.Class.TestStructWrapper();
                wrap.Fill(new TestStruct(1, 1, new TestClass(11, 11)));

                Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));
                Assert.That(queue.TryPush(wrap.Ptr), Is.EqualTo(true));

                Assert.That(queue.Size, Is.EqualTo((nuint)4));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)4));
                queue.Clear();
                Assert.That(queue.Size, Is.EqualTo((nuint)0));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)4));
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
                    var queue2 = new Tests.Struct.QueueOfTestStruct(5, &memory2);
                    using var wrap = new Tests.Class.TestStructWrapper();

                    {
                        using var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);
                        wrap.Fill(new TestStruct(1, 1, new TestClass(11, 11)));
                        queue.Push(wrap.Ptr);

                        wrap.Fill(new TestStruct(2, 2, new TestClass(22, 22)));
                        queue.Push(wrap.Ptr);

                        wrap.Fill(new TestStruct(3, 3, new TestClass(33, 33)));
                        queue.Push(wrap.Ptr);

                        wrap.Fill(new TestStruct(4, 4, new TestClass(44, 44)));
                        queue.Push(wrap.Ptr);

                        wrap.Fill(new TestStruct(5, 5, new TestClass(55, 55)));
                        queue.Push(wrap.Ptr);

                        queue.Copy(queue2.Start);
                        Assert.That(queue2.Size, Is.EqualTo((nuint)0));
                        queue.GetPositions(out var head, out var tail, out var size);
                        queue2.SetPositions(head, tail, size);
                        Assert.That(queue.Size, Is.EqualTo(queue2.Size));
                    }

                    var front = queue2.Front();
                    var back = queue2.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue2.Pop();

                    front = queue2.Front();
                    back = queue2.Back();
                    Assert.That(front.Int32, Is.EqualTo(2));
                    Assert.That(front.Int64, Is.EqualTo(2));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(22));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue2.Pop();

                    front = queue2.Front();
                    back = queue2.Back();
                    Assert.That(front.Int32, Is.EqualTo(3));
                    Assert.That(front.Int64, Is.EqualTo(3));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue2.Pop();

                    front = queue2.Front();
                    back = queue2.Back();
                    Assert.That(front.Int32, Is.EqualTo(4));
                    Assert.That(front.Int64, Is.EqualTo(4));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue2.Pop();

                    front = queue2.Front();
                    back = queue2.Back();
                    Assert.That(front.Int32, Is.EqualTo(5));
                    Assert.That(front.Int64, Is.EqualTo(5));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(55));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue2.Pop();

                    Assert.That(queue2.Size, Is.EqualTo((nuint)0));
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
                    var queue = new Tests.Struct.QueueOfTestStruct(3, &memory);

                    queue.Push(new TestStruct(45, 60, new TestClass(11, 22)));
                    queue.Push(new TestStruct(45, 60, new TestClass(11, 22)));
                    queue.Push(new TestStruct(45, 60, new TestClass(11, 22)));
                    queue.ExpandCapacity(2);
                    queue.Push(new TestStruct(45, 60, new TestClass(11, 22)));

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(queue.Size, Is.EqualTo((nuint)4));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    queue.TrimExcess();
                    Assert.That(queue.Size, Is.EqualTo((nuint)4));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)4));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 4))));
                }
            }
        }

        [Test]
        public void TrimExcessOwnTest()
        {
            unsafe
            {
                using var queue = new Tests.Struct.QueueOfTestStruct();

                queue.Push(new TestStruct(45, 60, new TestClass(11, 22)));
                queue.Push(new TestStruct(45, 60, new TestClass(11, 22)));
                queue.Push(new TestStruct(45, 60, new TestClass(11, 22)));
                queue.Push(new TestStruct(45, 60, new TestClass(11, 22)));

                queue.ExpandCapacity(6);
                queue.Push(new TestStruct(45, 60, new TestClass(11, 22)));

                Assert.That(queue.Size, Is.EqualTo((nuint)5));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)10));
                queue.TrimExcess();
                Assert.That(queue.Size, Is.EqualTo((nuint)5));
                Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
            }
        }

        [Test]
        public void ExpandCapacityTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 8))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    queue.ExpandCapacity(3);
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)8));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 8))));
                }
            }

            unsafe
            {
                using var queue = new Tests.Struct.QueueOfTestStruct();
                Assert.That(queue.Capacity, Is.EqualTo((nuint)4));
                queue.ExpandCapacity(6);
                Assert.That(queue.Capacity, Is.EqualTo((nuint)10));
            }
        }

        [Test]
        public void ExpandCapacityHeadAfterTailTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 8))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                    queue.Pop();
                    queue.Pop();
                    queue.Pop();

                    queue.Push(new TestStruct(6, 6, new TestClass(66, 66)));
                    queue.Push(new TestStruct(7, 7, new TestClass(77, 77)));
                    queue.Push(new TestStruct(8, 8, new TestClass(88, 88)));

                    queue.ExpandCapacity(3);
                    var front = queue.Front();
                    var back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(4));
                    Assert.That(front.Int64, Is.EqualTo(4));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                    Assert.That(back.Int32, Is.EqualTo(8));
                    Assert.That(back.Int64, Is.EqualTo(8));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(88));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(88));

                    queue.Push(new TestStruct(9, 9, new TestClass(99, 99)));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(4));
                    Assert.That(front.Int64, Is.EqualTo(4));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                    Assert.That(back.Int32, Is.EqualTo(9));
                    Assert.That(back.Int64, Is.EqualTo(9));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(99));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(99));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(5));
                    Assert.That(front.Int64, Is.EqualTo(5));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(55));
                    Assert.That(back.Int32, Is.EqualTo(9));
                    Assert.That(back.Int64, Is.EqualTo(9));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(99));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(99));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(6));
                    Assert.That(front.Int64, Is.EqualTo(6));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(66));
                    Assert.That(back.Int32, Is.EqualTo(9));
                    Assert.That(back.Int64, Is.EqualTo(9));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(99));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(99));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(7));
                    Assert.That(front.Int64, Is.EqualTo(7));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(77));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(77));
                    Assert.That(back.Int32, Is.EqualTo(9));
                    Assert.That(back.Int64, Is.EqualTo(9));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(99));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(99));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(8));
                    Assert.That(front.Int64, Is.EqualTo(8));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(88));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(88));
                    Assert.That(back.Int32, Is.EqualTo(9));
                    Assert.That(back.Int64, Is.EqualTo(9));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(99));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(99));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(9));
                    Assert.That(front.Int64, Is.EqualTo(9));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(99));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(99));
                    Assert.That(back.Int32, Is.EqualTo(9));
                    Assert.That(back.Int64, Is.EqualTo(9));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(99));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(99));
                    queue.Pop();

                    Assert.That(queue.Size, Is.EqualTo((nuint)0));
                }
            }
        }

        [Test]
        public void ExpandCapacityHeadAfterTailOwnTest()
        {
            unsafe
            {
                using var queue = new Tests.Struct.QueueOfTestStruct();

                queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));

                queue.Pop();
                queue.Pop();

                queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));
                queue.Push(new TestStruct(6, 6, new TestClass(66, 66)));

                queue.ExpandCapacity(3);
                queue.Push(new TestStruct(7, 7, new TestClass(77, 77)));

                var front = queue.Front();
                var back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(3));
                Assert.That(front.Int64, Is.EqualTo(3));
                Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                Assert.That(back.Int32, Is.EqualTo(7));
                Assert.That(back.Int64, Is.EqualTo(7));
                Assert.That(back.TestClass.Int32, Is.EqualTo(77));
                Assert.That(back.TestClass.Int64, Is.EqualTo(77));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(4));
                Assert.That(front.Int64, Is.EqualTo(4));
                Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                Assert.That(back.Int32, Is.EqualTo(7));
                Assert.That(back.Int64, Is.EqualTo(7));
                Assert.That(back.TestClass.Int32, Is.EqualTo(77));
                Assert.That(back.TestClass.Int64, Is.EqualTo(77));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(5));
                Assert.That(front.Int64, Is.EqualTo(5));
                Assert.That(front.TestClass.Int32, Is.EqualTo(55));
                Assert.That(front.TestClass.Int64, Is.EqualTo(55));
                Assert.That(back.Int32, Is.EqualTo(7));
                Assert.That(back.Int64, Is.EqualTo(7));
                Assert.That(back.TestClass.Int32, Is.EqualTo(77));
                Assert.That(back.TestClass.Int64, Is.EqualTo(77));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(6));
                Assert.That(front.Int64, Is.EqualTo(6));
                Assert.That(front.TestClass.Int32, Is.EqualTo(66));
                Assert.That(front.TestClass.Int64, Is.EqualTo(66));
                Assert.That(back.Int32, Is.EqualTo(7));
                Assert.That(back.Int64, Is.EqualTo(7));
                Assert.That(back.TestClass.Int32, Is.EqualTo(77));
                Assert.That(back.TestClass.Int64, Is.EqualTo(77));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(7));
                Assert.That(front.Int64, Is.EqualTo(7));
                Assert.That(front.TestClass.Int32, Is.EqualTo(77));
                Assert.That(front.TestClass.Int64, Is.EqualTo(77));
                Assert.That(back.Int32, Is.EqualTo(7));
                Assert.That(back.Int64, Is.EqualTo(7));
                Assert.That(back.TestClass.Int32, Is.EqualTo(77));
                Assert.That(back.TestClass.Int64, Is.EqualTo(77));
                queue.Pop();

                Assert.That(queue.Size, Is.EqualTo((nuint)0));
            }
        }

        [Test]
        public void ExpandCapacityHeadBeforeTailTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 8))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                    queue.ExpandCapacity(3);

                    var front = queue.Front();
                    var back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));

                    queue.Push(new TestStruct(6, 6, new TestClass(66, 66)));

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(2));
                    Assert.That(front.Int64, Is.EqualTo(2));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(22));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(3));
                    Assert.That(front.Int64, Is.EqualTo(3));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(4));
                    Assert.That(front.Int64, Is.EqualTo(4));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(5));
                    Assert.That(front.Int64, Is.EqualTo(5));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(55));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(6));
                    Assert.That(front.Int64, Is.EqualTo(6));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(66));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    Assert.That(queue.Size, Is.EqualTo((nuint)0));
                }
            }
        }

        [Test]
        public void ExpandCapacityHeadBeforeTailOwnTest()
        {
            unsafe
            {
                var queue = new Tests.Struct.QueueOfTestStruct();

                queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));

                queue.ExpandCapacity(3);

                var front = queue.Front();
                var back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(1));
                Assert.That(front.Int64, Is.EqualTo(1));
                Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                Assert.That(back.Int32, Is.EqualTo(4));
                Assert.That(back.Int64, Is.EqualTo(4));
                Assert.That(back.TestClass.Int32, Is.EqualTo(44));
                Assert.That(back.TestClass.Int64, Is.EqualTo(44));

                queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(1));
                Assert.That(front.Int64, Is.EqualTo(1));
                Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                Assert.That(back.Int32, Is.EqualTo(5));
                Assert.That(back.Int64, Is.EqualTo(5));
                Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(2));
                Assert.That(front.Int64, Is.EqualTo(2));
                Assert.That(front.TestClass.Int32, Is.EqualTo(22));
                Assert.That(front.TestClass.Int64, Is.EqualTo(22));
                Assert.That(back.Int32, Is.EqualTo(5));
                Assert.That(back.Int64, Is.EqualTo(5));
                Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(3));
                Assert.That(front.Int64, Is.EqualTo(3));
                Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                Assert.That(back.Int32, Is.EqualTo(5));
                Assert.That(back.Int64, Is.EqualTo(5));
                Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(4));
                Assert.That(front.Int64, Is.EqualTo(4));
                Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                Assert.That(back.Int32, Is.EqualTo(5));
                Assert.That(back.Int64, Is.EqualTo(5));
                Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(5));
                Assert.That(front.Int64, Is.EqualTo(5));
                Assert.That(front.TestClass.Int32, Is.EqualTo(55));
                Assert.That(front.TestClass.Int64, Is.EqualTo(55));
                Assert.That(back.Int32, Is.EqualTo(5));
                Assert.That(back.Int64, Is.EqualTo(5));
                Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                queue.Pop();

                Assert.That(queue.Size, Is.EqualTo((nuint)0));
            }
        }

        [Test]
        public void ReducingCapacityTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 5))));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    queue.ReducingCapacity(1);
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)4));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 4))));
                }
            }

            unsafe
            {
                using var queue = new Tests.Struct.QueueOfTestStruct();
                queue.ExpandCapacity(6);

                Assert.That(queue.Capacity, Is.EqualTo((nuint)10));
                queue.ReducingCapacity(1);
                Assert.That(queue.Capacity, Is.EqualTo((nuint)9));
            }
        }

        [Test]
        public void ReducingCapacityHeadAfterTailTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                    queue.Pop();
                    queue.Pop();

                    queue.Push(new TestStruct(6, 6, new TestClass(66, 66)));

                    queue.ReducingCapacity(1);

                    var front = queue.Front();
                    var back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(3));
                    Assert.That(front.Int64, Is.EqualTo(3));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(4));
                    Assert.That(front.Int64, Is.EqualTo(4));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(5));
                    Assert.That(front.Int64, Is.EqualTo(5));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(55));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(6));
                    Assert.That(front.Int64, Is.EqualTo(6));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(66));
                    Assert.That(back.Int32, Is.EqualTo(6));
                    Assert.That(back.Int64, Is.EqualTo(6));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                    queue.Pop();

                    Assert.That(queue.Size, Is.EqualTo((nuint)0));
                }
            }
        }

        [Test]
        public void ReducingCapacityHeadAfterTailOwnTest()
        {
            unsafe
            {
                var queue = new Tests.Struct.QueueOfTestStruct();

                queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));

                queue.ExpandCapacity(1);

                queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                queue.Pop();
                queue.Pop();

                queue.Push(new TestStruct(6, 6, new TestClass(66, 66)));

                queue.ReducingCapacity(1);

                var front = queue.Front();
                var back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(3));
                Assert.That(front.Int64, Is.EqualTo(3));
                Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                Assert.That(back.Int32, Is.EqualTo(6));
                Assert.That(back.Int64, Is.EqualTo(6));
                Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(4));
                Assert.That(front.Int64, Is.EqualTo(4));
                Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                Assert.That(back.Int32, Is.EqualTo(6));
                Assert.That(back.Int64, Is.EqualTo(6));
                Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(5));
                Assert.That(front.Int64, Is.EqualTo(5));
                Assert.That(front.TestClass.Int32, Is.EqualTo(55));
                Assert.That(front.TestClass.Int64, Is.EqualTo(55));
                Assert.That(back.Int32, Is.EqualTo(6));
                Assert.That(back.Int64, Is.EqualTo(6));
                Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(6));
                Assert.That(front.Int64, Is.EqualTo(6));
                Assert.That(front.TestClass.Int32, Is.EqualTo(66));
                Assert.That(front.TestClass.Int64, Is.EqualTo(66));
                Assert.That(back.Int32, Is.EqualTo(6));
                Assert.That(back.Int64, Is.EqualTo(6));
                Assert.That(back.TestClass.Int32, Is.EqualTo(66));
                Assert.That(back.TestClass.Int64, Is.EqualTo(66));
                queue.Pop();

                Assert.That(queue.Size, Is.EqualTo((nuint)0));
            }
        }

        [Test]
        public void ReducingCapacityHeadBeforeTailTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));

                    queue.Pop();

                    queue.ReducingCapacity(2);

                    var front = queue.Front();
                    var back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(2));
                    Assert.That(front.Int64, Is.EqualTo(2));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(22));
                    Assert.That(back.Int32, Is.EqualTo(4));
                    Assert.That(back.Int64, Is.EqualTo(4));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(44));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(3));
                    Assert.That(front.Int64, Is.EqualTo(3));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                    Assert.That(back.Int32, Is.EqualTo(4));
                    Assert.That(back.Int64, Is.EqualTo(4));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(44));
                    queue.Pop();

                    front = queue.Front();
                    back = queue.Back();
                    Assert.That(front.Int32, Is.EqualTo(4));
                    Assert.That(front.Int64, Is.EqualTo(4));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                    Assert.That(back.Int32, Is.EqualTo(4));
                    Assert.That(back.Int64, Is.EqualTo(4));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(44));
                    queue.Pop();

                    Assert.That(queue.Size, Is.EqualTo((nuint)0));
                }
            }
        }

        [Test]
        public void ReducingCapacityHeadBeforeTailOwnTest()
        {
            unsafe
            {
                var queue = new Tests.Struct.QueueOfTestStruct();

                queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));

                queue.Pop();

                queue.ReducingCapacity(2);

                var front = queue.Front();
                var back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(2));
                Assert.That(front.Int64, Is.EqualTo(2));
                Assert.That(front.TestClass.Int32, Is.EqualTo(22));
                Assert.That(front.TestClass.Int64, Is.EqualTo(22));
                Assert.That(back.Int32, Is.EqualTo(3));
                Assert.That(back.Int64, Is.EqualTo(3));
                Assert.That(back.TestClass.Int32, Is.EqualTo(33));
                Assert.That(back.TestClass.Int64, Is.EqualTo(33));
                queue.Pop();

                front = queue.Front();
                back = queue.Back();
                Assert.That(front.Int32, Is.EqualTo(3));
                Assert.That(front.Int64, Is.EqualTo(3));
                Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                Assert.That(back.Int32, Is.EqualTo(3));
                Assert.That(back.Int64, Is.EqualTo(3));
                Assert.That(back.TestClass.Int32, Is.EqualTo(33));
                Assert.That(back.TestClass.Int64, Is.EqualTo(33));
                queue.Pop();

                Assert.That(queue.Size, Is.EqualTo((nuint)0));
            }
        }

        [Test]
        public void SizeTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    Assert.That(queue.Size, Is.EqualTo((nuint)1));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    Assert.That(queue.Size, Is.EqualTo((nuint)2));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    Assert.That(queue.Size, Is.EqualTo((nuint)3));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    Assert.That(queue.Size, Is.EqualTo((nuint)4));

                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));
                    Assert.That(queue.Size, Is.EqualTo((nuint)5));
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
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Pop();
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
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
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                    Assert.That(new IntPtr(queue[0]), Is.EqualTo(new IntPtr((byte*)memory.Start)));
                    Assert.That(new IntPtr(queue[1]), Is.EqualTo(new IntPtr((byte*)memory.Start + TestStructHelper.SizeOf)));
                    Assert.That(new IntPtr(queue[2]), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 2))));
                    Assert.That(new IntPtr(queue[3]), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 3))));
                    Assert.That(new IntPtr(queue[4]), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 4))));

                    Assert.That(() => queue[5],
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Element outside the queue")
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
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                    queue.Pop();
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)4));

                    queue.Pop();
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)3));

                    queue.Pop();
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)2));

                    queue.Pop();
                    Assert.That(queue.IsEmpty, Is.EqualTo(false));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)1));

                    queue.Pop();
                    Assert.That(queue.IsEmpty, Is.EqualTo(true));
                    Assert.That(queue.Capacity, Is.EqualTo((nuint)5));
                    Assert.That(queue.Size, Is.EqualTo((nuint)0));

                    Assert.That(() => queue.Pop(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("There are no elements on the queue")
                        );
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void FrontBackOutTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                    TestStruct front;
                    TestStruct back;
                    queue.FrontOut(out front);
                    queue.BackOut(out back);
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.FrontOut(out front);
                    queue.BackOut(out back);
                    Assert.That(front.Int32, Is.EqualTo(2));
                    Assert.That(front.Int64, Is.EqualTo(2));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(22));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.FrontOut(out front);
                    queue.BackOut(out back);
                    Assert.That(front.Int32, Is.EqualTo(3));
                    Assert.That(front.Int64, Is.EqualTo(3));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.FrontOut(out front);
                    queue.BackOut(out back);
                    Assert.That(front.Int32, Is.EqualTo(4));
                    Assert.That(front.Int64, Is.EqualTo(4));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.FrontOut(out front);
                    queue.BackOut(out back);
                    Assert.That(front.Int32, Is.EqualTo(5));
                    Assert.That(front.Int64, Is.EqualTo(5));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(55));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void FrontBackPtrTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                    using var frontW = new Tests.Struct.TestStructWrapper();
                    using var backW = new Tests.Struct.TestStructWrapper();

                    queue.Front(frontW.Ptr);
                    queue.Back(backW.Ptr);
                    Assert.That(frontW.Int32, Is.EqualTo(1));
                    Assert.That(frontW.Int64, Is.EqualTo(1));
                    Assert.That(frontW.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(frontW.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(backW.Int32, Is.EqualTo(5));
                    Assert.That(backW.Int64, Is.EqualTo(5));
                    Assert.That(backW.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(backW.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.Front(frontW.Ptr);
                    queue.Back(backW.Ptr);
                    Assert.That(frontW.Int32, Is.EqualTo(2));
                    Assert.That(frontW.Int64, Is.EqualTo(2));
                    Assert.That(frontW.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(frontW.TestClass.Int64, Is.EqualTo(22));
                    Assert.That(backW.Int32, Is.EqualTo(5));
                    Assert.That(backW.Int64, Is.EqualTo(5));
                    Assert.That(backW.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(backW.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.Front(frontW.Ptr);
                    queue.Back(backW.Ptr);
                    Assert.That(frontW.Int32, Is.EqualTo(3));
                    Assert.That(frontW.Int64, Is.EqualTo(3));
                    Assert.That(frontW.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(frontW.TestClass.Int64, Is.EqualTo(33));
                    Assert.That(backW.Int32, Is.EqualTo(5));
                    Assert.That(backW.Int64, Is.EqualTo(5));
                    Assert.That(backW.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(backW.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.Front(frontW.Ptr);
                    queue.Back(backW.Ptr);
                    Assert.That(frontW.Int32, Is.EqualTo(4));
                    Assert.That(frontW.Int64, Is.EqualTo(4));
                    Assert.That(frontW.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(frontW.TestClass.Int64, Is.EqualTo(44));
                    Assert.That(backW.Int32, Is.EqualTo(5));
                    Assert.That(backW.Int64, Is.EqualTo(5));
                    Assert.That(backW.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(backW.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.Front(frontW.Ptr);
                    queue.Back(backW.Ptr);
                    Assert.That(frontW.Int32, Is.EqualTo(5));
                    Assert.That(frontW.Int64, Is.EqualTo(5));
                    Assert.That(frontW.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(frontW.TestClass.Int64, Is.EqualTo(55));
                    Assert.That(backW.Int32, Is.EqualTo(5));
                    Assert.That(backW.Int64, Is.EqualTo(5));
                    Assert.That(backW.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(backW.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();
                }
            }
        }

        [Test]
        [SkipLocalsInit]
        public void FrontBackRefValueTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));

                    TestStruct front;
                    Unsafe.SkipInit(out front);
                    TestStruct back;
                    Unsafe.SkipInit(out back);

                    queue.Front(ref front);
                    queue.Back(ref back);
                    Assert.That(front.Int32, Is.EqualTo(1));
                    Assert.That(front.Int64, Is.EqualTo(1));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(11));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(11));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.Front(ref front);
                    queue.Back(ref back);
                    Assert.That(front.Int32, Is.EqualTo(2));
                    Assert.That(front.Int64, Is.EqualTo(2));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(22));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(22));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.Front(ref front);
                    queue.Back(ref back);
                    Assert.That(front.Int32, Is.EqualTo(3));
                    Assert.That(front.Int64, Is.EqualTo(3));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(33));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(33));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.Front(ref front);
                    queue.Back(ref back);
                    Assert.That(front.Int32, Is.EqualTo(4));
                    Assert.That(front.Int64, Is.EqualTo(4));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(44));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(44));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();

                    queue.Front(ref front);
                    queue.Back(ref back);
                    Assert.That(front.Int32, Is.EqualTo(5));
                    Assert.That(front.Int64, Is.EqualTo(5));
                    Assert.That(front.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(front.TestClass.Int64, Is.EqualTo(55));
                    Assert.That(back.Int32, Is.EqualTo(5));
                    Assert.That(back.Int64, Is.EqualTo(5));
                    Assert.That(back.TestClass.Int32, Is.EqualTo(55));
                    Assert.That(back.TestClass.Int64, Is.EqualTo(55));
                    queue.Pop();
                }
            }
        }

        [Test]
        public void GetFrontBackPtrTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.SizeOf * 5))
                {
                    var queue = new Tests.Struct.QueueOfTestStruct(5, &memory);

                    queue.Push(new TestStruct(1, 1, new TestClass(11, 11)));
                    var itemPtr0Front = queue.FrontPtr();
                    var itemPtr0Back = queue.BackPtr();
                    Assert.That(new IntPtr(itemPtr0Back), Is.EqualTo(new IntPtr((byte*)memory.Start)));
                    Assert.That(new IntPtr(itemPtr0Front), Is.EqualTo(new IntPtr((byte*)memory.Start)));

                    queue.Push(new TestStruct(2, 2, new TestClass(22, 22)));
                    var itemPtr1Front = queue.FrontPtr();
                    var itemPtr1Back = queue.BackPtr();
                    Assert.That(new IntPtr(itemPtr1Front), Is.EqualTo(new IntPtr((byte*)memory.Start)));
                    Assert.That(new IntPtr(itemPtr1Back), Is.EqualTo(new IntPtr((byte*)memory.Start + TestStructHelper.SizeOf)));

                    queue.Push(new TestStruct(3, 3, new TestClass(33, 33)));
                    var itemPtr2Front = queue.FrontPtr();
                    var itemPtr2Back = queue.BackPtr();
                    Assert.That(new IntPtr(itemPtr2Front), Is.EqualTo(new IntPtr((byte*)memory.Start)));
                    Assert.That(new IntPtr(itemPtr2Back), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 2))));

                    queue.Push(new TestStruct(4, 4, new TestClass(44, 44)));
                    var itemPtr3Front = queue.FrontPtr();
                    var itemPtr3Back = queue.BackPtr();
                    Assert.That(new IntPtr(itemPtr3Front), Is.EqualTo(new IntPtr((byte*)memory.Start)));
                    Assert.That(new IntPtr(itemPtr3Back), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 3))));

                    queue.Push(new TestStruct(5, 5, new TestClass(55, 55)));
                    var itemPtr4Front = queue.FrontPtr();
                    var itemPtr4Back = queue.BackPtr();
                    Assert.That(new IntPtr(itemPtr4Front), Is.EqualTo(new IntPtr((byte*)memory.Start)));
                    Assert.That(new IntPtr(itemPtr4Back), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.SizeOf * 4))));
                }
            }
        }


    }
}