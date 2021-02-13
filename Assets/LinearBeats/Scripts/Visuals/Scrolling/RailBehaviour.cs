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

            float positionInMeter = meterPerPulse * (Pulse - currentPulse);
            SetZPosition(positionInMeter);
        }

        private void SetZPosition(float zPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        }
    }
}
