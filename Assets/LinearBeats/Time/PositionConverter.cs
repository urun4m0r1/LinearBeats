using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using Utils.Extensions;

namespace LinearBeats.Time
{
    public interface IPositionConverter
    {
        Position Convert(Pulse pulse, TimingEventOptions? options = null);
    }

    public sealed partial class PositionConverter : IPositionConverter
    {
        [NotNull] private readonly ITimingModifier _modifier;
        [NotNull] private TimingEventPosition[] _stopEvents;
        [NotNull] private TimingEventPosition[] _rewindEvents;
        [NotNull] private TimingEventPosition[] _jumpEvents;
        [NotNull] private PositionScaler _scaler;
        [NotNull] private PositionNormalizer _normalizer;

        [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized")]
        private PositionConverter([NotNull] ITimingModifier modifier) => _modifier = modifier;

        public sealed class Builder
        {
            [NotNull] private readonly PositionConverter _base;
            [CanBeNull] private TimingEvent[] _stopEvents;
            [CanBeNull] private TimingEvent[] _rewindEvents;
            [CanBeNull] private TimingEvent[] _jumpEvents;

            public Builder([NotNull] ITimingModifier modifier) => _base = new PositionConverter(modifier);

            [NotNull]
            public Builder SetStopEvent([CanBeNull] IEnumerable<TimingEvent> events)
            {
                _stopEvents = events?.ToArray();
                return this;
            }

            [NotNull]
            public Builder SetRewindEvent([CanBeNull] IEnumerable<TimingEvent> events)
            {
                _rewindEvents = events?.ToArray();
                return this;
            }

            [NotNull]
            public Builder SetJumpEvent([CanBeNull] IEnumerable<TimingEvent> events)
            {
                _jumpEvents = events?.ToArray();
                return this;
            }

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
            public PositionConverter Build()
            {
                _base._stopEvents = ToTimingEventPosition(_stopEvents);
                _base._rewindEvents = ToTimingEventPosition(_rewindEvents);
                _base._jumpEvents = ToTimingEventPosition(_jumpEvents);

                return _base;
            }

            [NotNull]
            private TimingEventPosition[] ToTimingEventPosition(
                [CanBeNull] IReadOnlyCollection<TimingEvent> timingEvents)
            {
                if (timingEvents == null) return new TimingEventPosition[] { };

                if (timingEvents.Count == 0)
                    throw new ArgumentException("At least one TimingEvent required");
                if (timingEvents.Any(v => v.Pulse < 0))
                    throw new ArgumentException("All TimingEvent.Pulse must be positive");
                if (timingEvents.Any(v => v.Duration <= 0))
                    throw new ArgumentException("All TimingEvent.Duration must be non-zero positive");

                return (from v in timingEvents.OrderBy(v => v.Pulse)
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

        //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)
        //TODO: SpeedEvent 처리 (구간강제배속)
        //TODO: 백점프 추가

        public Position Convert(Pulse pulse, TimingEventOptions? options = null) =>
            ApplyTimingEvents(ToPosition(pulse), options ?? default);

        private Position ApplyTimingEvents(Position origin, TimingEventOptions options)
        {
            var result = origin;
            if (!options.IgnoreJump) ApplyJumpDistance(ref result, origin);
            if (!options.IgnoreStop) ApplyStopDistance(ref result, origin);
            if (!options.IgnoreRewind) ApplyRewindDistance(ref result, origin);
            return result;
        }

        private void ApplyJumpDistance(ref Position point, Position origin)
        {
            foreach (var v in _jumpEvents)
            {
                if (origin.IsBetweenIE(v.Start, v.End)) point += v.Duration;
                if (origin >= v.End) point += v.Duration;
            }
        }

        private void ApplyRewindDistance(ref Position point, Position origin)
        {
            foreach (var v in _rewindEvents)
            {
                if (origin.IsBetweenIE(v.Start, v.End)) point -= (origin - v.Start) * 2;
                if (origin >= v.End) point -= v.Duration * 2;
            }
        }

        private void ApplyStopDistance(ref Position point, Position origin)
        {
            foreach (var v in _stopEvents)
            {
                if (origin.IsBetweenIE(v.Start, v.End)) point -= origin - v.Start;
                if (origin >= v.End) point -= v.Duration;
            }
        }
    }
}
