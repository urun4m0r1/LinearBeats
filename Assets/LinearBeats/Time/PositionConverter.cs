using System;
using System.Linq;
using LinearBeats.Script;
using Utils.Extensions;

namespace LinearBeats.Time
{
    public interface IPositionConverter : ITimingConverter
    {
        float ToPosition(Pulse normalizedPulse);
    }

    public sealed class PositionConverter : IPositionConverter
    {
        private ITimingConverter TimingConverter { get; }

        private TimingEvent[] _stopEvents = { };
        private TimingEvent[] _rewindEvents = { };
        private TimingEvent[] _jumpEvents = { };

        private PositionConverter(TimingConverter timingConverter) => TimingConverter = timingConverter;

        public sealed class Builder
        {
            private readonly PositionConverter _positionConverter;

            public PositionConverter Build() => _positionConverter;

            public Builder(TimingConverter converter)
            {
                if (converter == null) throw new ArgumentNullException();
                _positionConverter = new PositionConverter(converter);
            }

            public Builder StopEvent(TimingEvent[] events)
            {
                _positionConverter._stopEvents = NormalizeTimingEvents(events);
                return this;
            }

            public Builder RewindEvent(TimingEvent[] events)
            {
                _positionConverter._rewindEvents = NormalizeTimingEvents(events);
                return this;
            }

            public Builder JumpEvent(TimingEvent[] events)
            {
                _positionConverter._jumpEvents = NormalizeTimingEvents(events);
                return this;
            }

            private TimingEvent[] NormalizeTimingEvents(TimingEvent[] timingEvents)
            {
                if (timingEvents.IsNullOrEmpty())
                {
                    throw new IndexOutOfRangeException("At least one TimingEvent item must be decleared");
                }
                if (timingEvents.Any(v => v.Pulse < 0))
                {
                    throw new ArgumentException("All TimingEvent.Pulse must be positive");
                }
                if (timingEvents.Any(v => v.Duration <= 0))
                {
                    throw new ArgumentException("All TimingEvent.Duration must be non-zero positive");
                }

                return (from var in timingEvents
                    orderby var.Pulse
                    let a = _positionConverter.Normalize(var.Pulse)
                    let b = _positionConverter.Normalize(var.Duration)
                    select new TimingEvent {Pulse = a, Duration = b}).ToArray();
            }
        }

        public float ToPosition(Pulse normalizedPulse)
        {
            //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)
            float position = normalizedPulse;

            //TODO: SpeedEvent 처리 (구간강제배속)
            //TODO: Ignore 플래그 처리

            //TODO: 백점프 추가
            HandleJumpEvents(normalizedPulse, ref position);
            HandleElapsedRewindEvents(normalizedPulse, ref position);
            HandleRewindEvents(normalizedPulse, ref position);
            HandleElapsedStopEvents(normalizedPulse, ref position);
            HandleStopEvents(normalizedPulse, ref position);

            return position;
        }

        private void HandleJumpEvents(Pulse pulse, ref float position)
        {
            position += (from var in _jumpEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed >= 0
                select var.Duration).Sum(v => v);
        }

        private void HandleElapsedRewindEvents(Pulse pulse, ref float position)
        {
            position -= (from var in _rewindEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed >= var.Duration
                select var.Duration * 2).Sum(v => v);
        }

        private void HandleRewindEvents(Pulse pulse, ref float position)
        {
            position -= (from var in _rewindEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed.IsBetweenIE(0, var.Duration)
                select pulseElapsed * 2).FirstOrDefault();
        }

        private void HandleElapsedStopEvents(Pulse pulse, ref float position)
        {
            position -= (from var in _stopEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed >= var.Duration
                select var.Duration).Sum(v => v);
        }

        private void HandleStopEvents(Pulse pulse, ref float position)
        {
            position -= (from var in _stopEvents
                let pulseElapsed = pulse - var.Pulse
                where pulseElapsed.IsBetweenIE(0, var.Duration)
                select pulseElapsed).FirstOrDefault();
        }

        private Pulse Normalize(Pulse value) => TimingConverter.Normalize(value, GetTimingIndex(value));

        public Second ToSecond(Sample value) => TimingConverter.ToSecond(value);
        public Sample ToSample(Second value) => TimingConverter.ToSample(value);
        public Pulse ToPulse(Sample value, int timingIndex) => TimingConverter.ToPulse(value, timingIndex);
        public Sample ToSample(Pulse value, int timingIndex) => TimingConverter.ToSample(value, timingIndex);
        public Pulse Normalize(Pulse value, int timingIndex) => TimingConverter.Normalize(value, timingIndex);
        public float GetBpm(int timingIndex) => TimingConverter.GetBpm(timingIndex);
        public int GetTimingIndex(Pulse pulse) => TimingConverter.GetTimingIndex(pulse);
        public int GetTimingIndex(Sample sample) => TimingConverter.GetTimingIndex(sample);
    }
}
