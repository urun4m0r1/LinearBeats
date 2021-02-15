#pragma warning disable IDE0051
#pragma warning disable IDE0090

using LinearBeats.Script;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Utils.Extensions;

namespace LinearBeats.Game
{
    [HideReferenceObjectPicker]
    public sealed class TimingController
    {
#pragma warning disable IDE0044
        [HideReferenceObjectPicker]
        [SerializeField]
        private UnityEvent<string> _onBpmChanged = new UnityEvent<string>();

        [HideReferenceObjectPicker]
        [SerializeField]
        private UnityEvent<float> _onProgressChanged = new UnityEvent<float>();
#pragma warning restore IDE0044
        public uint TimingIndex
        {
            get => _timingIndex;
            private set
            {
                if (_timingIndex != value)
                {
                    _timingIndex = value;
                    OnBpmChanged();
                }
            }
        }
        private uint _timingIndex = 0;

        public ulong CurrentPulse
        {
            get => _currentPulse;
            private set
            {
                if (_currentPulse != value)
                {
                    _currentPulse = value;
                    OnProgressChanged();
                }
                _currentPulse = value;
            }
        }
        private ulong _currentPulse = 0;

        private Timing[] _timings;
        private ulong _pulsesLength = 0;
        private float[] _pulsesPerSample = null;
        private float[] _samplePointsOnBpmChange = null;

        public void InitTiming(Timing[] timings, int[] samplesPerTime, ushort pulsesPerQuarterNote, int samplesLength)
        {
            _timings = timings;

            float[] samplesPerPulse = GetSamplesPerPulse(samplesPerTime, pulsesPerQuarterNote);

            _pulsesPerSample = samplesPerPulse.Reciprocal();
            _samplePointsOnBpmChange = GetSamplePointsOnBpmChange(samplesPerPulse);

            _pulsesLength = ConvertSamplePointToPulse(samplesLength);

            OnBpmChanged();
            OnProgressChanged();
        }

        private float[] GetSamplesPerPulse(int[] samplesPerTime, ushort pulsesPerQuarterNote)
        {
            var samplesPerPulse = new float[_timings.Length];
            for (var i = 0; i < _timings.Length; ++i)
            {
                float timePerQuarterNote = 60f / _timings[i].Bpm;
                float timePerPulse = timePerQuarterNote / pulsesPerQuarterNote;
                samplesPerPulse[i] = samplesPerTime[i] * timePerPulse;
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
        }

        private ulong GetTimingIntervalPulses(int index)
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

        private ulong ConvertSamplePointToPulse(int currentSamplePoint)
        {
            return ConvertSamplePointToPulse(currentSamplePoint, GetTimingIndex(currentSamplePoint));
        }

        private ulong ConvertSamplePointToPulse(int currentSamplePoint, uint timingIndex)
        {
            Assert.IsTrue(timingIndex >= 0);
            float samplesElapsedAfterBpmChanged = currentSamplePoint - _samplePointsOnBpmChange[timingIndex];
            Assert.IsTrue(samplesElapsedAfterBpmChanged >= 0);

            ulong pulsesElapsedAfterBpmChanged = (ulong)(_pulsesPerSample[timingIndex] * samplesElapsedAfterBpmChanged);
            return checked(_timings[timingIndex].Pulse + pulsesElapsedAfterBpmChanged);
        }

        private void OnBpmChanged()
        {
            string bpm = _timings[TimingIndex].Bpm.ToString();
            _onBpmChanged.Invoke(bpm);
        }

        private void OnProgressChanged()
        {
            float progress = (float)CurrentPulse / (float)_pulsesLength;
            _onProgressChanged.Invoke(progress);
        }

        public void UpdateTiming(int currentSamplePoint)
        {
            TimingIndex = GetTimingIndex(currentSamplePoint);
            CurrentPulse = ConvertSamplePointToPulse(currentSamplePoint, TimingIndex);
        }

        private uint GetTimingIndex(int currentSamplePoint)
        {
            for (uint i = 0; i < _timings.Length - 1; ++i)
            {
                bool withinTimingRange = currentSamplePoint >= _samplePointsOnBpmChange[i]
                    && currentSamplePoint < _samplePointsOnBpmChange[i + 1];
                if (withinTimingRange)
                {
                    return i;
                }
            }
            return (uint)(_timings.Length - 1);
        }

        public void ResetTiming()
        {
            TimingIndex = 0;
            CurrentPulse = 0;
        }
    }
}
