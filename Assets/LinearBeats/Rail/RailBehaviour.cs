using JetBrains.Annotations;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Rail
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class RailBehaviour : SerializedMonoBehaviour
    {
        [ShowInInspector, ReadOnly] [CanBeNull] public RailObject RailObject { get; set; }
        [ShowInInspector, ReadOnly] protected abstract bool RailDisabled { get; }
        [ShowInInspector, ReadOnly] protected abstract Vector3 Position { get; }
        [ShowInInspector, ReadOnly] protected abstract Vector3 Scale { get; }
        [ShowInInspector, ReadOnly] private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _rigidbody.MovePosition(Position);
            transform.localScale = Scale;
            if (RailDisabled) LeanPool.Despawn(this);
        }
    }
}
