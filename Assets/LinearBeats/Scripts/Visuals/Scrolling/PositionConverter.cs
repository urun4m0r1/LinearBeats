using System.Linq;
using LinearBeats.Script;
using LinearBeats.Time;
using Sirenix.Utilities;

namespace LinearBeats.Visuals
{
    public sealed class PositionConverter
    {
        public TimingConverter Converter { get; private set; }

        private TimingEvent[] _normalizedStopEvents = new TimingEvent[] { };
        private TimingEvent[] _normalizedRewindEvents = new TimingEvent[] { };
        private TimingEvent[] _normalizedJumpEvents = new TimingEvent[] { };

        private PositionConverter() { }

        public class Builder
        {
            public PositionConverter PositionConverter { get; private set; }

            public Builder(TimingConverter converter)
            {
                PositionConverter.Converter = converter;
            }

            public Builder StopEvent(TimingEvent[] events)
            {
                PositionConverter._normalizedStopEvents = NormalizeTimingEvents(events);
                return this;
            }

            public Builder RewindEvent(TimingEvent[] events)
            {
                PositionConverter._normalizedRewindEvents = NormalizeTimingEvents(events);
                return this;
            }

            public Builder JumpEvent(TimingEvent[] events)
            {
                PositionConverter._normalizedJumpEvents = NormalizeTimingEvents(events);
                return this;
            }

            public PositionConverter Build()
            {
                return PositionConverter;
            }

            private TimingEvent[] NormalizeTimingEvents(TimingEvent[] timingEvents)
            {
                if (timingEvents.IsNullOrEmpty()) return timingEvents;
                var events = timingEvents.OrderBy(v => v.Pulse);
                var eventsPulse = events.Select(v => PositionConverter.Converter.ToNormalizedPulse(v.Pulse));
                var eventsDuration = events.Select(v => PositionConverter.Converter.ToNormalizedPulse(v.Duration));
                var result = eventsPulse.Zip(eventsDuration, (a, b) => new TimingEvent() { Pulse = a, Duration = b });
                return result.ToArray();
            }
        }

        public float ToPosition(Pulse normalizedPulse)
        {
            float position = normalizedPulse;

            HandleJumpEvents(normalizedPulse, ref position);
            HandleElapsedRewindEvents(normalizedPulse, ref position);
            HandleRewindEvents(normalizedPulse, ref position);
            HandleStopEvents(normalizedPulse, ref position);

            return position;
        }

        private void HandleJumpEvents(Pulse pulse, ref float position)
        {
            foreach (var jumpEvent in from var in _normalizedJumpEvents
                                      let noteDist = pulse - var.Pulse
                                      where noteDist >= 0
                                      select var)
            {
                position += jumpEvent.Duration;
            }
        }

        private void HandleElapsedRewindEvents(Pulse pulse, ref float position)
        {
            foreach (var rewindEvent in from var in _normalizedRewindEvents
                                        let noteDist = pulse - var.Pulse
                                        where noteDist >= var.Duration
                                        select var)
            {
                position -= rewindEvent.Duration * 2;
            }
        }

        private void HandleRewindEvents(Pulse pulse, ref float position)
        {
            foreach (var rewindEvent in from rewindEvent in _normalizedRewindEvents
                                        let noteDist = pulse - rewindEvent.Pulse
                                        where noteDist >= 0 && noteDist < rewindEvent.Duration
                                        select rewindEvent)
            {
                position = (rewindEvent.Pulse * 2) - pulse;
            }
        }

        private void HandleStopEvents(Pulse pulse, ref float position)
        {
            foreach (var stopEvent in from stopEvent in _normalizedStopEvents
                                      let noteDist = pulse - stopEvent.Pulse
                                      where noteDist >= 0 && noteDist < stopEvent.Duration
                                      select stopEvent)
            {
                position = stopEvent.Pulse;
            }
        }
    }
}
