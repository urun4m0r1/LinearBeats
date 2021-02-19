using UnityEngine;

namespace LinearBeats.Visuals
{
    public class RailBehaviour : MonoBehaviour
    {
        public ulong Pulse { get; set; } = 0;

        public void UpdateRailPosition(ulong currentPulse, float meterPerPulse)
        {
            //TODO: BPM 정지 구현 ulong a = _script.Timings[0].PulseStopDuration;
            //TODO: BPM 역스크롤 구현 ulong a = _script.Timings[0].PulseReverseDuration (like a folded timeline!)
            //TODO: 변속 대응하기

            float positionInMeter = meterPerPulse * (Pulse - currentPulse);
            SetZPosition(positionInMeter);
        }

        private void SetZPosition(float zPosition)
        {
            //NOTE: rigidBody.MovePosition() 사용하는편이 성능이 낫다.
            transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        }
    }
}
