using System;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Extensions;

namespace LinearBeats.Time
{
    public enum ScrollEvent : byte
    {
        Stop,
        Jump,
        BackJump,
        Rewind,
        Speed,
        SpeedBounce,
    }

    public sealed partial class PositionConverter
    {
        private static class ScrollEventConverterFactory
        {
            [NotNull]
            public static ScrollEventConverter Create(ScrollEvent type, [NotNull] ScrollEventPosition[] eventPositions)
            {
                return type switch
                {
                    ScrollEvent.Stop => new StopEventConverter(eventPositions),
                    ScrollEvent.Jump => new JumpEventConverter(eventPositions),
                    ScrollEvent.BackJump => new BackJumpEventConverter(eventPositions),
                    ScrollEvent.Rewind => new RewindEventConverter(eventPositions),
                    ScrollEvent.Speed => new SpeedEventConverter(eventPositions),
                    ScrollEvent.SpeedBounce => new SpeedBounceEventConverter(eventPositions),
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };
            }
        }

        private abstract class ScrollEventConverter
        {
            [NotNull] protected readonly ScrollEventPosition[] TimingEvents;

            protected ScrollEventConverter([NotNull] ScrollEventPosition[] timingEvents) =>
                TimingEvents = timingEvents;

            public abstract void ApplyDistance(ref Position point, Position origin);
        }

        private sealed class StopEventConverter : ScrollEventConverter
        {
            public StopEventConverter([NotNull] ScrollEventPosition[] timingEvents) : base(timingEvents)
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

        private sealed class JumpEventConverter : ScrollEventConverter
        {
            public JumpEventConverter([NotNull] ScrollEventPosition[] timingEvents) : base(timingEvents)
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

        private sealed class BackJumpEventConverter : ScrollEventConverter
        {
            public BackJumpEventConverter([NotNull] ScrollEventPosition[] timingEvents) : base(timingEvents)
            {
            }

            public override void ApplyDistance(ref Position point, Position origin)
            {
                foreach (var v in TimingEvents)
                {
                    if (origin.IsBetweenIE(v.Start, v.End)) point -= v.Duration;
                    if (origin >= v.End) point -= v.Duration;
                }
            }
        }


        private sealed class RewindEventConverter : ScrollEventConverter
        {
            public RewindEventConverter([NotNull] ScrollEventPosition[] timingEvents) : base(timingEvents)
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

        private sealed class SpeedEventConverter : ScrollEventConverter
        {
            public SpeedEventConverter([NotNull] ScrollEventPosition[] timingEvents) : base(timingEvents)
            {
            }

            //FIXME: SpeedEventConverter
            public override void ApplyDistance(ref Position point, Position origin)
            {
                foreach (var v in TimingEvents)
                {
                    if (origin.IsBetweenIE(v.Start, v.End)) point += v.Amount * v.Duration;
                    if (origin >= v.End) point += v.Amount * v.Duration;
                }
            }
        }

        private sealed class SpeedBounceEventConverter : ScrollEventConverter
        {
            public SpeedBounceEventConverter([NotNull] ScrollEventPosition[] timingEvents) : base(timingEvents)
            {
            }

            //FIXME: SpeedBounceEventConverter
            public override void ApplyDistance(ref Position point, Position origin)
            {
                foreach (var v in TimingEvents)
                {
                    if (origin.IsBetweenIE(v.Start, v.End))
                    {
                        var elapsed = origin - v.Start;
                        var remain = v.End - origin;
                        point += v.Amount * Mathf.Min(elapsed, remain);
                    }

                    if (origin >= v.End) point += v.Amount * (v.Duration * v.Duration * .25f);
                }
            }
        }
    }
}
