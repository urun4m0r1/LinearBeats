using System.Collections.Generic;
using JetBrains.Annotations;
using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Visuals
{
    public class RailBehaviour : MonoBehaviour
    {
#pragma warning disable IDE0044
        [SerializeField]
        private Rigidbody _rigidbody;
        private float _noteDisappearOffset = 0f;
#pragma warning restore IDE0044


        public FixedTime StartTime { get; set; }
        public FixedTime Duration { get; set; }
        public IDictionary<ScrollEvent, bool> IgnoreOptions { get; set; }

        public void UpdateRailPosition(IPositionConverter positionConverter, FixedTime currentTime,
            float meterPerQuarterNote,
            float bpmMultiplier)
        {
            //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)

            if (currentTime.Second - _noteDisappearOffset >= StartTime)
            {
                SetZPosition(-100f);
            }
            else
            {
                var start = positionConverter.Convert(StartTime, IgnoreOptions);
                var current = positionConverter.Convert(currentTime, IgnoreOptions);

                var positionInMeter = meterPerQuarterNote * (start - current);
                SetZPosition(positionInMeter * bpmMultiplier);


                if (Duration.Pulse == 0) return;

                var scale = transform.localScale;
                var end = positionConverter.Convert(StartTime + Duration, IgnoreOptions);

                var dur = end - start;

                transform.localScale = new Vector3(scale.x, scale.y, meterPerQuarterNote * dur);
            }
        }

        private void SetZPosition(float zPosition)
        {
            var position = _rigidbody.position;
            _rigidbody.MovePosition(new Vector3(position.x, position.y, zPosition));
        }
    }
}
