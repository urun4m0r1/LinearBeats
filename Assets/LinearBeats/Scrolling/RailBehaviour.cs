using JetBrains.Annotations;
using Lean.Pool;
using UnityEngine;

namespace LinearBeats.Scrolling
{
    [RequireComponent(typeof(Rigidbody))]
    public class RailBehaviour : MonoBehaviour
    {
        [CanBeNull] public RailObject RailObject { get; set; }

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

        private void FixedUpdate()
        {
            UpdateLifecycle();
        }

        private void UpdateRailPosition()
        {
            if (RailObject == null) return;

            var position = GetPosition(RailObject);
            _rigidbody.MovePosition(position);
        }

        private void UpdateRailScale()
        {
            if (RailObject == null) return;

            var scale = GetScale(RailObject);
            transform.localScale = scale;
        }

        protected virtual void UpdateLifecycle()
        {
            if (RailObject == null) return;

            if (RailObject.CurrentTime >= RailObject.StartTime) LeanPool.Despawn(this);
        }

        protected virtual Vector3 GetPosition([NotNull] RailObject railObject) =>
            new Vector3(0f, 0f, railObject.StartPosition);

        protected virtual Vector3 GetScale([NotNull] RailObject railObject) => Vector3.one;
    }
}
