using System;
using System.Collections.Generic;
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
        private readonly struct NormalizedTimingEvent
        {
            public float Start { get; }
            public float Duration { get; }
            public float End { get; }
            public float CumulativeDuration { get; }

            public NormalizedTimingEvent(float start, float duration, float cumulativeDuration)
            {
                Start = start;
                Duration = duration;
                End = start + duration;
                CumulativeDuration = cumulativeDuration;
            }
        }

        [NotNull] private readonly ITimingConverter _timingConverter;
        [NotNull] private NormalizedTimingEvent[] _stopEvents = { };
        [NotNull] private NormalizedTimingEvent[] _rewindEvents = { };
        [NotNull] private NormalizedTimingEvent[] _jumpEvents = { };

        private PositionConverter([NotNull] ITimingConverter timingConverter) =>
            _timingConverter = timingConverter;

        public sealed class Builder
        {
            [NotNull] private readonly PositionConverter _positionConverter;

            [NotNull]
            public PositionConverter Build() => _positionConverter;

            public Builder([NotNull] ITimingConverter converter) =>
                _positionConverter = new PositionConverter(converter);

            [NotNull]
            public Builder StopEvent([NotNull] IReadOnlyCollection<TimingEvent> events)
            {
                _positionConverter._stopEvents = NormalizeTimingEvents(events);
                return this;
            }

            [NotNull]
            public Builder RewindEvent([NotNull] IReadOnlyCollection<TimingEvent> events)
            {
                _positionConverter._rewindEvents = NormalizeTimingEvents(events);
                return this;
            }

            [NotNull]
            public Builder JumpEvent([NotNull] IReadOnlyCollection<TimingEvent> events)
            {
                _positionConverter._jumpEvents = NormalizeTimingEvents(events);
                return this;
            }

            [NotNull]
            private NormalizedTimingEvent[] NormalizeTimingEvents(
                [NotNull] IReadOnlyCollection<TimingEvent> timingEvents)
            {
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
                    select new NormalizedTimingEvent(pulse, duration, v.b)).ToArray();
            }


            private float Normalize(Pulse value)
            {
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

            var input = fixedTime.NormalizedPulse;

            var result = input;
            result += GetJumpDistance(input);
            result -= GetStoppedDistance(input);
            result -= GetRewoundDistance(input);
            result -= GetStoppingDistance(input);
            result -= GetRewindingDistance(input);

            return result;
        }

        private float GetJumpDistance(float point) =>
            (from v in _jumpEvents where point > v.Start select v.CumulativeDuration).LastOrDefault();

        private float GetStoppedDistance(float point) =>
            (from v in _stopEvents where point >= v.End select v.CumulativeDuration).LastOrDefault();

        private float GetRewoundDistance(float point) =>
            (from v in _rewindEvents where point >= v.End select v.CumulativeDuration * 2).LastOrDefault();

        private float GetRewindingDistance(float point) =>
            (from v in _rewindEvents
                where point.IsBetweenIE(v.Start, v.End)
                select (point - v.Start) * 2).FirstOrDefault();

        private float GetStoppingDistance(float point) =>
            (from v in _stopEvents
                where point.IsBetweenIE(v.Start, v.End)
                select (point - v.Start)).FirstOrDefault();
    }
}
