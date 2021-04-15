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
        bool Normalized { get; }
        float ToPosition(FixedTime fixedTime);
    }

    public sealed class PositionConverter : IPositionConverter
    {
        private readonly struct TimingEventData
        {
            public float Start { get; }
            public float End { get; }
            public float CumulativeDuration { get; }

            public TimingEventData(float start, float duration, float cumulativeDuration)
            {
                Start = start;
                End = start + duration;
                CumulativeDuration = cumulativeDuration;
            }
        }

        [NotNull] private readonly ITimingConverter _timingConverter;
        [NotNull] private TimingEventData[] _stopEvents;
        [NotNull] private TimingEventData[] _rewindEvents;
        [NotNull] private TimingEventData[] _jumpEvents;
        public bool Normalized { get; private set; }

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
            public Builder StopEvent([NotNull] IEnumerable<TimingEvent> events)
            {
                _stopEvents = events.ToArray();
                return this;
            }

            [NotNull]
            public Builder RewindEvent([NotNull] IEnumerable<TimingEvent> events)
            {
                _rewindEvents = events.ToArray();
                return this;
            }

            [NotNull]
            public Builder JumpEvent([NotNull] IEnumerable<TimingEvent> events)
            {
                _jumpEvents = events.ToArray();
                return this;
            }

            [NotNull]
            public Builder Normalize(bool normalize)
            {
                _positionConverter.Normalized = normalize;
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

                var cumulativeDuration = (from v in orderedEvents
                    select Normalize(v.Duration)).CumulativeSum().ToArray();

                return (from v in orderedEvents.Zip(cumulativeDuration, (a, b) => (a, b))
                    let pulse = Normalize(v.a.Pulse)
                    let duration = Normalize(v.a.Duration)
                    select new TimingEventData(pulse, duration, v.b)).ToArray();
            }


            private float Normalize(Pulse value)
            {
                if (!_positionConverter.Normalized) return value;

                var timingIndex = _positionConverter._timingConverter.GetTimingIndex(value);
                return _positionConverter._timingConverter.Normalize(value, timingIndex);
            }
        }


        // ReSharper restore Unity.ExpensiveCode

        public float ToPosition(FixedTime fixedTime)
        {
            //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)
            //TODO: SpeedEvent 처리 (구간강제배속)
            //TODO: Ignore 플래그 처리
            //TODO: 백점프 추가

            var input = Normalized ? fixedTime.NormalizedPulse : (float) fixedTime.Pulse;

            var result = input;
            result += GetJumpDistance(input);
            result -= GetStopDistance(input);
            result -= GetRewindDistance(input);

            return result;
        }

        private float GetJumpDistance(float point) =>
            (from v in _jumpEvents where point > v.Start select v.CumulativeDuration).LastOrDefault();


        private float GetRewindDistance(float point)
        {
            var rewound = (from v in _rewindEvents
                where point >= v.End
                select v.CumulativeDuration * 2).LastOrDefault();

            var rewinding = (from v in _rewindEvents
                where point.IsBetweenIE(v.Start, v.End)
                select (point - v.Start) * 2).FirstOrDefault();

            return rewound + rewinding;
        }

        private float GetStopDistance(float point)
        {
            var stopped = (from v in _stopEvents
                where point >= v.End
                select v.CumulativeDuration).LastOrDefault();

            var stopping = (from v in _stopEvents
                where point.IsBetweenIE(v.Start, v.End)
                select point - v.Start).FirstOrDefault();

            return stopped + stopping;
        }
    }
}
