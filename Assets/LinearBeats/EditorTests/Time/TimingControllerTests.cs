using System;
using JetBrains.Annotations;
using LinearBeats.Audio;
using LinearBeats.Time;
using NUnit.Framework;
using static LinearBeats.EditorTests.FloatTests;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class TimingControllerTests
    {
        private sealed class MockAudioClip : IAudioClip
        {
            private readonly float _x = RandomFloat;
            public Sample Current => _x + 0.5f; // [0 ~ 1.0]
            public Sample Length => 1f;
        }

        private static void Iterate([NotNull] Action<MockAudioClip, TimingController> action)
        {
            for (var i = 0; i < Iteration; ++i)
            {
                var audio = new MockAudioClip();
                action(audio, new TimingController(audio, FixedTimeTests.Factory));
            }
        }

        [Test]
        public void Should_Get_CurrentTime()
        {
            Iterate((a, c) =>
            {
                var currentTime = FixedTimeTests.Factory.Create(a.Current);
                Assert.IsTrue(currentTime == c.CurrentTime);
            });
        }

        [Test]
        public void Should_Get_CurrentProgress()
        {
            Iterate((a, c) =>
            {
                var progress = a.Current / a.Length;
                Assert.AreEqual(progress, c.CurrentProgress, Delta);
            });
        }

        [Test]
        public void Should_Apply_Offset()
        {
            Iterate((a, _) =>
            {
                var offset = FixedTimeTests.Factory.Create(new Second(1f));
                var c = new TimingController(a, FixedTimeTests.Factory, offset);
                var currentTime = FixedTimeTests.Factory.Create(a.Current);
                Assert.IsTrue(currentTime + offset == c.CurrentTime);
            });
        }
    }
}
