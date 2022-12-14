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

        [Test]
        public void GetPropertyPtrTest()
        {
            unsafe
            {
                using var wrap2 = new Struct.HelpStructWrapper();
                var int64Ptr = HelpStructHelper.GetInt64Ptr(wrap2.Ptr);
                Assert.That(new IntPtr(int64Ptr), Is.EqualTo(new IntPtr((byte*)wrap2.Ptr + 0)));

                var int32Ptr = HelpStructHelper.GetInt32Ptr(wrap2.Ptr);
                Assert.That(new IntPtr(int32Ptr), Is.EqualTo(new IntPtr((byte*)wrap2.Ptr + 8)));

                var helpClassPtr = HelpStructHelper.GetHelpClassPtr(wrap2.Ptr);
                Assert.That(new IntPtr(helpClassPtr), Is.EqualTo(new IntPtr((byte*)wrap2.Ptr + 12)));

                var helpClass2Ptr = HelpStructHelper.GetHelpClass2Ptr(wrap2.Ptr);
                Assert.That(new IntPtr(helpClass2Ptr), Is.EqualTo(new IntPtr((byte*)wrap2.Ptr + 12 + HelpClassHelper.SizeOf)));
            }
        }

        [Test]
        public void GetPropertyValueTest()
        {
            unsafe
            {
                var wrap2 = new Struct.HelpStructWrapper();
                var helpStructRef = new HelpStruct()
                {
                    Int32 = 45,
                    Int64 = 4564564564,
                    HelpClass = new HelpClass()
                    {
                        Int32 = 12,
                        Int64 = 321123,
                        HelpStruct2 = new HelpStruct2(321, 98746512),
                        HelpClass2 = new HelpClass()
                    }
                };
                wrap2.Fill(in helpStructRef);
                wrap2.HelpClass2 = new IntPtr(4823);

                var int64Value = HelpStructHelper.GetInt64Value(wrap2.Ptr);
                Assert.That(int64Value, Is.EqualTo(4564564564));

                var int32Value = HelpStructHelper.GetInt32Value(wrap2.Ptr);
                Assert.That(int32Value, Is.EqualTo(45));

                var helpClassValue = HelpStructHelper.GetHelpClassValue(wrap2.Ptr);
                var helpClassValue2 = HelpStructHelper.GetHelpClassValue(wrap2.Ptr);
                Assert.That(!Is.ReferenceEquals(helpClassValue, helpClassValue2));
                Assert.That(helpClassValue.Int32, Is.EqualTo(12));
                Assert.That(helpClassValue.Int64, Is.EqualTo(321123));
                Assert.That(helpClassValue.HelpStruct2, Is.EqualTo(new HelpStruct2(321, 98746512)));
                Assert.That(helpClassValue.HelpClass2, Is.EqualTo(null));

                Assert.That(helpClassValue.Int32, Is.EqualTo(helpClassValue2.Int32));
                Assert.That(helpClassValue.Int64, Is.EqualTo(helpClassValue2.Int64));
                Assert.That(helpClassValue.HelpStruct2, Is.EqualTo(helpClassValue.HelpStruct2));
                Assert.That(helpClassValue.HelpClass2, Is.EqualTo(helpClassValue.HelpClass2));

                var helpClass2Value = HelpStructHelper.GetHelpClass2Value(wrap2.Ptr);
                Assert.That(helpClass2Value, Is.EqualTo(new IntPtr(4823)));

                wrap2.Dispose();
            }
        }

        [Test]
        public void GetOutPropertyValueTest()
        {
            unsafe
            {
                var wrap2 = new Struct.HelpStructWrapper();
                var helpStructRef = new HelpStruct()
                {
                    Int32 = 45,
                    Int64 = 4564564564,
                    HelpClass = new HelpClass()
                    {
                        Int32 = 12,
                        Int64 = 321123,
                        HelpStruct2 = new HelpStruct2(321, 98746512),
                        HelpClass2 = new HelpClass()
                    }
                };
                wrap2.Fill(in helpStructRef);
                wrap2.HelpClass2 = new IntPtr(4823);

                HelpStructHelper.GetOutInt64Value(wrap2.Ptr, out var int64Value);
                Assert.That(int64Value, Is.EqualTo(4564564564));

                HelpStructHelper.GetOutInt32Value(wrap2.Ptr, out var int32Value);
                Assert.That(int32Value, Is.EqualTo(45));

                HelpStructHelper.GetOutHelpClass2Value(wrap2.Ptr, out var helpClass2Value);
                Assert.That(helpClass2Value, Is.EqualTo(new IntPtr(4823)));

                wrap2.Dispose();
            }
        }

        [Test]
        public void GetRefPropertyValueTest()
        {
            unsafe
            {
                var wrap2 = new Struct.HelpStructWrapper();
                var helpStructRef = new HelpStruct()
                {
                    Int32 = 45,
                    Int64 = 4564564564,
                    HelpClass = new HelpClass()
                    {
                        Int32 = 12,
                        Int64 = 321123,
                        HelpStruct2 = new HelpStruct2(321, 98746512),
                        HelpClass2 = new HelpClass()
                    }
                };
                wrap2.Fill(in helpStructRef);
                wrap2.HelpClass2 = new IntPtr(4823);

                long int64Value = 0;
                HelpStructHelper.GetRefInt64Value(wrap2.Ptr, ref int64Value);
                Assert.That(int64Value, Is.EqualTo(4564564564));

                int int32Value = 0;
                HelpStructHelper.GetRefInt32Value(wrap2.Ptr, ref int32Value);
                Assert.That(int32Value, Is.EqualTo(45));

                IntPtr helpClass2Value = IntPtr.Zero;
                HelpStructHelper.GetRefHelpClass2Value(wrap2.Ptr, ref helpClass2Value);
                Assert.That(helpClass2Value, Is.EqualTo(new IntPtr(4823)));

                wrap2.Dispose();
            }
        }

        [Test]
        public void IsNullableTest()
        {
            Assert.That(HelpStruct2Helper.IsNullable(), Is.EqualTo(false));
            Assert.That(HelpStructHelper.IsNullable(), Is.EqualTo(false));
            Assert.That(HelpClassHelper.IsNullable(), Is.EqualTo(true));
        }
    }
}