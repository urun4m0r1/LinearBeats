using NUnit.Framework;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class FixedTImeTests : TimingConvertingTests
    {
        [Test]
        public void Should_Store_Bpm_With_Pulse()
        {
            TestUtils.AreEqual(FirstBpm, fixedTimeFactory.Create(pulseA).Bpm);
            TestUtils.AreEqual(SecondBpm, fixedTimeFactory.Create(pulseB).Bpm);
            TestUtils.AreEqual(ThirdBpm, fixedTimeFactory.Create(pulseC).Bpm);
        }

        [Test]
        public void Should_Compare_Between_FixedTime()
        {
            Assert.IsTrue(fixedTimeFactory.Create(pulseB) > fixedTimeFactory.Create(pulseA));
        }
    }
}
