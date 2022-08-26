using NUnit.Framework;
using System;

namespace Tests
{
    [TestFixture]
    public class StackIntTest
    {
        [Test]
        public void DisposeStackTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    {
                        using var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
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
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    {
                        var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
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
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(memory.Start)));
                    var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
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
        public void IndexTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
                    stack.Push(48);
                    stack.Push(12);
                    stack.Push(50);

                    Assert.That(*stack[0], Is.EqualTo(50));
                    Assert.That(*stack[1], Is.EqualTo(12));
                    Assert.That(*stack[2], Is.EqualTo(48));

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
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
                    stack.Push(48);
                    stack.Push(12);

                    var item = stack.Top();
                    var itemPtr = stack.TopPtr();
                    Assert.That(new IntPtr(itemPtr), Is.EqualTo(new IntPtr((int*)memory.Start + 1)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));
                    Assert.That(item, Is.EqualTo(12));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    item = stack.Top();
                    itemPtr = stack.TopPtr();
                    Assert.That(new IntPtr(itemPtr), Is.EqualTo(new IntPtr((int*)memory.Start)));
                    Assert.That(stack.IsEmpty, Is.EqualTo(false));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));
                    Assert.That(item, Is.EqualTo(48));

                    stack.Pop();
                    Assert.That(stack.IsEmpty, Is.EqualTo(true));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(stack.Size, Is.EqualTo((nuint)0));

                    Assert.That(() => stack.Top(),
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