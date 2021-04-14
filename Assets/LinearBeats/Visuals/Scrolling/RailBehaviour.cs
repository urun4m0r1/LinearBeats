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


        public FixedTime StartTime { get; set; }
        public FixedTime Duration { get; set; }

        public void UpdateRailPosition(IPositionConverter positionConverter, FixedTime currentTime,
            float meterPerNormalizedPulse)
        {
            if (currentTime.Second - _noteDisappearOffset >= StartTime)
            {
                SetZPosition(-10f);
            }
            else
            {
                var start = positionConverter.ToPosition(StartTime);
                var current = positionConverter.ToPosition(currentTime);
                //TODO: bpmBounce timingEvent 추가 if(pulseElapsed.BetweenIE(0, bpmBounce.Duration)) positionInMeter *= (bpmBounce.Amount * (pulseElapsed / bpmBounce.Duration));
                var positionInMeter = meterPerNormalizedPulse * (start - current);
                SetZPosition(positionInMeter);

                var scale = transform.localScale;
                var duration = positionConverter.ToPosition(Duration);
                transform.localScale = new Vector3(scale.x, scale.y, meterPerNormalizedPulse * duration);
            }
        }

        private void SetZPosition(float zPosition)
        {
            var position = _rigidbody.position;
            _rigidbody.MovePosition(new Vector3(position.x, position.y, zPosition));
        }
    }
}
