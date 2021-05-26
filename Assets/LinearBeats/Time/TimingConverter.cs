using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Scrolling;
using Sirenix.OdinInspector;
using UnityEngine;
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

    public sealed class TimingEvent
    {
        private readonly BpmEvent _bpmEvent;

        public TimingEvent(BpmEvent bpmEvent) => _bpmEvent = bpmEvent;

        [ShowInInspector, ReadOnly, HorizontalGroup(LabelWidth = 30)] public float Ppqn => _bpmEvent.Ppqn;
        [ShowInInspector, ReadOnly, HorizontalGroup] public float Bpm => _bpmEvent.Bpm;
        [ShowInInspector, ReadOnly, HorizontalGroup, LabelText("Tick")] public Pulse Pulse => _bpmEvent.Pulse;
        [ShowInInspector, ReadOnly, HorizontalGroup, LabelText("Hz")] public Sample Sample { get; set; }
        [ShowInInspector, ReadOnly, LabelWidth(150)] public float SamplesPerPulse { get; set; }
        [ShowInInspector, ReadOnly, LabelWidth(150)] public float PulsesPerSample { get; set; }
        [ShowInInspector, ReadOnly, LabelWidth(150)] public float PulseFlattener { get; set; }
        [ShowInInspector, ReadOnly, LabelWidth(150)] public float BpmScaler { get; set; }
        [ShowInInspector, ReadOnly, LabelWidth(150)] public float BpmNormalizer { get; set; }
        [ShowInInspector, ReadOnly, LabelWidth(150)] public float BpmScaledPulse { get; set; }
        [ShowInInspector, ReadOnly, LabelWidth(150)] public float BpmNormalizedPulse { get; set; }
    }

    public sealed class TimingConverter : ITimingConverter, ITimingModifier
    {
        [ShowInInspector, ReadOnly] [NotNull] private readonly IReadOnlyList<TimingEvent> _timingEvents;
        [ShowInInspector, ReadOnly] private readonly float _samplesPerSecond;
        [ShowInInspector, ReadOnly] private readonly float _secondsPerSample;
        [ShowInInspector, ReadOnly] private readonly float _pulseNormalizer;
        [ShowInInspector, ReadOnly] private readonly float _beatNormalizer;

        public TimingConverter([NotNull] IReadOnlyCollection<BpmEvent> bpmEvents,
            float samplesPerSecond, float? standardBpm = null, float? standardPpqn = null)
        {
            ValidateArgs();

            _timingEvents = (from v in bpmEvents.AsParallel() orderby v.Pulse select new TimingEvent(v)).ToArray();

            _samplesPerSecond = samplesPerSecond;
            _secondsPerSample = 1f / samplesPerSecond;

            _beatNormalizer = 1f / (standardBpm ?? _timingEvents[0].Bpm);
            _pulseNormalizer = 1f / (standardPpqn ?? _timingEvents[0].Ppqn);

            for (var i = 0; i < _timingEvents.Count; ++i)
            {
                var current = _timingEvents[i];
                var previous = i == 0 ? _timingEvents[0] : _timingEvents[i - 1];

                SetTimingEvent(ref current, previous);
            }

            void ValidateArgs()
            {
                if (bpmEvents.Count == 0)
                    throw new InvalidScriptException("At least one BpmEvent required");
                if (bpmEvents.Any(v => v.Ppqn <= 0f))
                    throw new InvalidScriptException("All BpmEvent.Ppqn must be non-zero positive");
                if (bpmEvents.All(v => v.Pulse != new Pulse(0)))
                    throw new InvalidScriptException("At least one BpmEvent.Pulse must be zero");
                if (bpmEvents.Any(v => v.Pulse < new Pulse(0)))
                    throw new InvalidScriptException("All BpmEvent.Bpm must be positive");
                if (bpmEvents.Any(v => v.Bpm <= 0f))
                    throw new InvalidScriptException("All BpmEvent.Bpm must be non-zero positive");
                if (standardBpm <= 0f)
                    throw new InvalidScriptException("standardBpm must be non-zero positive");
                if (standardPpqn <= 0f)
                    throw new InvalidScriptException("standardPpqn must be non-zero positive");

                if (samplesPerSecond <= 0f)
                    throw new ArgumentException("samplesPerSecond must be non-zero positive");
            }
        }

        private void SetTimingEvent([NotNull] ref TimingEvent current, [NotNull] TimingEvent previous)
        {
            current.PulseFlattener = 1f / current.Ppqn;

            current.BpmScaler = current.Bpm * _beatNormalizer;
            current.BpmNormalizer = 1f / current.BpmScaler;

            var secondsPerQuarterNote = 60f / current.Bpm;
            var samplesPerQuarterNote = secondsPerQuarterNote * _samplesPerSecond;
            current.SamplesPerPulse = samplesPerQuarterNote / current.Ppqn;
            current.PulsesPerSample = 1f / current.SamplesPerPulse;

            var intervalPulses = current.Pulse - previous.Pulse;

            current.Sample = previous.Sample + Mathf.RoundToInt(intervalPulses * previous.SamplesPerPulse);
            current.BpmScaledPulse = previous.BpmScaledPulse + intervalPulses * previous.BpmScaler;
            current.BpmNormalizedPulse = previous.BpmNormalizedPulse + intervalPulses * previous.BpmNormalizer;
        }

        public int GetTimingIndex(Pulse pulse) => _timingEvents.FindNearestSmallerIndex(pulse, v => v.Pulse);
        public int GetTimingIndex(Sample sample) => _timingEvents.FindNearestSmallerIndex(sample, v => v.Sample);
        public float GetBpm(int timingIndex) => _timingEvents[timingIndex].Bpm;
        public Second ToSecond(Sample value) => value * _secondsPerSample;
        public Sample ToSample(Second value) => Mathf.RoundToInt(value * _samplesPerSecond);

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

        public float BpmScale(Pulse pulse, int timingIndex)
        {
            var v = _timingEvents[timingIndex];
            return MultiplyElapsed(pulse - v.Pulse, v.BpmScaler, v.BpmScaledPulse);
        }

        public float BpmNormalize(Pulse pulse, int timingIndex)
        {
            var v = _timingEvents[timingIndex];
            return MultiplyElapsed(pulse - v.Pulse, v.BpmNormalizer, v.BpmNormalizedPulse);
        }

        public Position Flatten(float scaledPulse, int timingIndex) => scaledPulse * _timingEvents[timingIndex].PulseFlattener;

        public Position Normalize(float scaledPulse) => scaledPulse * _pulseNormalizer;

        private static float MultiplyElapsed(int elapsed, float multiplier, float standard) =>
            standard + elapsed * multiplier;

        private static int MultiplyElapsed(int elapsed, float multiplier, int standard) =>
            standard + Mathf.RoundToInt(elapsed * multiplier);
    }
}
