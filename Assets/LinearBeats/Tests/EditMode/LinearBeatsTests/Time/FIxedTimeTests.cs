#pragma warning disable IDE0090

using LinearBeats.Time;
using NUnit.Framework;

namespace LinearBeatsTests.Time
{
    [TestFixture]
    public class FixedTImeTests : TimingConvertingTests
    {
        [Test]
        public void Should_Store_Bpm_With_Pulse()
        {
            TestUtils.AreEqual(FirstBpm, new FixedTime(pulseA).Bpm);
            TestUtils.AreEqual(SecondBpm, new FixedTime(pulseB).Bpm);
            TestUtils.AreEqual(ThirdBpm, new FixedTime(pulseC).Bpm);
        }

        [Test]
        public void Should_Compare_Between_FixedTime()
        {
            Assert.IsTrue(pulseB > new FixedTime(pulseA));
        }
    }
}