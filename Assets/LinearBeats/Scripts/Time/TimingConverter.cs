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
        public Timing[] Timings { get => _timings; }

        private readonly Timing[] _timings;
        private readonly float[] _samplesPerPulse = null;
        private readonly float[] _pulsesPerSample = null;
        private readonly ulong[] _pulses = null;
        private readonly int[] _samples = null;

        public TimingConverter(ushort pulsesPerQuarterNote, Timing[] timings, int[] samplesPerTime)
        {
            _timings = timings;
            _pulses = (from timing in timings select timing.Pulse).ToArray();

            _samplesPerPulse = GetSamplesPerPulse();
            _pulsesPerSample = _samplesPerPulse.Reciprocal();

            _samples = GetSamples();

            float[] GetSamplesPerPulse()
            {
                var samplesPerPulse = new float[timings.Length];
                for (var i = 0; i < timings.Length; ++i)
                {
                    float timePerQuarterNote = 60f / timings[i].Bpm;
                    float timePerPulse = timePerQuarterNote / pulsesPerQuarterNote;
                    samplesPerPulse[i] = samplesPerTime[i] * timePerPulse;
                }
                return samplesPerPulse;
            }

            int[] GetSamples()
            {
                var timingIntervalSamples = new float[timings.Length];
                var samples = new int[timings.Length];
                for (var i = 0; i < timings.Length; ++i)
                {
                    timingIntervalSamples[i] = _samplesPerPulse[i] * GetTimingIntervalPulses(i);
                    samples[i] = (int)timingIntervalSamples.Sigma(0, i);
                }
                return samples;

                ulong GetTimingIntervalPulses(int index)
                {
                    if (index < timings.Length - 1) return _pulses[index + 1] - _pulses[index];
                    else return 0;
                }
            }
        }

        public ulong SampleToPulse(int currentSample) => SampleToPulse(currentSample, GetTimingIndex(currentSample));

        public int PulseToSample(ulong currentPulse) => PulseToSample(currentPulse, GetTimingIndex(currentPulse));

        public ulong SampleToPulse(int currentSample, uint timingIndex)
        {
            Assert.IsTrue(currentSample >= _samples[timingIndex]);

            int elapsedTimingSamples = currentSample - _samples[timingIndex];
            float elapsedTimingPulses = _pulsesPerSample[timingIndex] * elapsedTimingSamples;
            ulong currentPulse = checked((ulong)(_pulses[timingIndex] + elapsedTimingPulses));

            return currentPulse;
        }

        public int PulseToSample(ulong currentPulse, uint timingIndex)
        {
            Assert.IsTrue(currentPulse >= _pulses[timingIndex]);

            ulong elapsedTimingPulses = currentPulse - _pulses[timingIndex];
            float elapsedTimingSamples = _samplesPerPulse[timingIndex] * elapsedTimingPulses;
            int currentSample = checked((int)(_samples[timingIndex] + elapsedTimingSamples));

            return currentSample;
        }

        public uint GetTimingIndex(int currentSample) => GetTimingIndex(currentSample, _samples);

        public uint GetTimingIndex(ulong currentPulse) => GetTimingIndex(currentPulse, _pulses);

        private uint GetTimingIndex<T>(T currentTiming, T[] sortedTiming) where T : IComparable<T>
        {
            for (uint i = 0; i < _timings.Length - 1; ++i)
            {
                if (currentTiming.IsBetweenIE(sortedTiming[i], sortedTiming[i + 1])) return i;
            }
            return (uint)(_timings.Length - 1);
        }
    }
}
