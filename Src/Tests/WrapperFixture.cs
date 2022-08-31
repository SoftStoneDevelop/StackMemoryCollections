using NUnit.Framework;
using System;

namespace Tests
{
    [TestFixture]
    public class WrapperFixture
    {
        [Test]
        public void FillAndGetValueTest()
        {
            unsafe
            {
                var wrap2 = new Struct.HelpStructWrapper();
                var helpStructValueIn = new HelpStruct()
                {
                    Int32 = 45,
                    Int64 = 4564564564,
                    HelpClass = new HelpClass()
                    {
                        Int32 = 12,
                        Int64 = 321123,
                        HelpStruct2 = new HelpStruct2(321, 98746512),
                        HelpClass2 = new HelpClass()
                        {
                            Int32 = 238,
                            Int64 = 40,
                            HelpStruct2 = new HelpStruct2(1, 2)
                        }
                    }
                };
                wrap2.Fill(in helpStructValueIn);
                Assert.That(wrap2.HelpClass2, Is.EqualTo(IntPtr.Zero));
                Assert.That(wrap2.Int64, Is.EqualTo(4564564564));
                Assert.That(wrap2.Int32, Is.EqualTo(45));
                var helpClassValue = wrap2.HelpClass;
                var helpClassValue2 = wrap2.HelpClass;
                Assert.That(!Is.ReferenceEquals(helpClassValue, helpClassValue2));
                Assert.That(helpClassValue.Int32, Is.EqualTo(12));
                Assert.That(helpClassValue.Int64, Is.EqualTo(321123));
                Assert.That(helpClassValue.HelpStruct2, Is.EqualTo(new HelpStruct2(321, 98746512)));
                Assert.That(helpClassValue.HelpClass2, Is.EqualTo(null));

                Assert.That(helpClassValue.Int32, Is.EqualTo(helpClassValue2.Int32));
                Assert.That(helpClassValue.Int64, Is.EqualTo(helpClassValue2.Int64));
                Assert.That(helpClassValue.HelpStruct2, Is.EqualTo(helpClassValue.HelpStruct2));
                Assert.That(helpClassValue.HelpClass2, Is.EqualTo(helpClassValue.HelpClass2));

                var wrapClass = new Struct.HelpClassWrapper();
                wrapClass.Fill(new HelpClass(44, 235, new HelpStruct2(140, 78)));
                wrap2.HelpClass2 = new IntPtr(wrapClass.Ptr);
                Assert.That(wrap2.HelpClass2, Is.EqualTo(new IntPtr(wrapClass.Ptr)));
                var helpClass2Value = wrap2.HelpClass2ValueInPtr;
                Assert.That(helpClass2Value, Is.Not.EqualTo(null));
                Assert.That(helpClass2Value.Int32, Is.EqualTo(44));
                Assert.That(helpClass2Value.Int64, Is.EqualTo(235));
                Assert.That(helpClass2Value.HelpStruct2, Is.EqualTo(new HelpStruct2(140, 78)));
                Assert.That(helpClass2Value.HelpClass2, Is.EqualTo(null));

                var helpStructValue = wrap2.GetValue();
                Assert.That(helpStructValue.Int64, Is.EqualTo(4564564564));
                Assert.That(helpStructValue.Int32, Is.EqualTo(45));
                Assert.That(helpStructValue.HelpClass.Int32, Is.EqualTo(12));
                Assert.That(helpStructValue.HelpClass.Int64, Is.EqualTo(321123));
                Assert.That(helpStructValue.HelpClass.HelpStruct2, Is.EqualTo(new HelpStruct2(321, 98746512)));
                
                Assert.That(helpStructValue.HelpClass2, Is.Not.EqualTo(null));
                Assert.That(helpStructValue.HelpClass2.Int32, Is.EqualTo(44));
                Assert.That(helpStructValue.HelpClass2.Int64, Is.EqualTo(235));
                Assert.That(helpStructValue.HelpClass2.HelpStruct2, Is.EqualTo(new HelpStruct2(140, 78)));
                Assert.That(helpStructValue.HelpClass2.HelpClass2, Is.EqualTo(null));

                wrapClass.Dispose();
                wrap2.Dispose();
            }
        }

        [Test]
        public void GetPtrTest()
        {
            unsafe
            {
                var wrap2 = new Struct.HelpStructWrapper();
                var int64Ptr = wrap2.Int64Ptr;
                Assert.That(new IntPtr(int64Ptr), Is.EqualTo(new IntPtr((byte*)wrap2.Ptr + 0)));

                var int32Ptr = wrap2.Int32Ptr;
                Assert.That(new IntPtr(int32Ptr), Is.EqualTo(new IntPtr((byte*)wrap2.Ptr + 8)));

                var helpClassPtr = wrap2.HelpClassPtr;
                Assert.That(new IntPtr(helpClassPtr), Is.EqualTo(new IntPtr((byte*)wrap2.Ptr + 12)));

                var helpClass2Ptr = wrap2.HelpClass2Ptr;
                Assert.That(new IntPtr(helpClass2Ptr), Is.EqualTo(new IntPtr((byte*)wrap2.Ptr + 12 + HelpClassHelper.SizeOf)));

                wrap2.Dispose();
            }
        }

        [Test]
        public void NullableTest()
        {
            unsafe
            {
                using (var memory = new StackMemoryCollections.Struct.StackMemory(HelpClassHelper.SizeOf))
                {
                    { 
                        var wrap2 = new Struct.HelpClassWrapper(memory.Current, true);
                        Assert.That(wrap2.HelpClass2, Is.EqualTo(IntPtr.Zero));
                        Assert.That(wrap2.IsNull, Is.EqualTo(false));
                        wrap2.HelpClass2 = new IntPtr(456421332);
                        Assert.That(wrap2.HelpClass2, Is.Not.EqualTo(IntPtr.Zero));
                        wrap2.Fill(null);
                        Assert.That(wrap2.IsNull, Is.EqualTo(true));
                        wrap2.CreateInstance();
                        Assert.That(wrap2.IsNull, Is.EqualTo(false));
                        Assert.That(wrap2.HelpClass2, Is.EqualTo(IntPtr.Zero));
                    }
                }
            }
        }

        [Test]
        public void GetOutTest()
        {
            unsafe
            {
                var wrap2 = new Struct.HelpStructWrapper();
                var helpStructValueIn = new HelpStruct()
                {
                    Int32 = 45,
                    Int64 = 4564564564,
                    HelpClass = new HelpClass()
                    {
                        Int32 = 12,
                        Int64 = 321123,
                        HelpStruct2 = new HelpStruct2(321, 98746512),
                        HelpClass2 = new HelpClass()
                        {
                            Int32 = 238,
                            Int64 = 40,
                            HelpStruct2 = new HelpStruct2(1, 2)
                        }
                    }
                };
                wrap2.Fill(in helpStructValueIn);

                IntPtr helpClass2Out;
                wrap2.GetOutHelpClass2(out helpClass2Out);
                Assert.That(helpClass2Out, Is.EqualTo(IntPtr.Zero));
                long int64Out;
                wrap2.GetOutInt64(out int64Out);
                Assert.That(int64Out, Is.EqualTo(4564564564));
                int int32Out;
                wrap2.GetOutInt32(out int32Out);
                Assert.That(int32Out, Is.EqualTo(45));
                HelpClass helpClassValue;
                wrap2.GetOutHelpClass(out helpClassValue);
                Assert.That(helpClassValue.Int32, Is.EqualTo(12));
                Assert.That(helpClassValue.Int64, Is.EqualTo(321123));
                Assert.That(helpClassValue.HelpStruct2, Is.EqualTo(new HelpStruct2(321, 98746512)));
                Assert.That(helpClassValue.HelpClass2, Is.EqualTo(null));

                var wrapClass = new Struct.HelpClassWrapper();
                wrapClass.Fill(new HelpClass(44, 235, new HelpStruct2(140, 78)));
                var wrapClass2 = new Struct.HelpClassWrapper(wrap2.HelpClassPtr, false);
                wrapClass2.HelpClass2 = new IntPtr(wrapClass.Ptr);

                HelpClass helpClassValue2;
                wrap2.GetOutHelpClass(out helpClassValue2);
                Assert.That(helpClassValue2.Int32, Is.EqualTo(12));
                Assert.That(helpClassValue2.Int64, Is.EqualTo(321123));
                Assert.That(helpClassValue2.HelpStruct2, Is.EqualTo(new HelpStruct2(321, 98746512)));
                Assert.That(helpClassValue2.HelpClass2, Is.Not.EqualTo(null));

                Assert.That(helpClassValue2.HelpClass2.Int32, Is.EqualTo(44));
                Assert.That(helpClassValue2.HelpClass2.Int64, Is.EqualTo(235));
                Assert.That(helpClassValue2.HelpClass2.HelpStruct2, Is.EqualTo(new HelpStruct2(140, 78)));
                Assert.That(helpClassValue2.HelpClass2.HelpClass2, Is.EqualTo(null));

                wrapClass.Dispose();
                wrap2.Dispose();
            }
        }
    }
}