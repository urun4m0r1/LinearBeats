using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public class RailBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody = null;
        public FixedTime FixedTime { get; set; }

        //public Timing Timing { get; set; } = 0;

        public void UpdateRailPosition(FixedTime currentTime, float meterPerSecond)
        {
            //TODO: BPM 정지 구현 int a = _script.Timings[0].PulseStopDuration;
            //TODO: BPM 역스크롤 구현 int a = _script.Timings[0].PulseReverseDuration (like a folded timeline!)

            if (FixedTime <= currentTime)
                SetZPosition(0);
            else
            {
                float positionInMeter = meterPerSecond * (FixedTime - currentTime).Second;
                SetZPosition(positionInMeter);
            }
        }

        private void SetZPosition(float zPosition)
        {
            _rigidbody.MovePosition(new Vector3(_rigidbody.position.x, _rigidbody.position.y, zPosition));
        }
    }
}
