using NUnit.Framework;
using StackMemoryCollections;
using System;
using System.Runtime.InteropServices;

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
                using (var memory = new StackMemory((nuint)Marshal.SizeOf<TestStruct>() * 3))
                {
                    {
                        using var stack = new StackOfStruct<TestStruct>(3, &memory);
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
                using (var memory = new StackMemory((nuint)Marshal.SizeOf<TestStruct>() * 3))
                {
                    {
                        var stack = new StackOfStruct<TestStruct>(3, &memory);
                    }

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((TestStruct*)memory.Start + 3)));
                }
            }
        }

        [Test]
        public void PushTest()
        {
            unsafe
            {
                using (var memory = new StackMemory((nuint)Marshal.SizeOf<TestStruct>() * 3))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var stack = new StackOfStruct<TestStruct>(3, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((TestStruct*)memory.Start + 3)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));

                    var s1 = new TestStruct(1255, 45465465654, true);
                    stack.Push(s1);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    s1 = new TestStruct(8845, 878778778787, true);
                    stack.Push(s1);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));

                    stack.Push(new TestStruct(798845, 99999955555, true));

                    Assert.That(() => stack.Push(new TestStruct(45, 788787, false)),
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
                using (var memory = new StackMemory((nuint)Marshal.SizeOf<TestStruct>() * 3))
                {
                    var stack = new StackOfStruct<TestStruct>(3, &memory);
                    var s1 = new TestStruct(1255, 45465465654, true);
                    stack.Push(s1);
                    s1 = new TestStruct(8845, 878778778787, true);
                    stack.Push(s1);
                    s1 = new TestStruct(444, 1332, false);
                    stack.Push(s1);

                    Assert.That(stack[0], Is.EqualTo(new TestStruct(444, 1332, false)));
                    Assert.That(stack[1], Is.EqualTo(new TestStruct(8845, 878778778787, true)));
                    Assert.That(stack[2], Is.EqualTo(new TestStruct(1255, 45465465654, true)));

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
                using (var memory = new StackMemory((nuint)Marshal.SizeOf<TestStruct>() * 3))
                {
                    var stack = new StackOfStruct<TestStruct>(3, &memory);
                    
                    var s1 = new TestStruct(1111, 55555555, true);
                    stack.Push(s1);

                    s1 = new TestStruct(2222, 333333333, true);
                    stack.Push(s1);

                    var item = stack.Front();
                    var itemPtr = stack.FrontPtr();
                    Assert.That(new IntPtr(itemPtr), Is.EqualTo(new IntPtr((TestStruct*)memory.Start + 1)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));
                    Assert.That(item, Is.EqualTo(new TestStruct(2222, 333333333, true)));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    item = stack.Front();
                    itemPtr = stack.FrontPtr();
                    Assert.That(new IntPtr(itemPtr), Is.EqualTo(new IntPtr((TestStruct*)memory.Start)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));
                    Assert.That(item, Is.EqualTo(new TestStruct(1111, 55555555, true)));

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