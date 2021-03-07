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

        public Pulse CurrentPulse
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
        private Pulse _currentPulse = 0;

        private TimingConverter _timingConverter = null;
        private Pulse _length = 0;
        private Second _offset = 0;

        public void InitTiming(TimingConverter timingConverter, Sample length, Second offset)
        {
            _timingConverter = timingConverter;
            _length = timingConverter.ToPulse(length);
            _offset = offset;

            OnBpmChanged();
            OnProgressChanged();
        }

        private void OnBpmChanged()
        {
            _onBpmChanged.Invoke(_timingConverter.GetBpm(_timingIndex).ToString());
        }

        private void OnProgressChanged()
        {
            var progress = (float)_currentPulse / _length;
            _onProgressChanged.Invoke(progress);
        }

        public void UpdateTiming(Sample currentSample)
        {
            TimingIndex = _timingConverter.GetTimingIndex(currentSample);
            CurrentPulse = _timingConverter.ToPulse(currentSample);
        }

        public void ResetTiming()
        {
            TimingIndex = 0;
            CurrentPulse = 0;
        }
    }
}
