using NUnit.Framework;
using System;

namespace Tests
{
    [TestFixture]
    public class StackStructFixture
    {
        [Test]
        public void DisposeStackTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.GetSize() * 3))
                {
                    {
                        using var stack = new Tests.Struct.StackOfTestStruct(3, &memory);
                    }

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                }
            }
        }

        [Test]
        public void NotDisposeStackTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.GetSize() * 3))
                {
                    {
                        var stack = new Tests.Struct.StackOfTestStruct(3, &memory);
                    }

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + (TestStructHelper.GetSize() * 3))));
                }
            }
        }

        [Test]
        public void PushTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.GetSize() * 3))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var stack = new Tests.Struct.StackOfTestStruct(3, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start +(TestStructHelper.GetSize() * 3))));
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));

                    var s1 = new TestStruct { Int32 = 1255, Int64 = 45465465654 };
                    stack.Push(in s1);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    s1.Int32 = 8845;
                    s1.Int64 = 878778778787;
                    stack.Push(in s1);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));

                    stack.Push(new TestStruct { Int32 = 798845, Int64 = 99999955555 });

                    Assert.That(() => stack.Push(new TestStruct { Int32 = 45, Int64 = 788787 }),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Not enough memory to allocate stack element")
                        );
                }
            }
        }

        [Test]
        public void IndexTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.GetSize() * 3))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(3, &memory);
                    var s1 = new TestStruct { Int32 = 1255, Int64 = 45465465654 };
                    stack.Push(in s1);
                    s1.Int32 = 8845;
                    s1.Int64 = 878778778787;
                    stack.Push(in s1);
                    s1.Int32 = 444;
                    s1.Int64 = 1332;
                    stack.Push(in s1);

                    var val = new TestStruct();
                    TestStructHelper.CopyToValue(stack[0], ref val);
                    Assert.That(val, Is.EqualTo(new TestStruct { Int32 = 444, Int64 = 1332 }));

                    val = new TestStruct();
                    TestStructHelper.CopyToValue(stack[1], ref val);
                    Assert.That(val, Is.EqualTo(new TestStruct { Int32 = 8845, Int64 = 878778778787 }));

                    val = new TestStruct();
                    TestStructHelper.CopyToValue(stack[2], ref val);
                    Assert.That(val, Is.EqualTo(new TestStruct { Int32 = 1255, Int64 = 45465465654 }));

                    Assert.That(() => stack[3],
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Element outside the stack")
                        );
                }
            }
        }

        [Test]
        public void FrontPopTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(TestStructHelper.GetSize() * 3))
                {
                    var stack = new Tests.Struct.StackOfTestStruct(3, &memory);
                    
                    var s1 = new TestStruct { Int32 = 1111, Int64 = 55555555 };
                    stack.Push(in s1);

                    s1.Int32 = 2222;
                    s1.Int64 = 333333333;
                    stack.Push(in s1);

                    var item = stack.Front();
                    var itemPtr = stack.FrontPtr();
                    Assert.That(new IntPtr(itemPtr), Is.EqualTo(new IntPtr((byte*)memory.Start + TestStructHelper.GetSize())));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));
                    Assert.That(item, Is.EqualTo(new TestStruct { Int32 = 2222, Int64 = 333333333 }));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    item = stack.Front();
                    itemPtr = stack.FrontPtr();
                    Assert.That(new IntPtr(itemPtr), Is.EqualTo(new IntPtr((byte*)memory.Start)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));
                    Assert.That(item, Is.EqualTo(new TestStruct { Int32 = 1111, Int64 = 55555555 }));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)0));

                    Assert.That(() => stack.Front(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("There are no elements on the stack")
                        );

                    Assert.That(() => stack.Pop(),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("There are no elements on the stack")
                        );
                }
            }
        }
    }
}