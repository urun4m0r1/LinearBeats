using System;
using System.Collections.Generic;
using System.Linq;
using LinearBeats.Script;
using LinearBeats.Time;
using Sirenix.Utilities;
using UnityEngine.Assertions;

namespace LinearBeats.Visuals
{
    public sealed class PositionConverter
    {
        private readonly TimingConverter _converter;
        private readonly TimingEvent[] _normalizedStopEvents;
        private readonly TimingEvent[] _normalizedRewindEvents;
        private readonly TimingEvent[] _normalizedJumpEvents;

        public PositionConverter(Timing timing, TimingConverter converter)
        {
            _converter = converter;
            _normalizedStopEvents = NormalizePulse(timing.StopEvents);
            _normalizedRewindEvents = NormalizePulse(timing.RewindEvents);
            _normalizedJumpEvents = NormalizePulse(timing.JumpEvents);

            TimingEvent[] NormalizePulse(TimingEvent[] timingEvents)
            {
                if (timingEvents.IsNullOrEmpty()) return new TimingEvent[] { };
                var events = timingEvents.OrderBy(v => v.Pulse);
                var eventsPulse = events.Select(v => converter.ToNormalizedPulse(v.Pulse));
                var eventsDuration = events.Select(v => converter.ToNormalizedPulse(v.Duration));
                var result = eventsPulse.Zip(eventsDuration, (a, b) => new TimingEvent() { Pulse = a, Duration = b });
                return result.ToArray();
            }
        }

        public float ToPosition(FixedTime fixedTime, FixedTime currentTime)
        {
            if (_converter != fixedTime.Converter) throw new InvalidOperationException();

            Pulse note = fixedTime.NormalizedPulse;
            Pulse current = currentTime.NormalizedPulse;

            Pulse newNote = note;
            Pulse newCurrent = current;

            foreach (var jumpEvent in _normalizedJumpEvents)
            {
                Pulse noteDist = note - jumpEvent.Pulse;
                Pulse currentDist = current - jumpEvent.Pulse;

                if (noteDist >= 0)
                    newNote += jumpEvent.Duration;
                if (currentDist >= 0)
                    newCurrent += jumpEvent.Duration;
            }

            foreach (var rewindEvent in _normalizedRewindEvents)
            {
                Pulse noteDist = note - rewindEvent.Pulse;
                Pulse currentDist = current - rewindEvent.Pulse;

                if (noteDist >= rewindEvent.Duration)
                    newNote -= rewindEvent.Duration * 2;
                if (currentDist >= rewindEvent.Duration)
                    newCurrent -= rewindEvent.Duration * 2;
            }

            foreach (var rewindEvent in _normalizedRewindEvents)
            {
                Pulse noteDist = note - rewindEvent.Pulse;
                Pulse currentDist = current - rewindEvent.Pulse;

                if (noteDist >= 0 && noteDist < rewindEvent.Duration)
                    newNote = rewindEvent.Pulse - noteDist;
                if (currentDist >= 0 && currentDist < rewindEvent.Duration)
                    newCurrent = rewindEvent.Pulse - currentDist;
            }

            foreach (var stopEvent in _normalizedStopEvents)
            {
                Pulse noteDist = note - stopEvent.Pulse;
                Pulse currentDist = current - stopEvent.Pulse;

                if (noteDist >= 0 && noteDist < stopEvent.Duration)
                    newNote = stopEvent.Pulse;
                if (currentDist >= 0 && currentDist < stopEvent.Duration)
                    newCurrent = stopEvent.Pulse;
            }

            return newNote - newCurrent;


        }
    }
}
