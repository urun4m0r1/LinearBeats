using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public class RailBehaviour : MonoBehaviour
    {
#pragma warning disable IDE0044
        [SerializeField]
        private Rigidbody _rigidbody;
        private float _noteDisappearOffset = 1f;
#pragma warning restore IDE0044


        public FixedTime FixedTime { get; set; }

        public void UpdateRailPosition(IPositionConverter positionConverter, FixedTime currentTime,
            float meterPerNormalizedPulse)
        {
            if ((Second) currentTime - _noteDisappearOffset >= FixedTime)
            {
                SetZPosition(-10f);
            }
            else
            {
                var a = positionConverter.ToPosition(FixedTime);
                var b = positionConverter.ToPosition(currentTime);
                //TODO: bpmBounce timingEvent 추가 if(pulseElapsed.BetweenIE(0, bpmBounce.Duration)) positionInMeter *= (bpmBounce.Amount * (pulseElapsed / bpmBounce.Duration));
                float positionInMeter = meterPerNormalizedPulse * (a - b);
                SetZPosition(positionInMeter);
            }
        }

        private void SetZPosition(float zPosition)
        {
            var position = _rigidbody.position;
            _rigidbody.MovePosition(new Vector3(position.x, position.y, zPosition));
        }
    }
}
