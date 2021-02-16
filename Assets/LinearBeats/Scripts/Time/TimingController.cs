//TODO: 시작 딜레이

#pragma warning disable IDE0051
#pragma warning disable IDE0090

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LinearBeats.Time
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
            }
        }
        private ulong _currentPulse = 0;

        private TimingConverter _timingConverter = null;
        private ulong _pulsesLength = 0;
        private ulong _pulsesOffset = 0;

        public void InitTiming(TimingConverter timingConverter, int samplesLength, ulong pulsesOffset)
        {
            _timingConverter = timingConverter;
            _pulsesLength = timingConverter.SampleToPulse(samplesLength);
            _pulsesOffset = pulsesOffset;

            OnBpmChanged();
            OnProgressChanged();
        }

        private void OnBpmChanged()
        {
            string bpm = _timingConverter.Timings[_timingIndex].Bpm.ToString();
            _onBpmChanged.Invoke(bpm);
        }

        private void OnProgressChanged()
        {
            var progress = (float)_currentPulse / _pulsesLength;
            _onProgressChanged.Invoke(progress);
        }

        public void UpdateTiming(int currentSample)
        {
            TimingIndex = _timingConverter.GetTimingIndex(currentSample);
            CurrentPulse = _timingConverter.SampleToPulse(currentSample, _timingIndex) + _pulsesOffset;
        }

        public void ResetTiming()
        {
            TimingIndex = 0;
            CurrentPulse = 0;
        }
    }
}
