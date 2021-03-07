using System;
using System.Linq;
using LinearBeats.Script;
using Sirenix.Utilities;
using UnityEngine.Assertions;
using Utils.Extensions;

//TODO: timingIndex 관련 private로 만들기

namespace LinearBeats.Time
{
    public sealed class TimingConverter
    {
        private readonly float[] _bpms = null;
        private readonly Pulse[] _pulses = null;
        private readonly Sample[] _samples = null;
        private readonly float _samplesPerSecond = 0f;
        private readonly float _secondsPerSample = 0f;
        private readonly float[] _samplesPerPulse = null;
        private readonly float[] _pulsesPerSample = null;

        public TimingConverter(Timing timing, float samplesPerSecond)
        {
            if (timing.BpmEvents.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }
            if (samplesPerSecond <= 0f || timing.PulsesPerQuarterNote <= 0)
            {
                throw new ArgumentException();
            }
            if (0f.IsIn(timing.BpmEvents.Select(v => v.Bpm).ToArray()))
            {
                throw new ArgumentException();
            }

            var bpmEvents = timing.BpmEvents.OrderBy(v => v.Pulse);
            _bpms = bpmEvents.Select(v => v.Bpm).ToArray();
            _pulses = bpmEvents.Select(v => v.Pulse).ToArray();

            _samplesPerSecond = samplesPerSecond;
            _secondsPerSample = 1f / samplesPerSecond;

            _samplesPerPulse = CalculateSamplesPerPulse();
            _pulsesPerSample = CalculatePulsesPerSample();
            _samples = CalculateSamples();

            float[] CalculateSamplesPerPulse()
            {
                var secondsPerQuarterNote = _bpms.Select(v => 60f / v);
                var secondsPerPulse = secondsPerQuarterNote.Select(v => v / timing.PulsesPerQuarterNote);
                var samplesPerPulse = secondsPerPulse.Select(v => v * _samplesPerSecond);
                return samplesPerPulse.ToArray();
            }

            float[] CalculatePulsesPerSample()
            {
                return _samplesPerPulse.Select(v => 1f / v).ToArray();
            }

            Sample[] CalculateSamples()
            {
                var intervalPulses = _pulses.Zip(_pulses.Skip(1), (current, next) => next - current);
                var intervalSamples = intervalPulses.Zip(_samplesPerPulse, (a, b) => a * b);
                var samples = intervalSamples.CumulativeSum().PrependWith(0);
                return samples.Select(v => (Sample)v).ToArray();
            }
        }

        public float GetBpm(Pulse pulse) => GetBpm(GetTimingIndex(pulse));
        public float GetBpm(Second second) => GetBpm(GetTimingIndex(second));
        public float GetBpm(Sample sample) => GetBpm(GetTimingIndex(sample));

        public float GetBpm(int timingIndex) => _bpms[timingIndex];

        public Pulse ToPulse(Second second) => ToPulse(ToSample(second));
        public Pulse ToPulse(Sample sample) => ToPulse(sample, GetTimingIndex(sample));
        public Second ToSecond(Pulse pulse) => ToSecond(ToSample(pulse));
        public Second ToSecond(Sample sample) => _secondsPerSample * sample;
        public Sample ToSample(Pulse pulse) => ToSample(pulse, GetTimingIndex(pulse));
        public Sample ToSample(Second second) => _samplesPerSecond * second;

        public int GetTimingIndex(Pulse pulse) => GetTimingIndex(pulse, _pulses);
        public int GetTimingIndex(Second second) => GetTimingIndex(ToSample(second));
        public int GetTimingIndex(Sample sample) => GetTimingIndex(sample, _samples);

        private Pulse ToPulse(Sample sample, int timingIndex)
        {
            Assert.IsTrue(sample >= _samples[timingIndex]);

            var samplesElapsed = sample - _samples[timingIndex];
            var pulsesElapsed = _pulsesPerSample[timingIndex] * samplesElapsed;
            var pulse = _pulses[timingIndex] + pulsesElapsed;

            return pulse.RoundToInt();
        }

        private Sample ToSample(Pulse pulse, int timingIndex)
        {
            Assert.IsTrue(pulse >= _pulses[timingIndex]);

            var pulsesElapsed = pulse - _pulses[timingIndex];
            var samplesElapsed = _samplesPerPulse[timingIndex] * pulsesElapsed;
            var sample = _samples[timingIndex] + samplesElapsed;

            return sample;
        }

        private int GetTimingIndex<T>(T timing, T[] sortedTiming) where T : IComparable<T>
        {
            for (var i = 0; i < sortedTiming.Length - 1; ++i)
            {
                if (timing.IsBetweenIE(sortedTiming[i], sortedTiming[i + 1])) return i;
            }
            return sortedTiming.Length - 1;
        }
    }
}
