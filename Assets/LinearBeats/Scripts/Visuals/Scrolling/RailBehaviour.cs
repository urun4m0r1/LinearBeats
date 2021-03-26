using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public class RailBehaviour : MonoBehaviour
    {
#pragma warning disable IDE0044
        [SerializeField]
        private Rigidbody _rigidbody = null;
        private float _noteDisappearOffset = 1f;
#pragma warning restore IDE0044


        public FixedTime FixedTime { get; set; }

        public void UpdateRailPosition(FixedTime currentTime, float meterPerNormalizedPulse)
        {
            if (currentTime.Second - _noteDisappearOffset >= FixedTime.Second)
                SetZPosition(-10f);
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
