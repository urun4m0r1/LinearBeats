#pragma warning disable IDE0090

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LinearBeats.Time
{
    [HideReferenceObjectPicker]
    public sealed class TimingController
    {
        private FixedTime _currentTime;
        private FixedTime _length;
        private FixedTime _offset;

        public FixedTime CurrentTime
        {
            get => _currentTime;
            private set
            {
                if (!_currentTime.Equals(value))
               {
                    var progress = value / _length;
                    _onProgressChanged.Invoke(progress.Second);
                }

                if (_currentTime.Bpm != value.Bpm)
                {
                    _onBpmChanged.Invoke(value.Bpm.ToString());
                }

                _currentTime = value;
            }
        }


        public void InitTiming(FixedTime length, FixedTime offset)
        {
            _length = length;
            _offset = offset;

            ResetTiming();
        }

        public void UpdateTiming(FixedTime inputTime)
        {
            //TODO: TimingEvent invoke하기
            CurrentTime = inputTime + _offset;
        }

        public void ResetTiming()
        {
            CurrentTime = _offset;
        }
#pragma warning disable IDE0044
        [HideReferenceObjectPicker] [SerializeField]
        private UnityEvent<string> _onBpmChanged = new UnityEvent<string>();

        [HideReferenceObjectPicker] [SerializeField]
        private UnityEvent<float> _onProgressChanged = new UnityEvent<float>();
#pragma warning restore IDE0044
    }
}
