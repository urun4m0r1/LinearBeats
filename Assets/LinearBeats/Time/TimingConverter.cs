using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using Utils.Extensions;

namespace LinearBeats.Time
{
    public interface ITimingConverter
    {
        Second ToSecond(Sample value);
        Sample ToSample(Second value);
        Pulse ToPulse(Sample value, int timingIndex);
        Sample ToSample(Pulse value, int timingIndex);
        float Normalize(Pulse value, int timingIndex);
        float GetBpm(int timingIndex);
        int GetTimingIndex(Pulse pulse);
        int GetTimingIndex(Sample sample);
    }

    public sealed class TimingConverter : ITimingConverter
    {
        private readonly float _samplesPerSecond;
        private readonly float _secondsPerSample;

        [NotNull] private readonly float[] _ppqns;
        [NotNull] private readonly Pulse[] _pulses;
        [NotNull] private readonly float[] _bpms;
        [NotNull] private readonly float[] _bpmScales;

        [NotNull] private readonly Pulse[] _scaledPulses;
        [NotNull] private readonly Sample[] _samples;
        [NotNull] private readonly float[] _samplesPerPulse;
        [NotNull] private readonly float[] _pulsesPerSample;

        public TimingConverter([NotNull] IReadOnlyCollection<BpmEvent> bpmEvents,
            float standardBpm,
            float samplesPerSecond)
        {
            if (bpmEvents.Count == 0)
                throw new ArgumentException("At least one BpmEvent required");
            if (bpmEvents.Any(v => v.Ppqn <= 0f))
                throw new ArgumentException("All BpmEvent.Ppqn must be non-zero positive");
            if (bpmEvents.All(v => v.Pulse != new Pulse(0f)))
                throw new ArgumentException("At least one BpmEvent.Pulse must be zero");
            if (bpmEvents.Any(v => v.Pulse < new Pulse(0f)))
                throw new ArgumentException("All BpmEvent.Bpm must be positive");
            if (bpmEvents.Any(v => v.Bpm <= 0f))
                throw new ArgumentException("All BpmEvent.Bpm must be non-zero positive");
            if (samplesPerSecond <= 0f)
                throw new ArgumentException("samplesPerSecond must be non-zero positive");

            _samplesPerSecond = samplesPerSecond;
            _secondsPerSample = 1f / samplesPerSecond;

            var orderedBpmEvents = (from v in bpmEvents.AsParallel() orderby v.Pulse select v).ToArray();
            _ppqns = (from v in orderedBpmEvents select v.Ppqn).ToArray();
            _pulses = (from v in orderedBpmEvents select v.Pulse).ToArray();
            _bpms = (from v in orderedBpmEvents select v.Bpm).ToArray();
            _bpmScales = (from v in orderedBpmEvents select v.Bpm / standardBpm).ToArray();

            _samplesPerPulse = CalculateSamplesPerPulse().ToArray();
            _pulsesPerSample = CalculatePulsesPerSample().ToArray();

            var intervalPulses = _pulses.Zip(_pulses.Skip(1), (current, next) => next - current).ToArray();

            _samples = CalculateSamples().ToArray();
            _scaledPulses = CalculateScaledPulses().ToArray();

            IEnumerable<float> CalculateSamplesPerPulse()
            {
                var samplesPerQuarterNote = from v in _bpms select (60f / v) * _samplesPerSecond;
                return samplesPerQuarterNote.Zip(_ppqns, (a, b) => a / b);
            }

            IEnumerable<float> CalculatePulsesPerSample() => (from v in _samplesPerPulse select 1f / v);

            IEnumerable<Sample> CalculateSamples() =>
                (from v in ScaledCumulativeSum(intervalPulses, _samplesPerPulse) select (Sample) v);

            IEnumerable<Pulse> CalculateScaledPulses() =>
                (from v in ScaledCumulativeSum(intervalPulses, _bpmScales) select (Pulse) v);

            static IEnumerable<float> ScaledCumulativeSum<T>(IEnumerable<T> value, IEnumerable<float> scales)
                where T : IFloat =>
                value.Zip(scales, (a, b) => a.ToFloat() * b).CumulativeSum().Prepend(0);
        }

        public Second ToSecond(Sample value) => _secondsPerSample * (float) value;
        public Sample ToSample(Second value) => _samplesPerSecond * (float) value;

        public Pulse ToPulse(Sample sample, int timingIndex)
        {
            var samplesElapsed = sample - _samples[timingIndex];
            var pulsesElapsed = _pulsesPerSample[timingIndex] * samplesElapsed;
            return _pulses[timingIndex] + pulsesElapsed;
        }

        public Sample ToSample(Pulse pulse, int timingIndex)
        {
            var pulsesElapsed = pulse - _pulses[timingIndex];
            var samplesElapsed = _samplesPerPulse[timingIndex] * pulsesElapsed;
            return _samples[timingIndex] + samplesElapsed;
        }

        public float Normalize(Pulse pulse, int timingIndex)
        {
            var pulsesElapsed = pulse - _pulses[timingIndex];
            var scaledPulsesElapsed = _bpmScales[timingIndex] * pulsesElapsed;
            var scaledPulse = _scaledPulses[timingIndex] + scaledPulsesElapsed;
            return scaledPulse / _ppqns[timingIndex];
        }

        public float GetBpm(int timingIndex) => _bpms[timingIndex];

        public int GetTimingIndex(Pulse pulse) => GetTimingIndex(pulse, _pulses);
        public int GetTimingIndex(Sample sample) => GetTimingIndex(sample, _samples);

        private static int GetTimingIndex<T>(T timing, [NotNull] IReadOnlyList<T> orderedTiming)
            where T : struct, IComparable<T>
        {
            if (timing.CompareTo(orderedTiming.First()) < 0) return 0;

            for (var i = 0; i < orderedTiming.Count - 1; ++i)
                if (timing.CompareTo(orderedTiming[i + 1]) < 0)
                    return i;

            return orderedTiming.Count - 1;
        }
    }
}
