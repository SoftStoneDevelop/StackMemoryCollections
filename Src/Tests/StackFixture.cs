using NUnit.Framework;
using StackMemoryCollections;
using System;

namespace Tests
{
    [TestFixture]
    public class StackTest
    {
        [Test]
        public void DisposeStackTest()
        {
            unsafe
            {
                using (var memory = new StackMemory(sizeof(int) * 3))
                {
                    {
                        using var stack = new Stack<int>(3, &memory);
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
                using (var memory = new StackMemory(sizeof(int) * 3))
                {
                    {
                        var stack = new Stack<int>(3, &memory);
                    }

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((int*)memory.Start + 3)));
                }
            }
        }

        [Test]
        public void PushTest()
        {
            unsafe
            {
                using (var memory = new StackMemory(sizeof(int) * 3))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var stack = new Stack<int>(3, &memory);
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((int*)memory.Start + 3)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));

                    stack.Push(48);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    stack.Push(12);
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));

                    stack.Push(50);

                    Assert.That(() => stack.Push(16),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Not enough memory to allocate stack element")
                        );
                }
            }
        }

        [Test]
        public void FrontPopTest()
        {
            unsafe
            {
                using (var memory = new StackMemory(sizeof(int) * 3))
                {
                    var stack = new Stack<int>(3, &memory);
                    stack.Push(48);
                    stack.Push(12);

                    var item = stack.Front();
                    Assert.That(new IntPtr(item), Is.EqualTo(new IntPtr((int*)memory.Start + 1)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));
                    Assert.That(*item, Is.EqualTo(12));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    item = stack.Front();
                    Assert.That(new IntPtr(item), Is.EqualTo(new IntPtr((int*)memory.Start)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));
                    Assert.That(*item, Is.EqualTo(48));

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