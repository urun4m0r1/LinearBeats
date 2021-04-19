using System;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;
using static LinearBeats.EditorTests.FloatTests;
using static LinearBeats.EditorTests.Time.FixedTimeTests;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class PositionConverterTests
    {
        //Normalize 전처리, 후처리 채보 스크롤 비교
        [NotNull]
        private static PositionConverter.Builder Builder =>
            new PositionConverter.Builder(TimingConverterTests.Converter);

        [NotNull] private readonly TimingEvent[] _jumpEvent =
        {
            new TimingEvent {Pulse = 200, Duration = 100},
            new TimingEvent {Pulse = 400, Duration = 100},
        };

        [NotNull] private readonly TimingEvent[] _stopEvent =
        {
            new TimingEvent {Pulse = 600, Duration = 100},
            new TimingEvent {Pulse = 800, Duration = 100},
        };

        [NotNull] private readonly TimingEvent[] _rewindEvent =
        {
            new TimingEvent {Pulse = 1000, Duration = 100},
            new TimingEvent {Pulse = 1500, Duration = 100},
        };

        private static void Iterate([NotNull] IPositionConverter converter, [NotNull] Action<FixedTime, float> action)
        {
            for (var i = 0; i < Iteration; ++i)
            {
                var randomPulse = new Pulse(RandomFloat);
                var fixedTime = Factory.Create(randomPulse);
                action(fixedTime, converter.ToPosition(fixedTime));
            }
        }

        private Pulse GetJumpDistance(Pulse f)
        {
            var pos = _jumpEvent.Where(v => f >= v.Pulse)
                .Aggregate(0f, (current, v) => current + v.Duration);

            return pos;
        }


        private Pulse GetStopDistance(Pulse f)
        {
            var pos = 0f;
            foreach (var v in _stopEvent)
            {
                if (f >= v.Pulse && f < v.Pulse + v.Duration) pos -= f - v.Pulse;
                if (f >= v.Pulse + v.Duration) pos -= v.Duration;
            }

            return pos;
        }

        private Pulse GetRewindDistance(Pulse f)
        {
            var pos = 0f;
            foreach (var v in _rewindEvent)
            {
                if (f >= v.Pulse && f < v.Pulse + v.Duration) pos -= (f - v.Pulse) * 2;
                if (f >= v.Pulse + v.Duration) pos -= v.Duration * 2;
            }

            return pos;
        }

        [Test]
        public void Should_Return_Same_Position_With_Default_Options()
        {
            var converter = Builder.Build();

            var p0 = converter.ToPosition(V0);
            Assert.AreEqual(V0.Pulse, p0, Delta);

            Iterate(converter, (f, p) => Assert.AreEqual(f.Pulse, p, Delta));
        }

        [Test]
        public void Should_Calculate_JumpEvents()
        {
            var converter = Builder.JumpEvent(_jumpEvent).Build();

            var p0 = converter.ToPosition(V0);
            Assert.AreEqual(V0.Pulse, p0, Delta);

            Iterate(converter, (f, p) =>
            {
                var distance = f + GetJumpDistance(f);
                Assert.AreEqual(distance, p, Delta);
            });
        }

        [Test]
        public void Should_Calculate_StopEvents()
        {
            var converter = Builder.StopEvent(_stopEvent).Build();

            var p0 = converter.ToPosition(V0);
            Assert.AreEqual(V0.Pulse, p0, Delta);

            Iterate(converter, (f, p) =>
            {
                var distance = f + GetStopDistance(f);
                Assert.AreEqual(distance, p, Delta);
            });
        }

        [Test]
        public void Should_Calculate_RewindEvents()
        {
            var converter = Builder.RewindEvent(_rewindEvent).Build();

            var p0 = converter.ToPosition(V0);
            Assert.AreEqual(V0.Pulse, p0, Delta);

            Iterate(converter, (f, p) =>
            {
                var distance = f + GetRewindDistance(f);
                Assert.AreEqual(distance, p, Delta);
            });
        }

        [Test]
        public void Should_Calculate_CombinedEvents()
        {
            var converter = Builder.JumpEvent(_jumpEvent).StopEvent(_stopEvent).RewindEvent(_rewindEvent).Build();

            var p0 = converter.ToPosition(V0);
            Assert.AreEqual(V0.Pulse, p0, Delta);

            Iterate(converter, (f, p) =>
            {
                var dj = GetJumpDistance(f);
                var ds = GetStopDistance(f);
                var dr = GetRewindDistance(f);
                var distance = f + dj + ds + dr;
                Assert.AreEqual(distance, p, Delta);
            });
        }
    }
}
