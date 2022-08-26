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
        public void ReseizeTest()
        {
            unsafe
            {
                var stack = new StackMemoryCollections.Struct.Stack<int>();
                stack.Push(5);
                stack.Push(5);
                stack.Push(5);
                stack.Push(5);
                Assert.That(stack.Size, Is.EqualTo((nuint)4));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)4));

                stack.Push(5);
                Assert.That(stack.Size, Is.EqualTo((nuint)5));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)8));
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
        public void TryPushTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
                    Assert.That(stack.TryPush(48),Is.EqualTo(true));
                    Assert.That(stack.TryPush(12), Is.EqualTo(true));
                    Assert.That(stack.TryPush(50), Is.EqualTo(true));
                    Assert.That(stack.TryPush(16), Is.EqualTo(false));
                    Assert.That(stack.TryPush(16), Is.EqualTo(false));
                }
            }
        }

        [Test]
        public void ClearTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
                    stack.Push(48);
                    stack.Push(12);
                    stack.Push(7);

                    Assert.That(stack.Size, Is.EqualTo((nuint)3));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    stack.Clear();
                    Assert.That(stack.Size, Is.EqualTo((nuint)0));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                }
            }

            unsafe
            {
                var stack = new StackMemoryCollections.Struct.Stack<int>();
                stack.Push(48);
                stack.Push(12);
                stack.Push(50);
                stack.Push(50);

                Assert.That(stack.Size, Is.EqualTo((nuint)4));
                Assert.That(stack.Capacity, Is.EqualTo((nuint)4));;
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
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                using (var memory2 = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Class.Stack<int>(3, &memory);
                    stack.Push(48);
                    stack.Push(12);
                    stack.Push(7);

                    var stack2 = new StackMemoryCollections.Class.Stack<int>(3, &memory2);
                    stack.Copy(in stack2);

                    Assert.That(stack.Size, Is.EqualTo(stack2.Size));
                    Assert.That(*stack[0], Is.EqualTo(*stack[0]));
                    Assert.That(*stack[1], Is.EqualTo(*stack[1]));
                    Assert.That(*stack[2], Is.EqualTo(*stack[2]));
                }
            }

            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                using (var memory2 = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
                    stack.Push(48);
                    stack.Push(12);
                    stack.Push(7);

                    var stack2 = new StackMemoryCollections.Struct.Stack<int>(3, &memory2);
                    stack.Copy(stack2.Start);
                    Assert.That(stack2.Size, Is.EqualTo((nuint)0));
                    stack2.Size = stack.Size;
                    Assert.That(stack.Size, Is.EqualTo(stack2.Size));

                    Assert.That(*stack[0], Is.EqualTo(*stack[0]));
                    Assert.That(*stack[1], Is.EqualTo(*stack[1]));
                    Assert.That(*stack[2], Is.EqualTo(*stack[2]));
                }
            }
        }

        [Test]
        public void TrimExcessTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(2, &memory);
                    stack.Push(48);
                    stack.Push(12);
                    stack.ExpandCapacity(1);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((int*)memory.Start + 3)));
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    stack.TrimExcess();
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)2));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((int*)memory.Start + 2)));
                }
            }

            unsafe
            {
                var stack = new StackMemoryCollections.Struct.Stack<int>();
                stack.Push(48);
                stack.Push(12);
                stack.Push(50);
                stack.Push(50);
                stack.ExpandCapacity(6);
                stack.Push(11);

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
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(2, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((int*)memory.Start + 2)));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)2));
                    stack.ExpandCapacity(1);
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((int*)memory.Start + 3)));
                }
            }

            unsafe
            {
                var stack = new StackMemoryCollections.Struct.Stack<int>();
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
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);

                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((int*)memory.Start + 3)));
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
                    stack.ReducingCapacity(1);
                    Assert.That(stack.Capacity, Is.EqualTo((nuint)2));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((int*)memory.Start + 2)));
                }
            }

            unsafe
            {
                var stack = new StackMemoryCollections.Struct.Stack<int>();
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
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
                    stack.Push(48);
                    Assert.That(stack.Size, Is.EqualTo((nuint)1));

                    stack.Push(12);
                    Assert.That(stack.Size, Is.EqualTo((nuint)2));

                    stack.Push(50);
                    Assert.That(stack.Size, Is.EqualTo((nuint)3));
                }
            }
        }

        [Test]
        public void CapacityTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(sizeof(int) * 3))
                {
                    var stack = new StackMemoryCollections.Struct.Stack<int>(3, &memory);
                    stack.Push(48);
                    stack.Pop();

                    Assert.That(stack.Capacity, Is.EqualTo((nuint)3));
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

                    Assert.That(new IntPtr(stack[0]), Is.EqualTo(new IntPtr((int*)memory.Start + 2)));
                    Assert.That(*stack[0], Is.EqualTo(50));

                    Assert.That(new IntPtr(stack[1]), Is.EqualTo(new IntPtr((int*)memory.Start + 1)));
                    Assert.That(*stack[1], Is.EqualTo(12));

                    Assert.That(new IntPtr(stack[2]), Is.EqualTo(new IntPtr((int*)memory.Start)));
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