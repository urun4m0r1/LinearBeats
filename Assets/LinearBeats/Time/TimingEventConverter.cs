using System;
using JetBrains.Annotations;
using Utils.Extensions;

//TODO: SpeedEvent 처리 (구간강제배속)
//TODO: 백점프 추가

namespace LinearBeats.Time
{
    public sealed partial class PositionConverter
    {
        private static class TimingEventConverterFactory
        {
            [NotNull]
            public static TimingEventConverter Create(TimingEventType type, [NotNull] TimingEventPosition[] pos)
            {
                return type switch
                {
                    TimingEventType.Jump => new JumpConverter(pos),
                    TimingEventType.Stop => new StopConverter(pos),
                    TimingEventType.Rewind => new RewindConverter(pos),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private abstract class TimingEventConverter
        {
            protected readonly TimingEventPosition[] TimingEvents;

            protected TimingEventConverter([NotNull] TimingEventPosition[] timingEvents) =>
                TimingEvents = timingEvents;

            public abstract void ApplyDistance(ref Position point, Position origin);
        }

        private sealed class JumpConverter : TimingEventConverter
        {
            public JumpConverter([NotNull] TimingEventPosition[] timingEvents) : base(timingEvents)
            {
            }

            public override void ApplyDistance(ref Position point, Position origin)
            {
                foreach (var v in TimingEvents)
                {
                    if (origin.IsBetweenIE(v.Start, v.End)) point += v.Duration;
                    if (origin >= v.End) point += v.Duration;
                }
            }
        }


        private sealed class RewindConverter : TimingEventConverter
        {
            public RewindConverter([NotNull] TimingEventPosition[] timingEvents) : base(timingEvents)
            {
            }

            public override void ApplyDistance(ref Position point, Position origin)
            {
                foreach (var v in TimingEvents)
                {
                    if (origin.IsBetweenIE(v.Start, v.End)) point -= (origin - v.Start) * 2;
                    if (origin >= v.End) point -= v.Duration * 2;
                }
            }
        }


        private sealed class StopConverter : TimingEventConverter
        {
            public StopConverter([NotNull] TimingEventPosition[] timingEvents) : base(timingEvents)
            {
            }

            public override void ApplyDistance(ref Position point, Position origin)
            {
                foreach (var v in TimingEvents)
                {
                    if (origin.IsBetweenIE(v.Start, v.End)) point -= origin - v.Start;
                    if (origin >= v.End) point -= v.Duration;
                }
            }
        }
    }
}
