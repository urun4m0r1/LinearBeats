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
        private readonly int _samplesPerTime = 0;
        private readonly float _timesPerSample = 0f;
        private readonly float[] _samplesPerPulse = null;
        private readonly float[] _pulsesPerSample = null;
        private readonly int[] _pulses = null;
        private readonly int[] _samples = null;

        public TimingConverter(Timing timing, int samplesPerTime)
        {
            _bpmEventsLength = timing.BpmEvents.Length;

            _bpms = (from bpmEvent in timing.BpmEvents select bpmEvent.Bpm).ToArray();
            _pulses = (from bpmEvent in timing.BpmEvents select bpmEvent.Pulse).ToArray();

            _samplesPerTime = samplesPerTime;
            _timesPerSample = 1f / samplesPerTime;

            _samplesPerPulse = GetSamplesPerPulse();
            _pulsesPerSample = _samplesPerPulse.Reciprocal();

            _samples = GetSamples();

            float[] GetSamplesPerPulse()
            {
                var samplesPerPulse = new float[_bpmEventsLength];
                for (var i = 0; i < _bpmEventsLength; ++i)
                {
                    float timePerQuarterNote = 60f / _bpms[i];
                    float timePerPulse = timePerQuarterNote / timing.PulsesPerQuarterNote;
                    samplesPerPulse[i] = _samplesPerTime * timePerPulse;
                }
                return samplesPerPulse;
            }

            int[] GetSamples()
            {
                var timingIntervalSamples = new float[_bpmEventsLength];
                var samples = new int[_bpmEventsLength];
                for (var i = 0; i < _bpmEventsLength; ++i)
                {
                    timingIntervalSamples[i] = _samplesPerPulse[i] * GetTimingIntervalPulses(i);
                    samples[i] = (int)timingIntervalSamples.Sigma(0, i);
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

        public int PulseToTime(int currentPulse)
        {
            return SampleToTime(PulseToSample(currentPulse));
        }

        public int TimeToPulse(int currentTime)
        {
            return SampleToPulse(TimeToSample(currentTime));
        }

        public int SampleToTime(int currentSample)
        {
            return checked((int)(_timesPerSample * currentSample));
        }

        public int TimeToSample(int currentTime)
        {
            return checked(_samplesPerTime * currentTime);
        }

        public int SampleToPulse(int currentSample)
        {
            return SampleToPulse(currentSample, GetTimingIndexFromSample(currentSample));
        }

        public int PulseToSample(int currentPulse)
        {
            return PulseToSample(currentPulse, GetTimingIndexFromPulse(currentPulse));
        }

        private int SampleToPulse(int currentSample, int timingIndex)
        {
            Assert.IsTrue(currentSample >= _samples[timingIndex]);

            int elapsedTimingSamples = currentSample - _samples[timingIndex];
            float elapsedTimingPulses = _pulsesPerSample[timingIndex] * elapsedTimingSamples;
            int currentPulse = checked(_pulses[timingIndex] + (int)elapsedTimingPulses);

            return currentPulse;
        }

        private int PulseToSample(int currentPulse, int timingIndex)
        {
            Assert.IsTrue(currentPulse >= _pulses[timingIndex]);

            int elapsedTimingPulses = currentPulse - _pulses[timingIndex];
            float elapsedTimingSamples = _samplesPerPulse[timingIndex] * elapsedTimingPulses;
            int currentSample = checked(_samples[timingIndex] + (int)elapsedTimingSamples);

            return currentSample;
        }

        public int GetTimingIndexFromTime(int currentTime)
        {
            return GetTimingIndexFromSample(TimeToSample(currentTime));
        }

        public int GetTimingIndexFromSample(int currentSample)
        {
            return GetTimingIndex(currentSample, _samples);
        }

        public int GetTimingIndexFromPulse(int currentPulse)
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
