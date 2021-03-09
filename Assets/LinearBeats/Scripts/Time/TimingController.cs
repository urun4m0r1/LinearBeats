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

        public float CurrentBpm
        {
            get => _currentBpm;
            private set
            {
                if (_currentBpm != value)
                {
                    _currentBpm = value;
                    OnBpmChanged();
                }
            }
        }
        private float _currentBpm = 0f;

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
            _length = _timingConverter.ToPulse(length);
            _offset = offset;

            OnBpmChanged();
            OnProgressChanged();
        }

        public void UpdateTiming(Sample currentSample)
        {
            CurrentBpm = _timingConverter.GetBpm(currentSample);
            CurrentPulse = _timingConverter.ToPulse(currentSample);
        }

        public void ResetTiming()
        {
            CurrentBpm = 0f;
            CurrentPulse = 0;
        }

        private void OnBpmChanged()
        {
            _onBpmChanged.Invoke(_currentBpm.ToString());
        }

        private void OnProgressChanged()
        {
            var progress = (float)_currentPulse / _length;
            _onProgressChanged.Invoke(progress);
        }
    }
}
