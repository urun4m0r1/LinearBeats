#pragma warning disable IDE0090
#pragma warning disable IDE0051

using UnityEngine;

namespace LinearBeats.Script
{
    public class TimingController
    {
        public ulong CurrentPulse { get; private set; } = 0;
        public float[] PulsesPerSamples { get; set; } = null;
        public float[] SamplePointOnBpmChanges { get; set; } = null;
        public float AudioSampleFreq { get; set; } = 0f;

        private int _timingIndex = 0;
        private readonly Timing[] _timings;
        private readonly ushort _pulsesPerQuarterNote;
        private readonly int[] _frequencies;

        public TimingController(Timing[] timings, ushort pulsesPerQuarterNote, int[] frequencies)
        {
            _timings = timings;
            _pulsesPerQuarterNote = pulsesPerQuarterNote;
            _frequencies = frequencies;
            CalculateTimingData();
        }

        private void CalculateTimingData()
        {
            Debug.Log("pulsesPerQuarterNote: " + _pulsesPerQuarterNote);

            float[] samplesPerPulses = new float[_timings.Length];
            PulsesPerSamples = new float[_timings.Length];
            SamplePointOnBpmChanges = new float[_timings.Length];
            SamplePointOnBpmChanges[0] = 0f;

            for (var i = 0; i < _timings.Length; ++i)
            {
                float timePerQuarterNote = 60f / _timings[i].Bpm;
                float timePerPulse = timePerQuarterNote / _pulsesPerQuarterNote;
                samplesPerPulses[i] = _frequencies[i] * timePerPulse;
                PulsesPerSamples[i] = 1 / samplesPerPulses[i];

                if (i != 0)
                {
                    ulong timingRangePulseLength = _timings[i].Pulse - _timings[i - 1].Pulse;
                    float timingRangeSamples = timingRangePulseLength * samplesPerPulses[i - 1];
                    float elapsedSamplesAfterBpmChanged = SamplePointOnBpmChanges[i - 1];
                    SamplePointOnBpmChanges[i] = elapsedSamplesAfterBpmChanged + timingRangeSamples;
                }

                Debug.Log("bpm: " + _timings[i].Bpm);
                Debug.Log("- timePerQuarterNote: " + (timePerQuarterNote * 1000) + "ms/quarterNote");
                Debug.Log("- timePerPulse: " + (timePerPulse * 1000) + "ms/pulse");
                Debug.Log("- samplesPerPulses: " + samplesPerPulses[i] + "Hz/pulse");
                Debug.Log("- samplePointOnBpmChanges: " + SamplePointOnBpmChanges[i] + "Hz");
            }
            Debug.Log("initialBpm: " + _timings[0].Bpm);
        }

        public void UpdateCurrentPulse(int timeSamples)
        {
            UpdateTiming();

            float sampleElapsedAfterBpmChanged = timeSamples - SamplePointOnBpmChanges[_timingIndex];
            ulong pulsesElapsedAfterBpmChanged = (ulong)(PulsesPerSamples[_timingIndex] * sampleElapsedAfterBpmChanged);
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
