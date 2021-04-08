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
            TestUtils.AreEqual(FirstBpm, FixedTimeFactory.Create(PulseA).Bpm);
            TestUtils.AreEqual(SecondBpm, FixedTimeFactory.Create(PulseB).Bpm);
            TestUtils.AreEqual(ThirdBpm, FixedTimeFactory.Create(PulseC).Bpm);
        }

        [Test]
        public void Should_Compare_Between_FixedTime()
        {
            Assert.IsTrue(FixedTimeFactory.Create(PulseA) < FixedTimeFactory.Create(PulseB));
            Assert.IsTrue(FixedTimeFactory.Create(PulseB) < FixedTimeFactory.Create(PulseC));
            Assert.IsTrue(FixedTimeFactory.Create(PulseA) < FixedTimeFactory.Create(PulseC));

            Assert.IsFalse(FixedTimeFactory.Create(PulseA) > FixedTimeFactory.Create(PulseB));
            Assert.IsFalse(FixedTimeFactory.Create(PulseB) > FixedTimeFactory.Create(PulseC));
            Assert.IsFalse(FixedTimeFactory.Create(PulseA) > FixedTimeFactory.Create(PulseC));
        }

        [Test]
        public void Should_Compare_Equatable_Between_FixedTime()
        {
            Assert.IsTrue(FixedTimeFactory.Create(PulseA) == FixedTimeFactory.Create(new Pulse(0f)));
            Assert.IsTrue(FixedTimeFactory.Create(PulseA) != FixedTimeFactory.Create(PulseB));
        }
    }
}
