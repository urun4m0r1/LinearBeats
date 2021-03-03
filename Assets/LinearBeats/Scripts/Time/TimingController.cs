//TODO: 시작 딜레이
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

        public int TimingIndex
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
        private int _timingIndex = 0;

        public int CurrentPulse
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
        private int _currentPulse = 0;

        private TimingConverter _timingConverter = null;
        private int _pulsesLength = 0;
        private int _pulsesOffset = 0;

        public void InitTiming(TimingConverter timingConverter, int samplesLength, int pulsesOffset)
        {
            _timingConverter = timingConverter;
            _pulsesLength = timingConverter.SampleToPulse(samplesLength);
            _pulsesOffset = pulsesOffset;

            OnBpmChanged();
            OnProgressChanged();
        }

        private void OnBpmChanged()
        {
            _onBpmChanged.Invoke(_timingConverter.GetBpmText(_timingIndex));
        }

        private void OnProgressChanged()
        {
            var progress = (float)_currentPulse / _pulsesLength;
            _onProgressChanged.Invoke(progress);
        }

        public void UpdateTiming(int currentSample)
        {
            TimingIndex = _timingConverter.GetTimingIndexFromSample(currentSample);
            CurrentPulse = _timingConverter.SampleToPulse(currentSample) + _pulsesOffset;
        }

        public void ResetTiming()
        {
            TimingIndex = 0;
            CurrentPulse = 0;
        }
    }
}
