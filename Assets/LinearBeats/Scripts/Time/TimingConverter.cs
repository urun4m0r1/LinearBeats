using System;
using System.Linq;
using LinearBeats.Script;
using Sirenix.Utilities;
using UnityEngine.Assertions;
using Utils.Extensions;

namespace LinearBeats.Time
{
    public sealed class TimingConverter
    {
        private readonly float[] _ppqn = null;
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
                throw new ArgumentNullException("BpmEvents cannot be null or empty");
            }
            if (timing.BpmEvents.All(v => v.Pulse != 0f))
            {
                throw new ArgumentException("At least one BpmEvent.Pulse must be zero");
            }
            if (timing.BpmEvents.Any(v => v.Pulse < 0f))
            {
                throw new ArgumentException("All BpmEvent.Bpm must be positive");
            }
            if (timing.BpmEvents.Any(v => v.Bpm <= 0f))
            {
                throw new ArgumentException("All BpmEvent.Bpm must be non-zero positive");
            }
            if (timing.BpmEvents.Any(v => v.Ppqn <= 0f))
            {
                throw new ArgumentException("All BpmEvent.Ppqn must be non-zero positive");
            }
            if (samplesPerSecond <= 0f)
            {
                throw new ArgumentException("samplesPerSecond must be non-zero positive");
            }

            var bpmEvents = timing.BpmEvents.OrderBy(v => v.Pulse);
            _bpms = bpmEvents.Select(v => v.Bpm).ToArray();
            _pulses = bpmEvents.Select(v => v.Pulse).ToArray();
            _ppqn = bpmEvents.Select(v => v.Ppqn).ToArray();

            _samplesPerSecond = samplesPerSecond;
            _secondsPerSample = 1f / samplesPerSecond;

            _samplesPerPulse = CalculateSamplesPerPulse();
            _pulsesPerSample = CalculatePulsesPerSample();
            _samples = CalculateSamples();

            float[] CalculateSamplesPerPulse()
            {
                var secondsPerQuarterNote = _bpms.Select(v => 60f / v);
                var secondsPerPulse = secondsPerQuarterNote.Zip(_ppqn, (a, b) => a / b);
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
                var intervalSamples = intervalPulses.Zip(_samplesPerPulse, (a, b) => (float)a * b);
                var samples = intervalSamples.CumulativeSum().PrependWith(0);
                return samples.Select(v => (Sample)v).ToArray();
            }
        }

        public float GetBpm(Second value) => GetBpm(GetTimingIndex(value));
        public float GetBpm(Pulse value) => GetBpm(GetTimingIndex(value));
        public float GetBpm(Sample value) => GetBpm(GetTimingIndex(value));
        public Second ToSecond(Pulse value) => ToSecond(ToSample(value));
        public Pulse ToPulse(Second value) => ToPulse(ToSample(value));
        public Second ToSecond(Sample value) => _secondsPerSample * (float)value;
        public Sample ToSample(Second value) => _samplesPerSecond * (float)value;
        public Sample ToSample(Pulse value) => ToSample(value, GetTimingIndex(value));
        public Pulse ToPulse(Sample value) => ToPulse(value, GetTimingIndex(value));
        public Second ToSecond(Second value) => value;
        public Pulse ToPulse(Pulse value) => value;
        public Sample ToSample(Sample value) => value;

        private float GetBpm(int timingIndex) => _bpms[timingIndex];

        private int GetTimingIndex(Second second) => GetTimingIndex(ToSample(second));
        private int GetTimingIndex(Pulse pulse) => GetTimingIndex(pulse, _pulses);
        private int GetTimingIndex(Sample sample) => GetTimingIndex(sample, _samples);
        private int GetTimingIndex<T>(T timing, T[] sortedTiming) where T : IComparable<T>
        {
            if (timing.CompareTo(sortedTiming[0]) < 0) return 0;
            for (var i = 0; i < sortedTiming.Length - 1; ++i)
            {
                if (timing.IsBetweenIE(sortedTiming[i], sortedTiming[i + 1])) return i;
            }
            return sortedTiming.Length - 1;
        }

        private Pulse ToPulse(Sample sample, int timingIndex)
        {
            if (sample < _samples[0]) timingIndex = 0;
            else Assert.IsTrue(sample >= _samples[timingIndex]);

            var samplesElapsed = sample - _samples[timingIndex];
            var pulsesElapsed = _pulsesPerSample[timingIndex] * samplesElapsed;
            var pulse = _pulses[timingIndex] + pulsesElapsed;

            return pulse;
        }

        private Sample ToSample(Pulse pulse, int timingIndex)
        {
            if (pulse < _pulses[0]) timingIndex = 0;
            else Assert.IsTrue(pulse >= _pulses[timingIndex]);

            var pulsesElapsed = pulse - _pulses[timingIndex];
            var samplesElapsed = _samplesPerPulse[timingIndex] * pulsesElapsed;
            var sample = _samples[timingIndex] + samplesElapsed;

            return sample;
        }
    }
}
