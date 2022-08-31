using NUnit.Framework;
using System;

namespace Tests
{
    [TestFixture]
    public class HelperFixture
    {
        [Test]
        public void SizeTest()
        {
            unsafe
            {
                var sizeIntPtr = (nuint)sizeof(IntPtr);

                var helpStruct2Size = (nuint)12;
                Assert.That(HelpStruct2Helper.SizeOf, Is.EqualTo(helpStruct2Size));

                var helpClassSize = helpStruct2Size + sizeIntPtr + 13;
                Assert.That(HelpClassHelper.SizeOf, Is.EqualTo(helpClassSize));

                var helpStructSize = 12 + helpClassSize + sizeIntPtr;
                Assert.That(HelpStructHelper.SizeOf, Is.EqualTo(helpStructSize));
            }
        }

        [Test]
        public void OffsetTest()
        {
            unsafe
            {
                var sizeIntPtr = (nuint)sizeof(IntPtr);
                var int32Offset = 9 + sizeIntPtr;
                Assert.That(HelpClassHelper.Int32Offset, Is.EqualTo(int32Offset));
                var helpStruct2Offset = int32Offset + 4;
                Assert.That(HelpClassHelper.HelpStruct2Offset, Is.EqualTo(helpStruct2Offset));

                var helpClass2Offset = 12 + HelpClassHelper.SizeOf;
                Assert.That(HelpStructHelper.HelpClass2Offset, Is.EqualTo(helpClass2Offset));
            }
        }

        [Test]
        public void SetFromRootValueTest()
        {
            unsafe
            {
                using var wrap = new Struct.HelpStructWrapper();
                var helpStruct = new HelpStruct();
                helpStruct.Int32 = 16;
                HelpStructHelper.SetInt32Value(wrap.Ptr, in helpStruct);
                Assert.That(wrap.Int32, Is.EqualTo(16));

                helpStruct.HelpClass = new HelpClass();
                helpStruct.HelpClass.HelpStruct2 = new HelpStruct2(44, 325);
                helpStruct.HelpClass.HelpClass2 = new HelpClass();
                HelpStructHelper.SetHelpClassValue(wrap.Ptr, in helpStruct);

                var helpclassW = wrap.HelpClass;
                Assert.That(helpclassW.HelpStruct2, Is.EqualTo(helpStruct.HelpClass.HelpStruct2));
                Assert.That(helpclassW.HelpClass2, Is.EqualTo(null));
            }
        }

        [Test]
        public void SetFromValueTest()
        {
            unsafe
            {
                using var wrap = new Struct.HelpStructWrapper();
                var helpStruct = new HelpStruct();
                helpStruct.Int32 = 16;
                HelpStructHelper.SetInt32Value(wrap.Ptr, 23);
                Assert.That(wrap.Int32, Is.EqualTo(23));

                var helpClass = new HelpClass();
                helpClass.Int32 = 78;
                helpClass.Int64 = 101;
                helpClass.HelpStruct2 = new HelpStruct2(44, 325);
                helpClass.HelpClass2 = new HelpClass();
                HelpStructHelper.SetHelpClassValue(wrap.Ptr, in helpClass);
                Assert.That(wrap.HelpClass.HelpStruct2, Is.EqualTo(helpClass.HelpStruct2));
                Assert.That(wrap.HelpClass.HelpClass2, Is.EqualTo(null));
                Assert.That(wrap.HelpClass.Int32, Is.EqualTo(78));
                Assert.That(wrap.HelpClass.Int64, Is.EqualTo(101));
            }
        }

        [Test]
        public void CopyTest()
        {
            unsafe
            {
                using var wrap = new Struct.HelpStructWrapper();
                var wrap2 = new Struct.HelpStructWrapper();
                wrap2.Int32 = 66;
                wrap2.HelpClass = new HelpClass() 
                { 
                    Int32 = 12,
                    HelpStruct2 = new HelpStruct2(321, 98746512),
                    HelpClass2 = new HelpClass()
                };
                HelpStructHelper.Copy(wrap2.Ptr, wrap.Ptr);
                Assert.That(wrap.Int32, Is.EqualTo(66));
                Assert.That(wrap.HelpClass.HelpStruct2, Is.EqualTo(new HelpStruct2(321, 98746512)));
                Assert.That(wrap.HelpClass.HelpClass2, Is.EqualTo(null));
                Assert.That(wrap.HelpClass.Int32, Is.EqualTo(12));
                wrap2.Dispose();
            }
        }

        [Test]
        public void CopyToValueOutTest()
        {
            unsafe
            {
                var wrap2 = new Struct.HelpStructWrapper();
                wrap2.Int32 = 66;
                wrap2.HelpClass = new HelpClass()
                {
                    Int32 = 12,
                    HelpStruct2 = new HelpStruct2(321, 98746512),
                    HelpClass2 = new HelpClass()
                };
                HelpStructHelper.CopyToValueOut(wrap2.Ptr, out var helpStructOut);
                Assert.That(helpStructOut.Int32, Is.EqualTo(66));
                Assert.That(helpStructOut.HelpClass.HelpStruct2, Is.EqualTo(new HelpStruct2(321, 98746512)));
                Assert.That(helpStructOut.HelpClass.HelpClass2, Is.EqualTo(null));
                Assert.That(helpStructOut.HelpClass.Int32, Is.EqualTo(12));
                wrap2.Dispose();
            }
        }

        [Test]
        public void CopyToValueRefTest()
        {
            unsafe
            {
                var wrap2 = new Struct.HelpStructWrapper();
                wrap2.Int32 = 66;
                wrap2.HelpClass = new HelpClass()
                {
                    Int32 = 12,
                    HelpStruct2 = new HelpStruct2(321, 98746512),
                    HelpClass2 = new HelpClass()
                };

                var helpStructRef = new HelpStruct();
                HelpStructHelper.CopyToValue(wrap2.Ptr, ref helpStructRef);
                Assert.That(helpStructRef.Int32, Is.EqualTo(66));
                Assert.That(helpStructRef.HelpClass.HelpStruct2, Is.EqualTo(new HelpStruct2(321, 98746512)));
                Assert.That(helpStructRef.HelpClass.HelpClass2, Is.EqualTo(null));
                Assert.That(helpStructRef.HelpClass.Int32, Is.EqualTo(12));
                wrap2.Dispose();
            }
        }

        [Test]
        public void CopyToPtrTest()
        {
            unsafe
            {
                var wrap2 = new Struct.HelpStructWrapper();
                var helpStructRef = new HelpStruct() 
                {
                    Int32 = 45,
                    HelpClass = new HelpClass() 
                    {
                        Int32 = 12,
                        HelpStruct2 = new HelpStruct2(321, 98746512),
                        HelpClass2 = new HelpClass()
                    }
                };
                HelpStructHelper.CopyToPtr(in helpStructRef, wrap2.Ptr);
                Assert.That(wrap2.Int32, Is.EqualTo(45));
                Assert.That(wrap2.HelpClass.HelpStruct2, Is.EqualTo(new HelpStruct2(321, 98746512)));
                Assert.That(wrap2.HelpClass.HelpClass2, Is.EqualTo(null));
                Assert.That(wrap2.HelpClass.Int32, Is.EqualTo(12));
                wrap2.Dispose();
            }
        }
    }
}