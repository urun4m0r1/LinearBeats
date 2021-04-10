using System;
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
            public float Pulse { get; }
            public float Duration { get; }

            public NormalizedTimingEvent(float pulse, float duration)
            {
                Pulse = pulse;
                Duration = duration;
            }
        }

        [NotNull] private ITimingConverter TimingConverter { get; }
        [NotNull] private NormalizedTimingEvent[] _stopEvents;
        [NotNull] private NormalizedTimingEvent[] _rewindEvents;
        [NotNull] private NormalizedTimingEvent[] _jumpEvents;

        private PositionConverter([NotNull] ITimingConverter timingConverter) =>
            TimingConverter = timingConverter;

        public sealed class Builder
        {
            [NotNull] private readonly PositionConverter _positionConverter;

            [NotNull]
            public PositionConverter Build() => _positionConverter;

            public Builder([NotNull] ITimingConverter converter)
            {
                if (converter == null) throw new ArgumentNullException();

                _positionConverter = new PositionConverter(converter);
            }

            [NotNull]
            public Builder StopEvent([NotNull] TimingEvent[] events)
            {
                _positionConverter._stopEvents = NormalizeTimingEvents(events);
                return this;
            }

            [NotNull]
            public Builder RewindEvent([NotNull] TimingEvent[] events)
            {
                _positionConverter._rewindEvents = NormalizeTimingEvents(events);
                return this;
            }

            [NotNull]
            public Builder JumpEvent([NotNull] TimingEvent[] events)
            {
                _positionConverter._jumpEvents = NormalizeTimingEvents(events);
                return this;
            }

            [NotNull]
            private NormalizedTimingEvent[] NormalizeTimingEvents([NotNull] TimingEvent[] timingEvents)
            {
                if (timingEvents.IsNullOrEmpty())
                    throw new IndexOutOfRangeException("At least one TimingEvent item must be declared");
                if (timingEvents.Any(v => v.Pulse < 0))
                    throw new ArgumentException("All TimingEvent.Pulse must be positive");
                if (timingEvents.Any(v => v.Duration <= 0))
                    throw new ArgumentException("All TimingEvent.Duration must be non-zero positive");

                return (from var in timingEvents
                    orderby var.Pulse
                    let a = _positionConverter.Normalize(var.Pulse)
                    let b = _positionConverter.Normalize(var.Duration)
                    select new NormalizedTimingEvent(a, b)).ToArray();
            }
        }

        public float ToPosition(FixedTime fixedTime)
        {
            //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)

            var input = fixedTime.NormalizedPulse;
            var result = input;

            //TODO: SpeedEvent 처리 (구간강제배속)
            //TODO: Ignore 플래그 처리

            //TODO: 백점프 추가
            HandleJumpEvents(input, ref result);
            HandleElapsedRewindEvents(input, ref result);
            HandleRewindEvents(input, ref result);
            HandleElapsedStopEvents(input, ref result);
            HandleStopEvents(input, ref result);

            return result;
        }

        private void HandleJumpEvents(float pulse, ref float position)
        {
            position += (from var in _jumpEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed >= 0
                select var.Duration).Sum(v => v);
        }

        private void HandleElapsedRewindEvents(float pulse, ref float position)
        {
            position -= (from var in _rewindEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed >= var.Duration
                select var.Duration * 2).Sum(v => v);
        }

        private void HandleRewindEvents(float pulse, ref float position)
        {
            position -= (from var in _rewindEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed.IsBetweenIE(0, var.Duration)
                select pulseElapsed * 2).FirstOrDefault();
        }

        private void HandleElapsedStopEvents(float pulse, ref float position)
        {
            position -= (from var in _stopEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed >= var.Duration
                select var.Duration).Sum(v => v);
        }

        private void HandleStopEvents(float pulse, ref float position)
        {
            position -= (from var in _stopEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed.IsBetweenIE(0, var.Duration)
                select pulseElapsed).FirstOrDefault();
        }

        private float Normalize(Pulse value) => TimingConverter.Normalize(value, TimingConverter.GetTimingIndex(value));
    }
}
