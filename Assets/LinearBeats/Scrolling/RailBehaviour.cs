using JetBrains.Annotations;
using Lean.Pool;
using UnityEngine;

namespace LinearBeats.Scrolling
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class RailBehaviour : MonoBehaviour
    {
        [CanBeNull] public RailObject RailObject { get; set; }
        protected abstract bool UpdateRailDisabled { get; }
        protected abstract Vector3 Position { get; }
        protected abstract Vector3 Scale { get; }

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _rigidbody.MovePosition(Position);
            transform.localScale = Scale;
            if (UpdateRailDisabled) LeanPool.Despawn(this);
        }
    }
}
