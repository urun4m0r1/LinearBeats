using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;

namespace LinearBeats.Time
{
    public interface IPositionConverter
    {
        Position Convert(Pulse pulse, ScrollEvent ignoreFlags);
    }

    public sealed partial class PositionConverter : IPositionConverter
    {
        private static readonly IEnumerable<ScrollEvent> EveryScrollEvents =
            (ScrollEvent[]) Enum.GetValues(typeof(ScrollEvent));

        [NotNull] private readonly ITimingModifier _modifier;
        [NotNull] private PositionScaler _scaler;
        [NotNull] private PositionNormalizer _normalizer;
        [NotNull] private readonly IDictionary<ScrollEvent, ScrollEventConverter> _converters;

        private PositionConverter([NotNull] ITimingModifier modifier)
        {
            _modifier = modifier;
            _scaler = new PositionRelativeScaler(modifier);
            _normalizer = new IndividualNormalizer(modifier);
            _converters = new Dictionary<ScrollEvent, ScrollEventConverter>();
        }

        public sealed class Builder
        {
            [NotNull] private readonly PositionConverter _base;
            [NotNull] private readonly IDictionary<ScrollEvent, ScrollingEvent[]> _timingEvents;

            public Builder([NotNull] ITimingModifier modifier)
            {
                _base = new PositionConverter(modifier);
                _timingEvents = new Dictionary<ScrollEvent, ScrollingEvent[]>();
            }

            [NotNull]
            public Builder SetPositionScaler(ScalerMode mode)
            {
                _base._scaler = mode switch
                {
                    ScalerMode.RegularInterval => new RegularIntervalScaler(_base._modifier),
                    ScalerMode.BpmRelative => new PositionRelativeScaler(_base._modifier),
                    ScalerMode.ConstantSpeed => new ConstantSpeedScaler(_base._modifier),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
                };

                return this;
            }

            [NotNull]
            public Builder SetPositionNormalizer(NormalizerMode mode)
            {
                _base._normalizer = mode switch
                {
                    NormalizerMode.Individual => new IndividualNormalizer(_base._modifier),
                    NormalizerMode.Standard => new StandardNormalizer(_base._modifier),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
                };

                return this;
            }

            [NotNull]
            public Builder SetScrollEvent(Script.Scrolling scrolling, ScrollEvent ignoreFlags = ScrollEvent.None)
            {
                //TODO: Remove Duplicated code
                AddEvent(ScrollEvent.Stop, scrolling.StopEvents);
                AddEvent(ScrollEvent.Jump, scrolling.JumpEvents);
                AddEvent(ScrollEvent.BackJump, scrolling.BackJumpEvents);
                AddEvent(ScrollEvent.Rewind, scrolling.RewindEvents);
                AddEvent(ScrollEvent.Speed, scrolling.SpeedEvents);
                AddEvent(ScrollEvent.SpeedBounce, scrolling.SpeedBounceEvents);

                return this;

                void AddEvent(ScrollEvent type, ScrollingEvent[] scrollingEvents)
                {
                    if (scrollingEvents != null && (ignoreFlags & type) == 0)
                        _timingEvents.Add(type, scrollingEvents);
                }
            }

            [NotNull]
            public PositionConverter Build()
            {
                foreach (var type in EveryScrollEvents)
                {
                    if (!_timingEvents.TryGetValue(type, out var timingEvents)) continue;

                    var scrollEventPositions = ToScrollEventPositions(timingEvents);
                    var scrollEventConverter = ScrollEventConverterFactory.Create(type, scrollEventPositions);
                    if (scrollEventConverter != null) _base._converters.Add(type, scrollEventConverter);
                }

                return _base;
            }

            [NotNull]
            private ScrollEventPosition[] ToScrollEventPositions(
                [CanBeNull] IReadOnlyCollection<ScrollingEvent> timingEvents)
            {
                if (timingEvents == null) return new ScrollEventPosition[] { };

                if (timingEvents.Count == 0)
                    throw new ArgumentException("At least one TimingEvent required");
                if (timingEvents.Any(v => v.Pulse < 0))
                    throw new ArgumentException("All TimingEvent.Pulse must be positive");
                if (timingEvents.Any(v => v.Duration <= 0))
                    throw new ArgumentException("All TimingEvent.Duration must be non-zero positive");

                return (from v in timingEvents.OrderBy(v => v.Pulse)
                    let start = _base.ToPosition(v.Pulse)
                    let end = _base.ToPosition(v.Pulse + v.Duration)
                    select new ScrollEventPosition(start, end, v.Amount)).ToArray();
            }
        }

        public Position Convert(Pulse pulse, ScrollEvent ignoreFlags) => ApplyTimingEvents(ToPosition(pulse), ignoreFlags);

        private Position ToPosition(Pulse pulse)
        {
            var timingIndex = _modifier.GetTimingIndex(pulse);
            var scaledPulse = _scaler.Scale(pulse, timingIndex);
            var position = _normalizer.Normalize(scaledPulse, timingIndex);
            return position;
        }

        [SuppressMessage("ReSharper", "LoopCanBePartlyConvertedToQuery")]
        private Position ApplyTimingEvents(Position origin, ScrollEvent ignoreFlags)
        {
            var result = origin;

            foreach (var type in EveryScrollEvents)
            {
                var ignoreEvent = (ignoreFlags & type) != 0;
                if (ignoreEvent || !_converters.TryGetValue(type, out var converter)) continue;

                converter.ApplyDistance(ref result, origin);
            }

            return result;
        }
    }
}
