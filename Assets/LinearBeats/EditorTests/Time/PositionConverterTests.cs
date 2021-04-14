using System;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;
using static LinearBeats.EditorTests.FloatTests;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class PositionConverterTests
    {
        //TODO: normalize 분기 검사
        [NotNull] private static readonly BpmEvent[] BpmEvents =
        {
            new BpmEvent {Ppqn = 100, Pulse = 0, Bpm = 100},
            new BpmEvent {Ppqn = 100, Pulse = 500, Bpm = 200},
            new BpmEvent {Ppqn = 100, Pulse = 1000, Bpm = 300},
        };

        [NotNull] private static readonly ITimingConverter Converter = new TimingConverter(BpmEvents, 120, 1000);
        [NotNull] private static readonly FixedTime.Factory Factory = new FixedTime.Factory(Converter);
        [NotNull] private static PositionConverter.Builder Builder => new PositionConverter.Builder(Converter);

        [NotNull] private static readonly TimingEvent[] JumpEvent =
        {
            new TimingEvent {Pulse = 200, Duration = 100},
            new TimingEvent {Pulse = 400, Duration = 100},
        };

        [NotNull] private static readonly TimingEvent[] StopEvent =
        {
            new TimingEvent {Pulse = 600, Duration = 100},
            new TimingEvent {Pulse = 800, Duration = 100},
        };

        [NotNull] private static readonly TimingEvent[] RewindEvent =
        {
            new TimingEvent {Pulse = 1000, Duration = 100},
            new TimingEvent {Pulse = 1500, Duration = 100},
        };

        private static readonly FixedTime V0 = Factory.Create(new Pulse(default));

        private static void Iterate([NotNull] IPositionConverter converter, [NotNull] Action<FixedTime, float> action)
        {
            for (var i = 0; i < Iteration; ++i)
            {
                var randomPulse = new Pulse(RandomFloat);
                var fixedTime = Factory.Create(randomPulse);
                action(fixedTime, converter.ToPosition(fixedTime));
            }
        }

        private static Pulse GetJumpDistance(Pulse f)
        {
            var pos = JumpEvent.Where(v => f >= v.Pulse)
                .Aggregate(0f, (current, v) => current + v.Duration);

            return pos;
        }


        private static Pulse GetStopDistance(Pulse f)
        {
            var pos = 0f;
            foreach (var v in StopEvent)
            {
                if (f >= v.Pulse && f < v.Pulse + v.Duration) pos -= f - v.Pulse;
                if (f >= v.Pulse + v.Duration) pos -= v.Duration;
            }

            return pos;
        }

        private static Pulse GetRewindDistance(Pulse f)
        {
            var pos = 0f;
            foreach (var v in RewindEvent)
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
            var converter = Builder.JumpEvent(JumpEvent).Build();

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
            var converter = Builder.StopEvent(StopEvent).Build();

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
            var converter = Builder.RewindEvent(RewindEvent).Build();

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
            var converter = Builder.JumpEvent(JumpEvent).StopEvent(StopEvent).RewindEvent(RewindEvent).Build();

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
