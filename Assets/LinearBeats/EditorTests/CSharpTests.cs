using NUnit.Framework;

namespace LinearBeats.EditorTests
{
    [TestFixture]
    public class CSharpTests
    {
        [Test]
        public void Test()
        {
            string a = null;

            Assert.IsFalse(a.Contains("aa"));
        }
    }
}
