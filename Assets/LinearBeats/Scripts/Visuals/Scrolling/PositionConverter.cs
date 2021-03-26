using System;
using System.Linq;
using LinearBeats.Script;
using LinearBeats.Time;
using Sirenix.Utilities;
using Utils.Extensions;

namespace LinearBeats.Visuals
{
    public sealed class PositionConverter
    {
        public TimingConverter Converter { get; }

        private TimingEvent[] _stopEvents = new TimingEvent[] { };
        private TimingEvent[] _rewindEvents = new TimingEvent[] { };
        private TimingEvent[] _jumpEvents = new TimingEvent[] { };

        private PositionConverter(TimingConverter converter) => Converter = converter;

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
                        orderby var.Pulse ascending
                        let converter = _positionConverter.Converter
                        let a = converter.Normalize(var.Pulse)
                        let b = converter.Normalize(var.Duration)
                        select new TimingEvent() { Pulse = a, Duration = b }).ToArray();
            }
        }

        public float ToPosition(Pulse normalizedPulse)
        {
            float position = normalizedPulse;

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
    }
}
