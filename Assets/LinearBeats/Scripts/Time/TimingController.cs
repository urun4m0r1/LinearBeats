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

        public FixedTime CurrentTime
        {
            get => _currentTime;
            private set
            {
                if (_currentTime.Sample != value.Sample)
                {
                    _currentTime = value.Sample;
                    OnProgressChanged();
                }
                if (_currentTime.Bpm != value.Bpm)
                {
                    OnBpmChanged();
                }
            }
        }

        private FixedTime _currentTime;
        private FixedTime _length;
        private FixedTime _offset;

        public void InitTiming(FixedTime length, FixedTime offset)
        {
            _length = length;
            _offset = offset;

            OnProgressChanged();
            OnBpmChanged();
        }

        public void UpdateTiming(FixedTime inputTime)
        {
            //TODO: Is this working?
            FixedTime offsetTime = (Sample)(inputTime.Sample + _offset.Sample);
            CurrentTime = offsetTime;
        }

        public void ResetTiming()
        {
            CurrentTime = (Sample)0f;
        }

        private void OnProgressChanged()
        {
            var progress = CurrentTime.Sample / _length.Sample;
            _onProgressChanged.Invoke(progress);
        }

        private void OnBpmChanged()
        {
            //Always return zero
            _onBpmChanged.Invoke(CurrentTime.Bpm.ToString());
        }
    }
}
