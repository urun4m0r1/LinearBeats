using System;
using System.Linq;
using LinearBeats.Script;
using UnityEngine.Assertions;
using Utils.Extensions;

namespace LinearBeats.Time
{
    public sealed class TimingConverter
    {
        private readonly int _bpmEventsLength = 0;
        private readonly float[] _bpms = null;
        private readonly Pulse[] _pulses = null;
        private readonly float[] _samples = null;
        private readonly Sample _samplesPerSecond = 0;
        private readonly float _secondsPerSample = 0f;
        private readonly float[] _samplesPerPulse = null;
        private readonly float[] _pulsesPerSample = null;

        public TimingConverter(Timing timing, Sample samplesPerSecond)
        {
            if (timing.BpmEvents.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }
            if (samplesPerSecond <= 0)
            {
                throw new ArgumentException();
            }

            _bpmEventsLength = timing.BpmEvents.Length;

            _bpms = (from bpmEvent in timing.BpmEvents select bpmEvent.Bpm).ToArray();
            _pulses = (from bpmEvent in timing.BpmEvents select bpmEvent.Pulse).ToArray();

            _samplesPerSecond = samplesPerSecond;
            _secondsPerSample = 1f / samplesPerSecond;

            _samplesPerPulse = GetSamplesPerPulse();
            _pulsesPerSample = _samplesPerPulse.Reciprocal();

            _samples = GetSamples();

            float[] GetSamplesPerPulse()
            {
                var samplesPerPulse = new float[_bpmEventsLength];
                for (var i = 0; i < _bpmEventsLength; ++i)
                {
                    float secondsPerQuarterNote = 60f / _bpms[i];
                    float secondsPerPulse = secondsPerQuarterNote / timing.PulsesPerQuarterNote;
                    samplesPerPulse[i] = _samplesPerSecond * secondsPerPulse;
                }
                return samplesPerPulse;
            }

            float[] GetSamples()
            {
                var timingIntervalSamples = new float[_bpmEventsLength];
                var samples = new float[_bpmEventsLength];
                for (var i = 0; i < _bpmEventsLength; ++i)
                {
                    timingIntervalSamples[i] = _samplesPerPulse[i] * GetTimingIntervalPulses(i);
                    samples[i] = timingIntervalSamples.Sigma(0, i);
                }
                return samples;

                int GetTimingIntervalPulses(int index)
                {
                    if (index < _bpmEventsLength - 1) return _pulses[index + 1] - _pulses[index];
                    else return 0;
                }
            }
        }

        public string GetBpmText(int timingIndex)
        {
            return _bpms[timingIndex].ToString();
        }

        public Second ToSecond(Pulse currentPulse)
        {
            return ToSecond(ToSample(currentPulse));
        }

        public Pulse ToPulse(Second currentSecond)
        {
            return ToPulse(ToSample(currentSecond));
        }

        public Second ToSecond(Sample currentSample)
        {
            return _secondsPerSample * currentSample;
        }

        public Sample ToSample(Second currentSecond)
        {
            return (_samplesPerSecond * currentSecond).RoundToInt();
        }

        public Pulse ToPulse(Sample currentSample)
        {
            return ToPulse(currentSample, GetTimingIndexFromSample(currentSample));
        }

        public Sample ToSample(Pulse currentPulse)
        {
            return ToSample(currentPulse, GetTimingIndexFromPulse(currentPulse));
        }

        private Pulse ToPulse(Sample currentSample, int timingIndex)
        {
            Assert.IsTrue(currentSample >= _samples[timingIndex]);

            float elapsedTimingSamples = currentSample - _samples[timingIndex];
            float elapsedTimingPulses = _pulsesPerSample[timingIndex] * elapsedTimingSamples;
            float currentPulse = _pulses[timingIndex] + elapsedTimingPulses;

            return currentPulse.RoundToInt();
        }

        private Sample ToSample(Pulse currentPulse, int timingIndex)
        {
            Assert.IsTrue(currentPulse >= _pulses[timingIndex]);

            Pulse elapsedTimingPulses = currentPulse - _pulses[timingIndex];
            float elapsedTimingSamples = _samplesPerPulse[timingIndex] * elapsedTimingPulses;
            float currentSample = _samples[timingIndex] + elapsedTimingSamples;

            return currentSample.RoundToInt();
        }

        public int GetTimingIndexFromSecond(Second currentSecond)
        {
            return GetTimingIndexFromSample(ToSample(currentSecond));
        }

        public int GetTimingIndexFromSample(Sample currentSample)
        {
            return GetTimingIndex(currentSample, _samples);
        }

        public int GetTimingIndexFromPulse(Pulse currentPulse)
        {
            return GetTimingIndex(currentPulse, _pulses);
        }

        private int GetTimingIndex<T>(T currentTiming, T[] sortedTiming) where T : IComparable<T>
        {
            for (var i = 0; i < _bpmEventsLength - 1; ++i)
            {
                if (currentTiming.IsBetweenIE(sortedTiming[i], sortedTiming[i + 1])) return i;
            }
            return _bpmEventsLength - 1;
        }
    }
}

