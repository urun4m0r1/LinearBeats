using System;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Extensions;

namespace LinearBeats.Time
{
    [Flags]
    public enum ScrollEvent
    {
        None = 0,
        Stop = 1 << 0,
        Jump = 1 << 1,
        BackJump = 1 << 2,
        Rewind = 1 << 3,
        Speed = 1 << 4,
        SpeedBounce = 1 << 5,
        All = int.MaxValue,
    }

    public sealed partial class PositionConverter
    {
        private static class ScrollEventConverterFactory
        {
            [CanBeNull]
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
                    ScrollEvent.None => null,
                    ScrollEvent.All => null,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
                };
            }
        }

        private abstract class ScrollEventConverter
        {
            [NotNull] protected readonly ScrollEventPosition[] TimingEvents;

            protected ScrollEventConverter([NotNull] ScrollEventPosition[] timingEvents) =>
                TimingEvents = timingEvents;

            //TODO: 이진탐색 또는 해시탐색을 이용해 스크롤 이벤트당 시간복잡도를 줄이기.
            //HINT: origin >= v.End 인 경우의 누적값은 캐시해 둘 수 있음.
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

            public override void ApplyDistance(ref Position point, Position origin)
            {
                foreach (var v in TimingEvents)
                {
                    if (origin.IsBetweenIE(v.Start, v.End)) point += (v.Amount - 1f) * (origin - v.Start);
                    if (origin >= v.End) point += (v.Amount - 1f) * v.Duration;
                }
            }
        }

        private sealed class SpeedBounceEventConverter : ScrollEventConverter
        {
            public SpeedBounceEventConverter([NotNull] ScrollEventPosition[] timingEvents) : base(timingEvents)
            {
            }

            //FIXME: SpeedBounceEventConverter 구현하기
            public override void ApplyDistance(ref Position point, Position origin)
            {
                foreach (var v in TimingEvents)
                {
                    if (origin.IsBetweenIE(v.Start, v.End)) point += Mathf.Pow((v.Amount - 1f) * (origin - v.Start), 2);
                    if (origin >= v.End) point += Mathf.Pow((v.Amount - 1f) * v.Duration, 2);
                }
            }
        }
    }
}
