using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public class RailBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody = null;
        public FixedTime FixedTime { get; set; }

        public void UpdateRailPosition(FixedTime currentTime, float meterPerNormalizedPulse, PositionConverter positionConverter)
        {
            if (FixedTime <= currentTime)
                SetZPosition(0);
            else
            {
                float positionInMeter = meterPerNormalizedPulse * positionConverter.ToPosition(FixedTime, currentTime);
                SetZPosition(positionInMeter);
            }
        }

        private void SetZPosition(float zPosition)
        {
            _rigidbody.MovePosition(new Vector3(_rigidbody.position.x, _rigidbody.position.y, zPosition));
        }
    }
}
