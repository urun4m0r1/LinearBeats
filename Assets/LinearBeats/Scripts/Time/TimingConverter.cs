#pragma warning disable IDE0051
#pragma warning disable IDE0090

using System;
using System.Linq;
using LinearBeats.Script;
using UnityEngine.Assertions;
using Utils.Extensions;

namespace LinearBeats.Time
{
    public sealed class TimingConverter
    {
        public BpmEvent[] BpmEvents { get => _bpmEvents; }

        private readonly BpmEvent[] _bpmEvents;
        private readonly float[] _samplesPerPulse = null;
        private readonly float[] _pulsesPerSample = null;
        private readonly int[] _pulses = null;
        private readonly int[] _samples = null;

        public TimingConverter(Timing timing, int[] samplesPerTime)
        {
            _bpmEvents = timing.BpmEvents;
            _pulses = (from bpmEvent in _bpmEvents select bpmEvent.Pulse).ToArray();

            _samplesPerPulse = GetSamplesPerPulse();
            _pulsesPerSample = _samplesPerPulse.Reciprocal();

            _samples = GetSamples();

            float[] GetSamplesPerPulse()
            {
                var samplesPerPulse = new float[_bpmEvents.Length];
                for (var i = 0; i < _bpmEvents.Length; ++i)
                {
                    float timePerQuarterNote = 60f / _bpmEvents[i].Bpm;
                    float timePerPulse = timePerQuarterNote / timing.PulsesPerQuarterNote;
                    samplesPerPulse[i] = samplesPerTime[i] * timePerPulse;
                }
                return samplesPerPulse;
            }

            int[] GetSamples()
            {
                var timingIntervalSamples = new float[_bpmEvents.Length];
                var samples = new int[_bpmEvents.Length];
                for (var i = 0; i < _bpmEvents.Length; ++i)
                {
                    timingIntervalSamples[i] = _samplesPerPulse[i] * GetTimingIntervalPulses(i);
                    samples[i] = (int)timingIntervalSamples.Sigma(0, i);
                }
                return samples;

                int GetTimingIntervalPulses(int index)
                {
                    if (index < _bpmEvents.Length - 1) return _pulses[index + 1] - _pulses[index];
                    else return 0;
                }
            }
        }

        public int SampleToPulse(int currentSample) => SampleToPulse(currentSample, GetTimingIndexFromSample(currentSample));

        public int PulseToSample(int currentPulse) => PulseToSample(currentPulse, GetTimingIndexFromPulse(currentPulse));

        public int SampleToPulse(int currentSample, uint timingIndex)
        {
            Assert.IsTrue(currentSample >= _samples[timingIndex]);

            int elapsedTimingSamples = currentSample - _samples[timingIndex];
            float elapsedTimingPulses = _pulsesPerSample[timingIndex] * elapsedTimingSamples;
            int currentPulse = checked((int)(_pulses[timingIndex] + elapsedTimingPulses));

            return currentPulse;
        }

        public int PulseToSample(int currentPulse, uint timingIndex)
        {
            Assert.IsTrue(currentPulse >= _pulses[timingIndex]);

            int elapsedTimingPulses = currentPulse - _pulses[timingIndex];
            float elapsedTimingSamples = _samplesPerPulse[timingIndex] * elapsedTimingPulses;
            int currentSample = checked((int)(_samples[timingIndex] + elapsedTimingSamples));

            return currentSample;
        }

        public uint GetTimingIndexFromSample(int currentSample) => GetTimingIndex(currentSample, _samples);

        public uint GetTimingIndexFromPulse(int currentPulse) => GetTimingIndex(currentPulse, _pulses);

        private uint GetTimingIndex<T>(T currentTiming, T[] sortedTiming) where T : IComparable<T>
        {
            for (uint i = 0; i < _bpmEvents.Length - 1; ++i)
            {
                if (currentTiming.IsBetweenIE(sortedTiming[i], sortedTiming[i + 1])) return i;
            }
            return (uint)(_bpmEvents.Length - 1);
        }
    }
}
