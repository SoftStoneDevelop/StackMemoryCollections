using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class CommonHelperFixture
    {
        [Test]
        public void CopyTest()
        {
            unsafe
            {
                byte byteVal = 0;
                byte* bytePtr = &byteVal;
                Assert.That(StackMemoryCollections.CommonHelper.IsNull(bytePtr), Is.EqualTo(true));
                byteVal = 1;
                Assert.That(StackMemoryCollections.CommonHelper.IsNull(bytePtr), Is.EqualTo(false));
            }
        }
    }
}