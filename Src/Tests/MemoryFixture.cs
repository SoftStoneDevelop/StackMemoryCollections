using NUnit.Framework;
using System;

namespace Tests
{
    [TestFixture]
    public class MemoryFixture
    {
        [Test]
        public void AllocateTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(6))
                {
                    var start = memory.Start;

                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)6));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start)));

                    var current0 = memory.Current;
                    var mPtr0 = memory.AllocateMemory(1);
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)5));
                    Assert.That(new IntPtr(current0), Is.EqualTo(new IntPtr(mPtr0)));
                    Assert.That(new IntPtr(start), Is.EqualTo(new IntPtr(memory.Start)));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + 1)));

                    var current1 = memory.Current;
                    var mPtr1 = memory.AllocateMemory(5);
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)0));
                    Assert.That(new IntPtr(current1), Is.EqualTo(new IntPtr(mPtr1)));
                    Assert.That(new IntPtr(start), Is.EqualTo(new IntPtr(memory.Start)));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + 6)));

                    Assert.That(() => memory.AllocateMemory(1),
                        Throws.Exception.TypeOf(typeof(ArgumentException))
                        .And.Message.EqualTo("Can't allocate memory")
                        );
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)0));
                    Assert.That(new IntPtr(current1), Is.EqualTo(new IntPtr(mPtr1)));
                    Assert.That(new IntPtr(start), Is.EqualTo(new IntPtr(memory.Start)));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + 6)));
                }
            }
        }

        [Test]
        public void TryAllocateTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(6))
                {
                    var start = memory.Start;

                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)6));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start)));

                    var current0 = memory.Current;
                    var result0 = memory.TryAllocateMemory(1, out var mPtr0);
                    Assert.That(result0, Is.EqualTo(true));
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)5));
                    Assert.That(new IntPtr(current0), Is.EqualTo(new IntPtr(mPtr0)));
                    Assert.That(new IntPtr(start), Is.EqualTo(new IntPtr(memory.Start)));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + 1)));

                    var current1 = memory.Current;
                    var result1 = memory.TryAllocateMemory(5, out var mPtr1);
                    Assert.That(result1, Is.EqualTo(true));
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)0));
                    Assert.That(new IntPtr(current1), Is.EqualTo(new IntPtr(mPtr1)));
                    Assert.That(new IntPtr(start), Is.EqualTo(new IntPtr(memory.Start)));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)memory.Start + 6)));

                    var current2 = memory.Current;
                    var result2 = memory.TryAllocateMemory(5, out var mPtr2);
                    Assert.That(result2, Is.EqualTo(false));
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)0));
                    Assert.That(new IntPtr(current2), Is.EqualTo(new IntPtr(memory.Current)));
                    Assert.That(new IntPtr(start), Is.EqualTo(new IntPtr(memory.Start)));
                    Assert.That(new IntPtr(mPtr2), Is.EqualTo(IntPtr.Zero));
                }
            }
        }

        [Test]
        public void FreeTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(6))
                {
                    memory.AllocateMemory(6);

                    var end = memory.Current;
                    memory.FreeMemory(1);
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)1));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)end - 1)));

                    memory.FreeMemory(5);
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)6));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)end - 6)));

                    var current0 = memory.Current;
                    Assert.That(() => memory.FreeMemory(1),
                        Throws.Exception.TypeOf(typeof(Exception))
                        .And.Message.EqualTo("Unable to free memory, it is out of available memory")
                        );
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)6));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(current0)));
                }
            }
        }

        [Test]
        public void TryFreeTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(6))
                {
                    memory.AllocateMemory(6);

                    var end = memory.Current;
                    var result0 = memory.TryFreeMemory(1);
                    Assert.That(result0, Is.EqualTo(true));
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)1));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)end - 1)));

                    var result1 = memory.TryFreeMemory(5);
                    Assert.That(result1, Is.EqualTo(true));
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)6));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr((byte*)end - 6)));

                    var current0 = memory.Current;
                    var result2 = memory.TryFreeMemory(5);
                    Assert.That(result2, Is.EqualTo(false));
                    Assert.That(memory.ByteCount, Is.EqualTo((nuint)6));
                    Assert.That(memory.FreeByteCount, Is.EqualTo((nuint)6));
                    Assert.That(new IntPtr(memory.Current), Is.EqualTo(new IntPtr(current0)));
                }
            }
        }
    }
}