using NUnit.Framework;
using System;

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
                    using var queue = new Tests.Class.QueueOfTestStruct(5, &memory);
                    queue.Push(new TestStruct(1, 128, new TestClass(88, 243)));//forget
                    var front = queue.Front();
                    var back = queue.Back();
                    queue.Push(new TestStruct(2, 128, new TestClass(88, 243)));//forget
                    front = queue.Front();
                    back = queue.Back();
                    queue.Push(new TestStruct(3, 128, new TestClass(88, 243)));//forget
                    front = queue.Front();
                    back = queue.Back();
                    queue.Pop();
                    front = queue.Front();
                    back = queue.Back();
                    queue.Pop();
                    front = queue.Front();
                    back = queue.Back();
                    queue.Push(new TestStruct(4, 128, new TestClass(88, 243)));
                    queue.Push(new TestStruct(5, 128, new TestClass(88, 243)));
                    queue.Pop();
                    queue.Push(new TestStruct(6, 128, new TestClass(88, 243)));
                    front = queue.Front();
                    back = queue.Back();
                }
            }
        }
    }
}