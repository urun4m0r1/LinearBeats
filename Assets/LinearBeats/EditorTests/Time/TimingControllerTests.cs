using System;
using JetBrains.Annotations;
using LinearBeats.Audio;
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;
using static LinearBeats.EditorTests.FloatTests;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class TimingControllerTests
    {
        [NotNull] private static readonly BpmEvent[] BpmEvents = {new BpmEvent {Ppqn = 100, Pulse = 0, Bpm = 120}};
        [NotNull] private static readonly ITimingConverter Converter = new TimingConverter(BpmEvents, 120, 1000);
        [NotNull] private static readonly FixedTime.Factory Factory = new FixedTime.Factory(Converter);

        private sealed class MockAudioClip : IAudioClip
        {
            private readonly float _x = RandomFloat;
            public Sample Current => _x + 5_000f; // [0 ~ 10_000]
            public Sample Length => 10_000f;
        }

        private static void Iterate([NotNull] Action<MockAudioClip, TimingController> action)
        {
            for (var i = 0; i < Iteration; ++i)
            {
                var audio = new MockAudioClip();
                action(audio, new TimingController(audio, Factory));
            }
        }

        [Test]
        public void Should_Get_CurrentTime()
        {
            Iterate((a, c) =>
            {
                var currentTime = Factory.Create(a.Current);
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
                Assert.GreaterOrEqual(c.CurrentProgress, 0f);
                Assert.LessOrEqual(c.CurrentProgress, 1f);
            });
        }

        [Test]
        public void Should_Apply_Offset()
        {
            Iterate((a, _) =>
            {
                var offset = Factory.Create(new Second(1f));
                var c = new TimingController(a, Factory, offset);
                var currentTime = Factory.Create(a.Current);
                Assert.IsTrue(currentTime + offset == c.CurrentTime);
            });
        }
    }
}
