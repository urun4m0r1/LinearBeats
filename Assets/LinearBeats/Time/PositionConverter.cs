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
        float ToPosition(FixedTime fixedTime);
    }

    public sealed class PositionConverter : IPositionConverter
    {
        private readonly struct TimingEventData
        {
            public float Start { get; }
            public float End { get; }
            public float Duration { get; }

            public TimingEventData(float start, float end)
            {
                Start = start;
                End = end;
                Duration = end - start;
            }
        }

        [NotNull] private readonly ITimingConverter _timingConverter;
        [NotNull] private TimingEventData[] _stopEvents;
        [NotNull] private TimingEventData[] _rewindEvents;
        [NotNull] private TimingEventData[] _jumpEvents;
        private bool _scaled = true;
        private bool _normalized = true;

        [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized")]
        private PositionConverter([NotNull] ITimingConverter timingConverter) =>
            _timingConverter = timingConverter;

        public sealed class Builder
        {
            [NotNull] private readonly PositionConverter _positionConverter;
            [CanBeNull] private TimingEvent[] _stopEvents;
            [CanBeNull] private TimingEvent[] _rewindEvents;
            [CanBeNull] private TimingEvent[] _jumpEvents;

            [NotNull]
            public PositionConverter Build()
            {
                _positionConverter._stopEvents = Convert(_stopEvents);
                _positionConverter._rewindEvents = Convert(_rewindEvents);
                _positionConverter._jumpEvents = Convert(_jumpEvents);

                return _positionConverter;
            }

            public Builder([NotNull] ITimingConverter converter) =>
                _positionConverter = new PositionConverter(converter);

            [NotNull]
            public Builder StopEvent([CanBeNull] IEnumerable<TimingEvent> events)
            {
                _stopEvents = events?.ToArray();
                return this;
            }

            [NotNull]
            public Builder RewindEvent([CanBeNull] IEnumerable<TimingEvent> events)
            {
                _rewindEvents = events?.ToArray();
                return this;
            }

            [NotNull]
            public Builder JumpEvent([CanBeNull] IEnumerable<TimingEvent> events)
            {
                _jumpEvents = events?.ToArray();
                return this;
            }

            [NotNull]
            public Builder Scale(bool scale)
            {
                _positionConverter._scaled = scale;
                return this;
            }

            [NotNull]
            public Builder Normalize(bool normalize)
            {
                _positionConverter._normalized = normalize;
                return this;
            }

            [NotNull]
            private TimingEventData[] Convert([CanBeNull] IReadOnlyCollection<TimingEvent> timingEvents)
            {
                if (timingEvents == null) return new TimingEventData[] { };

                if (timingEvents.Count == 0)
                    throw new ArgumentException("At least one TimingEvent required");
                if (timingEvents.Any(v => v.Pulse < 0))
                    throw new ArgumentException("All TimingEvent.Pulse must be positive");
                if (timingEvents.Any(v => v.Duration <= 0))
                    throw new ArgumentException("All TimingEvent.Duration must be non-zero positive");

                var orderedEvents = timingEvents.OrderBy(v => v.Pulse).ToArray();

                return (from v in orderedEvents
                    let sp = ApplyScale(v.Pulse, v.Pulse + v.Duration)
                    let np = ApplyNormalize(sp.Start, sp.End)
                    select new TimingEventData(np.Start, np.End)).ToArray();
            }

            private (Pulse Start, Pulse End) ApplyScale(Pulse start, Pulse end)
            {
                if (!_positionConverter._scaled) return (start, end);

                var converter = _positionConverter._timingConverter;
                var timingIndex = converter.GetTimingIndex(start);
                start = converter.Scale(start, timingIndex);
                end = converter.Scale(end, timingIndex);

                return (start, end);
            }

            private (float Start, float End) ApplyNormalize(Pulse start, Pulse end)
            {
                var converter = _positionConverter._timingConverter;

                if (_positionConverter._normalized)
                {
                    var timingIndex = converter.GetTimingIndex(start);
                    start = converter.Normalize(start, timingIndex);
                    end = converter.Normalize(end, timingIndex);
                }
                else
                {
                    start = converter.Flatten(start);
                    end = converter.Flatten(end);
                }

                return (start, end);
            }
        }

        //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)
        //TODO: SpeedEvent 처리 (구간강제배속)
        //TODO: Ignore 플래그 처리
        //TODO: 백점프 추가

        // ReSharper restore Unity.ExpensiveCode

        public float ToPosition(FixedTime fixedTime)
        {
            var index = _timingConverter.GetTimingIndex(fixedTime.Pulse);
            var pulse = _scaled ? _timingConverter.Scale(fixedTime.Pulse, index) : fixedTime.Pulse;
            var original = _normalized ? _timingConverter.Normalize(pulse, index) : _timingConverter.Flatten(pulse);

            return AddDistance(original);
        }

        private float AddDistance(float original)
        {
            var result = original;
            result += GetJumpDistance(original);
            result += GetStopDistance(original);
            result += GetRewindDistance(original);
            return result;
        }

        private float GetJumpDistance(float point)
        {
            var pos = _jumpEvents.Where(v => point >= v.Start)
                .Aggregate(0f, (current, v) => current + v.Duration);

            return pos;
        }


        private float GetRewindDistance(float point)
        {
            var pos = 0f;
            foreach (var v in _rewindEvents)
            {
                if (point.IsBetweenIE(v.Start, v.End)) pos -= (point - v.Start) * 2;
                if (point >= v.End) pos -= v.Duration * 2;
            }

            return pos;
        }

        private float GetStopDistance(float point)
        {
            var pos = 0f;
            foreach (var v in _stopEvents)
            {
                if (point.IsBetweenIE(v.Start, v.End)) pos -= point - v.Start;
                if (point >= v.Start + v.Duration) pos -= v.Duration;
            }

            return pos;
        }
    }
}
