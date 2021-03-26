using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public class RailBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody = null;
        public FixedTime FixedTime { get; set; }

        public void UpdateRailPosition(FixedTime currentTime, float meterPerNormalizedPulse)
        {
            if (FixedTime <= currentTime)
                SetZPosition(0);
            else
            {
                float positionInMeter = meterPerNormalizedPulse * (FixedTime.Position - currentTime.Position);
                SetZPosition(positionInMeter);
            }
        }

        private void SetZPosition(float zPosition)
        {
            _rigidbody.MovePosition(new Vector3(_rigidbody.position.x, _rigidbody.position.y, zPosition));
        }
    }
}
