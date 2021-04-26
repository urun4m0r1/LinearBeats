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
        Position Convert(Pulse pulse, [NotNull] IDictionary<TimingEventType, bool> ignore);
    }

    public sealed partial class PositionConverter : IPositionConverter
    {
        private static readonly IEnumerable<TimingEventType> AllTimingEventTypes =
            (TimingEventType[]) Enum.GetValues(typeof(TimingEventType));

        [NotNull] private readonly ITimingModifier _modifier;
        [NotNull] private PositionScaler _scaler;
        [NotNull] private PositionNormalizer _normalizer;
        [NotNull] private readonly IDictionary<TimingEventType, TimingEventConverter> _converter;

        private PositionConverter([NotNull] ITimingModifier modifier)
        {
            _modifier = modifier;
            _scaler = new PositionRelativeScaler(modifier);
            _normalizer = new IndividualNormalizer(modifier);
            _converter = new Dictionary<TimingEventType, TimingEventConverter>();
        }

        public sealed class Builder
        {
            [NotNull] private readonly PositionConverter _base;
            [CanBeNull] private IDictionary<TimingEventType, TimingEvent[]> _events;

            public Builder([NotNull] ITimingModifier modifier) => _base = new PositionConverter(modifier);

            [NotNull]
            public Builder SetPositionScaler(ScalerMode mode)
            {
                _base._scaler = mode switch
                {
                    ScalerMode.RegularInterval => new RegularIntervalScaler(_base._modifier),
                    ScalerMode.BpmRelative => new PositionRelativeScaler(_base._modifier),
                    ScalerMode.ConstantSpeed => new ConstantSpeedScaler(_base._modifier),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
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
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };

                return this;
            }

            [NotNull]
            public Builder SetTimingEvents([NotNull] IDictionary<TimingEventType, TimingEvent[]> events)
            {
                _events = events;
                return this;
            }

            [NotNull]
            public PositionConverter Build()
            {
                foreach (var v in AllTimingEventTypes)
                {
                    var timingEventConverter = TimingEventConverterFactory.Create(v, EnrichTimingEvents(_events?[v]));
                    _base._converter.Add(v, timingEventConverter);
                }

                return _base;
            }

            [NotNull]
            private TimingEventPosition[] EnrichTimingEvents(
                [CanBeNull] IReadOnlyCollection<TimingEvent> timingEvent)
            {
                if (timingEvent == null) return new TimingEventPosition[] { };

                if (timingEvent.Count == 0)
                    throw new ArgumentException("At least one TimingEvent required");
                if (timingEvent.Any(v => v.Pulse < 0))
                    throw new ArgumentException("All TimingEvent.Pulse must be positive");
                if (timingEvent.Any(v => v.Duration <= 0))
                    throw new ArgumentException("All TimingEvent.Duration must be non-zero positive");

                return (from v in timingEvent.OrderBy(v => v.Pulse)
                    let start = _base.ToPosition(v.Pulse)
                    let end = _base.ToPosition(v.Pulse + v.Duration)
                    select new TimingEventPosition(start, end)).ToArray();
            }
        }

        private Position ToPosition(Pulse pulse)
        {
            var timingIndex = _modifier.GetTimingIndex(pulse);
            var scaledPulse = _scaler.Scale(pulse, timingIndex);
            var position = _normalizer.Normalize(scaledPulse, timingIndex);
            return position;
        }

        public Position Convert(Pulse pulse, IDictionary<TimingEventType, bool> ignore) =>
            ApplyTimingEvents(ToPosition(pulse), ignore);

        [SuppressMessage("ReSharper", "LoopCanBePartlyConvertedToQuery")]
        private Position ApplyTimingEvents(Position origin, [NotNull] IDictionary<TimingEventType, bool> ignore)
        {
            var result = origin;

            foreach (var v in AllTimingEventTypes)
                if (!ignore[v])
                    _converter[v].ApplyDistance(ref result, origin);

            return result;
        }
    }
}
