using LinearBeats.Time;
using NUnit.Framework;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class FixedTImeTests : TimingConvertingTests
    {
        [Test]
        public void Should_Get_Bpm_With_Pulse()
        {
            TestUtils.AreEqual(FirstBpm, fixedTimeFactory.Create(pulseA).Bpm);
            TestUtils.AreEqual(SecondBpm, fixedTimeFactory.Create(pulseB).Bpm);
            TestUtils.AreEqual(ThirdBpm, fixedTimeFactory.Create(pulseC).Bpm);
        }

        [Test]
        public void Should_Compare_Between_FixedTime()
        {
            Assert.IsTrue(fixedTimeFactory.Create(pulseA) < fixedTimeFactory.Create(pulseB));
            Assert.IsTrue(fixedTimeFactory.Create(pulseB) < fixedTimeFactory.Create(pulseC));
            Assert.IsTrue(fixedTimeFactory.Create(pulseA) < fixedTimeFactory.Create(pulseC));

            Assert.IsFalse(fixedTimeFactory.Create(pulseA) > fixedTimeFactory.Create(pulseB));
            Assert.IsFalse(fixedTimeFactory.Create(pulseB) > fixedTimeFactory.Create(pulseC));
            Assert.IsFalse(fixedTimeFactory.Create(pulseA) > fixedTimeFactory.Create(pulseC));
        }

        [Test]
        public void Should_Compare_Equatable_Between_FixedTime()
        {
            Assert.IsTrue(fixedTimeFactory.Create(pulseA) == fixedTimeFactory.Create(new Pulse(0f)));
            Assert.IsTrue(fixedTimeFactory.Create(pulseA) != fixedTimeFactory.Create(pulseB));
        }
    }
}
