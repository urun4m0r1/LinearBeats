using JetBrains.Annotations;
using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Scrolling
{
    [RequireComponent(typeof(Rigidbody))]
    public class RailBehaviour : MonoBehaviour
    {
        [CanBeNull] public RailObject? RailObject { private get; set; }
        [CanBeNull] public FixedTime? CurrentTime { private get; set; }

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            UpdateRailPosition();
            UpdateRailScale();
        }

        private void UpdateRailPosition()
        {
            if (RailObject == null || CurrentTime == null) return;

            var position = GetPosition((RailObject) RailObject, (FixedTime) CurrentTime);
            _rigidbody.MovePosition(position);
        }

        private void UpdateRailScale()
        {
            if (RailObject == null || CurrentTime == null) return;

            var scale = GetScale((RailObject) RailObject, (FixedTime) CurrentTime);
            transform.localScale = scale;
        }

        protected virtual Vector3 GetPosition(RailObject railObject, FixedTime currentTime)
        {
            var startPosition = railObject.GetStartPosition(currentTime);
            return new Vector3(0f, 0f, startPosition);
        }

        protected virtual Vector3 GetScale(RailObject railObject, FixedTime currentTime) => Vector3.one;
    }
}
