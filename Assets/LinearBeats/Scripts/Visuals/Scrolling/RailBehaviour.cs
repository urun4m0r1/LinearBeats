using LinearBeats.Time;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public class RailBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody = null;
        public Pulse Pulse { get; set; } = 0;

        //public Timing Timing { get; set; } = 0;

        public void UpdateRailPosition(Pulse currentPulse, float meterPerPulse)
        {
            //TODO: BPM 정지 구현 int a = _script.Timings[0].PulseStopDuration;
            //TODO: BPM 역스크롤 구현 int a = _script.Timings[0].PulseReverseDuration (like a folded timeline!)
            //TODO: 변속 대응하기

            float positionInMeter = meterPerPulse * (Pulse - currentPulse);
            SetZPosition(positionInMeter);
        }

        private void SetZPosition(float zPosition)
        {
            _rigidbody.MovePosition(new Vector3(_rigidbody.position.x, _rigidbody.position.y, zPosition));
        }
    }
}
