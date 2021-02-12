#pragma warning disable IDE0051

using LinearBeats.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace LinearBeats.Game
{
    public sealed class TimingController
    {
        public ulong CurrentPulse { get; private set; } = 0;

        private readonly Timing[] _timings;
        private readonly int[] _audioFrequencies;
        private readonly ushort _pulsesPerQuarterNote;

        private float[] _pulsesPerSamples = null;
        private float[] _samplePointOnBpmChanges = null;
        private uint _timingIndex = 0;

        public TimingController(Timing[] timings, int[] audioFrequencies, ushort pulsesPerQuarterNote)
        {
            _timings = timings;
            _audioFrequencies = audioFrequencies;
            _pulsesPerQuarterNote = pulsesPerQuarterNote;
            InitiateTimingData();
        }

        private void InitiateTimingData()
        {

            float[] samplesPerPulses = new float[_timings.Length];
            _pulsesPerSamples = new float[_timings.Length];
            _samplePointOnBpmChanges = new float[_timings.Length];
            _samplePointOnBpmChanges[0] = 0f;

            for (var i = 0; i < _timings.Length; ++i)
            {
                float timePerQuarterNote = 60f / _timings[i].Bpm;
                float timePerPulse = timePerQuarterNote / _pulsesPerQuarterNote;
                samplesPerPulses[i] = _audioFrequencies[i] * timePerPulse;
                _pulsesPerSamples[i] = 1 / samplesPerPulses[i];

                if (i != 0)
                {
                    ulong timingRangePulseLength = _timings[i].Pulse - _timings[i - 1].Pulse;
                    float timingRangeSamples = timingRangePulseLength * samplesPerPulses[i - 1];
                    float elapsedSamplesAfterBpmChanged = _samplePointOnBpmChanges[i - 1];
                    _samplePointOnBpmChanges[i] = elapsedSamplesAfterBpmChanged + timingRangeSamples;
                }
            }

            PrintDebug();
        }

        private void PrintDebug()
        {
            Debug.Log("pulsesPerQuarterNote: " + _pulsesPerQuarterNote);
            for (var i = 0; i < _timings.Length; ++i)
            {
                Debug.Log("bpm: " + _timings[i].Bpm);
                Debug.Log("- pulsesPerSamples: " + _pulsesPerSamples[i] + "pulse/Hz");
                Debug.Log("- samplePointOnBpmChanges: " + _samplePointOnBpmChanges[i] + "Hz");
            }
            Debug.Log("initialBpm: " + _timings[0].Bpm);
        }

        public void UpdateTiming(int currentSamples)
        {
            UpdateCurrentPulse(currentSamples);
            UpdateTimingIndex();
        }

        private void UpdateCurrentPulse(int currentSamples)
        {
            float sampleElapsedAfterBpmChanged = currentSamples - _samplePointOnBpmChanges[_timingIndex];
            ulong pulsesElapsedAfterBpmChanged = (ulong)(_pulsesPerSamples[_timingIndex] * sampleElapsedAfterBpmChanged);
            Assert.IsTrue(sampleElapsedAfterBpmChanged >= 0);
            Assert.IsTrue(_pulsesPerSamples[_timingIndex] >= 0);

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
