#pragma warning disable IDE0051

using LinearBeats.Script;
using UnityEngine;
using UnityEngine.Assertions;
using Utils.Extensions;

namespace LinearBeats.Game
{
    public sealed class TimingController
    {
        public ulong CurrentPulse { get; private set; } = 0;

        private readonly Timing[] _timings;
        private readonly int[] _samplesPerTime;
        private readonly ushort _pulsesPerQuarterNote;

        private float[] _pulsesPerSample = null;
        private float[] _samplePointsOnBpmChange = null;
        private uint _timingIndex = 0;

        public TimingController(Timing[] timings, int[] samplesPerTime, ushort pulsesPerQuarterNote)
        {
            _timings = timings;
            _samplesPerTime = samplesPerTime;
            _pulsesPerQuarterNote = pulsesPerQuarterNote;

            InitTimingData();
        }

        private void InitTimingData()
        {
            float[] samplesPerPulse = GetSamplesPerPulse();
            _pulsesPerSample = samplesPerPulse.Reciprocal();
            _samplePointsOnBpmChange = GetSamplePointsOnBpmChange(samplesPerPulse);
        }

        private float[] GetSamplesPerPulse()
        {
            var samplesPerPulse = new float[_timings.Length];
            for (var i = 0; i < _timings.Length; ++i)
            {
                float timePerQuarterNote = 60f / _timings[i].Bpm;
                float timePerPulse = timePerQuarterNote / _pulsesPerQuarterNote;
                samplesPerPulse[i] = _samplesPerTime[i] * timePerPulse;
            }
            return samplesPerPulse;
        }

        private float[] GetSamplePointsOnBpmChange(float[] samplesPerPulse)
        {
            var timingIntervalSamples = new float[_timings.Length];
            var samplePointsOnBpmChange = new float[_timings.Length];
            for (var i = 0; i < _timings.Length; ++i)
            {
                timingIntervalSamples[i] = samplesPerPulse[i] * GetTimingIntervalPulses(i);
                samplePointsOnBpmChange[i] = timingIntervalSamples.Sigma(0, i);
            }
            return samplePointsOnBpmChange;

            ulong GetTimingIntervalPulses(int index)
            {
                if (index < _timings.Length - 1)
                {
                    return _timings[index + 1].Pulse - _timings[index].Pulse;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void UpdateTiming(int currentSamplePoint)
        {
            UpdateCurrentPulse(currentSamplePoint);
            UpdateTimingIndex();
        }

        private void UpdateCurrentPulse(int currentSamplePoint)
        {
            float samplesElapsedAfterBpmChanged = currentSamplePoint - _samplePointsOnBpmChange[_timingIndex];
            Assert.IsTrue(samplesElapsedAfterBpmChanged >= 0);

            ulong pulsesElapsedAfterBpmChanged = (ulong)(_pulsesPerSample[_timingIndex] * samplesElapsedAfterBpmChanged);
            CurrentPulse = checked(_timings[_timingIndex].Pulse + pulsesElapsedAfterBpmChanged);
        }

        private void UpdateTimingIndex()
        {
            uint nextTimingIndex = checked(_timingIndex + 1);
            bool canIncreaseTimingIndex = nextTimingIndex < _timings.Length;
            if (canIncreaseTimingIndex)
            {
                bool shouldIncreaseTimingIndex = CurrentPulse >= _timings[nextTimingIndex].Pulse;
                if (shouldIncreaseTimingIndex)
                {
                    _timingIndex = nextTimingIndex;
                    OnTimingIndexUpdate();
                }
            }
        }

        public void ResetTiming()
        {
            _timingIndex = 0;
            OnTimingIndexUpdate();
        }

        private void OnTimingIndexUpdate()
        {
            Debug.Log($"currentBpm: {_timings[_timingIndex].Bpm}");
        }
    }
}
