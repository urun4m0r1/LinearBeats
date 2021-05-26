using System;
using JetBrains.Annotations;
using LinearBeats.Audio;
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;
using static LinearBeats.EditorTests.FloatTests;
using static LinearBeats.EditorTests.Time.FixedTimeTests;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class TimingControllerTests
    {
        private sealed class MockAudioClip : IAudioClip
        {
            private readonly int _x = RandomInt;
            private readonly float _y = RandomFloat;
            public Sample Current => _x + 5_000; // [0 ~ 10_000]
            public Second Length => 10_000f / TimingConverterTests.SamplesPerSecond;
            public Second Offset => _y;
            public bool IsPlaying { get; }
        }

        private static void Iterate([NotNull] Action<MockAudioClip, AudioTimingInfo> action)
        {
            for (var i = 0; i < Iteration; ++i)
            {
                var audio = new MockAudioClip();
                action(audio, new AudioTimingInfo(audio, Factory));
            }
        }

        [Test]
        public void Should_Get_CurrentTime()
        {
            Iterate((a, c) =>
            {
                var currentTime = Factory.Create(a.Current);
                Assert.AreEqual(currentTime + a.Offset, c.Now.Second);
            });
        }

        [Test]
        public void Should_Get_CurrentProgress()
        {
            Iterate((a, c) =>
            {
                var progress = Factory.Create(a.Current) / a.Length;
                Assert.AreEqual(progress, c.Progress, Delta);
                Assert.GreaterOrEqual(c.Progress, 0f);
                Assert.LessOrEqual(c.Progress, 1f);
            });
        }
    }
}
