using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using Sirenix.OdinInspector;
using Utils.Extensions;

namespace LinearBeats.Time
{
    public interface ITimingConverter
    {
        int GetTimingIndex(Pulse pulse);
        int GetTimingIndex(Sample sample);
        float GetBpm(int timingIndex);
        Second ToSecond(Sample value);
        Sample ToSample(Second value);
        Pulse ToPulse(Sample value, int timingIndex);
        Sample ToSample(Pulse value, int timingIndex);
    }

    public interface ITimingModifier
    {
        int GetTimingIndex(Pulse pulse);
        Pulse BpmScale(Pulse value, int timingIndex);
        Pulse BpmNormalize(Pulse value, int timingIndex);
        float Flatten(Pulse value, int timingIndex);
        float Normalize(Pulse value);
    }

    public sealed class TimingConverter : ITimingConverter, ITimingModifier
    {
        private sealed class TimingEvent
        {
            private readonly BpmEvent _bpmEvent;
            [ShowInInspector, ReadOnly, HorizontalGroup(LabelWidth = 30)] public float Ppqn => _bpmEvent.Ppqn;
            [ShowInInspector, ReadOnly, HorizontalGroup] public float Bpm => _bpmEvent.Bpm;
            [ShowInInspector, ReadOnly, HorizontalGroup, LabelText("Tick")] public Pulse Pulse => _bpmEvent.Pulse;
            [ShowInInspector, ReadOnly, HorizontalGroup, LabelText("Hz")] public Sample Sample { get; set; }
            [ShowInInspector, ReadOnly, LabelWidth(150)] public float SamplesPerPulse { get; set; }
            [ShowInInspector, ReadOnly, LabelWidth(150)] public float PulsesPerSample { get; set; }
            [ShowInInspector, ReadOnly, LabelWidth(150)] public float PulseFlattener { get; set; }
            [ShowInInspector, ReadOnly, LabelWidth(150)] public float BpmScaler { get; set; }
            [ShowInInspector, ReadOnly, LabelWidth(150)] public float BpmNormalizer { get; set; }
            [ShowInInspector, ReadOnly, LabelWidth(150)] public Pulse BpmScaledPulse { get; set; }
            [ShowInInspector, ReadOnly, LabelWidth(150)] public Pulse BpmNormalizedPulse { get; set; }

            public TimingEvent(BpmEvent bpmEvent) => _bpmEvent = bpmEvent;
        }

        [ShowInInspector, ReadOnly] [NotNull] private readonly IReadOnlyList<TimingEvent> _timingEvents;
        [ShowInInspector, ReadOnly] private readonly float _samplesPerSecond;
        [ShowInInspector, ReadOnly] private readonly float _secondsPerSample;
        [ShowInInspector, ReadOnly] private readonly float _pulseNormalizer;

        public TimingConverter([NotNull] IReadOnlyCollection<BpmEvent> bpmEvents,
            float samplesPerSecond,
            float? standardBpm = null,
            float? standardPpqn = null)
        {
            if (bpmEvents.Count == 0)
                throw new InvalidScriptException("At least one BpmEvent required");
            if (bpmEvents.Any(v => v.Ppqn <= 0f))
                throw new InvalidScriptException("All BpmEvent.Ppqn must be non-zero positive");
            if (bpmEvents.All(v => v.Pulse != new Pulse(0f)))
                throw new InvalidScriptException("At least one BpmEvent.Pulse must be zero");
            if (bpmEvents.Any(v => v.Pulse < new Pulse(0f)))
                throw new InvalidScriptException("All BpmEvent.Bpm must be positive");
            if (bpmEvents.Any(v => v.Bpm <= 0f))
                throw new InvalidScriptException("All BpmEvent.Bpm must be non-zero positive");
            if (standardBpm <= 0f)
                throw new InvalidScriptException("standardBpm must be non-zero positive");
            if (standardPpqn <= 0f)
                throw new InvalidScriptException("standardPpqn must be non-zero positive");

            if (samplesPerSecond <= 0f)
                throw new ArgumentException("samplesPerSecond must be non-zero positive");

            _timingEvents = (from v in bpmEvents.AsParallel() orderby v.Pulse select new TimingEvent(v)).ToArray();

            _samplesPerSecond = samplesPerSecond;
            _secondsPerSample = 1f / samplesPerSecond;

            var beatNormalizer = 1f / (standardBpm ?? _timingEvents[0].Bpm);
            _pulseNormalizer = 1f / (standardPpqn ?? _timingEvents[0].Ppqn);

            for (var i = 0; i < _timingEvents.Count; ++i)
            {
                var v = _timingEvents[i];
                var prev = i == 0 ? _timingEvents[0] : _timingEvents[i - 1];

                v.PulseFlattener = 1f / v.Ppqn;

                v.BpmScaler = v.Bpm * beatNormalizer;
                v.BpmNormalizer = 1f / v.BpmScaler;

                var secondsPerQuarterNote = 60f / v.Bpm;
                var samplesPerQuarterNote = secondsPerQuarterNote * samplesPerSecond;
                v.SamplesPerPulse = samplesPerQuarterNote / v.Ppqn;
                v.PulsesPerSample = 1f / v.SamplesPerPulse;

                var intervalPulses = v.Pulse - prev.Pulse;

                v.Sample = prev.Sample + intervalPulses * prev.SamplesPerPulse;
                v.BpmScaledPulse = prev.BpmScaledPulse + intervalPulses * prev.BpmScaler;
                v.BpmNormalizedPulse = prev.BpmNormalizedPulse + intervalPulses * prev.BpmNormalizer;
            }
        }

        public int GetTimingIndex(Pulse pulse) => _timingEvents.FindNearestSmallerIndex(pulse, v => v.Pulse);
        public int GetTimingIndex(Sample sample) => _timingEvents.FindNearestSmallerIndex(sample, v => v.Sample);
        public float GetBpm(int timingIndex) => _timingEvents[timingIndex].Bpm;
        public Second ToSecond(Sample value) => Multiply(value, _secondsPerSample);
        public Sample ToSample(Second value) => Multiply(value, _samplesPerSecond);

        public Pulse ToPulse(Sample sample, int timingIndex)
        {
            var v = _timingEvents[timingIndex];
            return MultiplyElapsed(sample - v.Sample, v.PulsesPerSample, v.Pulse);
        }

        public Sample ToSample(Pulse pulse, int timingIndex)
        {
            var v = _timingEvents[timingIndex];
            return MultiplyElapsed(pulse - v.Pulse, v.SamplesPerPulse, v.Sample);
        }

        public Pulse BpmScale(Pulse pulse, int timingIndex)
        {
            var v = _timingEvents[timingIndex];
            return MultiplyElapsed(pulse - v.Pulse, v.BpmScaler, v.BpmScaledPulse);
        }

        public Pulse BpmNormalize(Pulse pulse, int timingIndex)
        {
            var v = _timingEvents[timingIndex];
            return MultiplyElapsed(pulse - v.Pulse, v.BpmNormalizer, v.BpmNormalizedPulse);
        }

        public float Flatten(Pulse pulse, int timingIndex) => Multiply(pulse, _timingEvents[timingIndex].PulseFlattener);

        public float Normalize(Pulse pulse) => Multiply(pulse, _pulseNormalizer);

        private static float MultiplyElapsed(float elapsed, float multiplier, float standard) =>
            standard + Multiply(elapsed, multiplier);

        private static float Multiply(float value, float multiplier) => multiplier * value;
    }
}
