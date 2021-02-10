using UnityEngine;

namespace LinearBeats.Script
{
    public sealed class TimingController
    {
        public ulong CurrentPulse { get; private set; } = 0;

        private readonly Timing[] _timings;
        private readonly ushort _pulsesPerQuarterNote;
        private readonly int[] _audioFrequencies = null;

        private float[] _pulsesPerSamples = null;
        private float[] _samplePointOnBpmChanges = null;

        private int _timingIndex = 0;

        public TimingController(Timing[] timings, int[] audioFrequencies, ushort pulsesPerQuarterNote)
        {
            _timings = timings;
            _audioFrequencies = audioFrequencies;
            _pulsesPerQuarterNote = pulsesPerQuarterNote;
            CalculateTimingData();
        }

        private void CalculateTimingData()
        {
            Debug.Log("pulsesPerQuarterNote: " + _pulsesPerQuarterNote);

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

                Debug.Log("bpm: " + _timings[i].Bpm);
                Debug.Log("- timePerQuarterNote: " + (timePerQuarterNote * 1000) + "ms/quarterNote");
                Debug.Log("- timePerPulse: " + (timePerPulse * 1000) + "ms/pulse");
                Debug.Log("- samplesPerPulses: " + samplesPerPulses[i] + "Hz/pulse");
                Debug.Log("- samplePointOnBpmChanges: " + _samplePointOnBpmChanges[i] + "Hz");
            }
            Debug.Log("initialBpm: " + _timings[0].Bpm);
        }

        public void UpdateCurrentPulse(int timeSamples)
        {
            UpdateTiming();

            float sampleElapsedAfterBpmChanged = timeSamples - _samplePointOnBpmChanges[_timingIndex];
            ulong pulsesElapsedAfterBpmChanged = (ulong)(_pulsesPerSamples[_timingIndex] * sampleElapsedAfterBpmChanged);
            CurrentPulse = _timings[_timingIndex].Pulse + pulsesElapsedAfterBpmChanged;
            //Debug.Log($"{_currentPulse}");

            void UpdateTiming()
            {
                if (_timingIndex + 1 < _timings.Length)
                {
                    if (CurrentPulse >= _timings[_timingIndex + 1].Pulse)
                    {
                        ++_timingIndex;
                        Debug.Log("currentBpm: " + _timings[_timingIndex].Bpm);
                    }
                }
            }
        }

        public void ResetTiming()
        {
            _timingIndex = 0;
        }
    }
}
