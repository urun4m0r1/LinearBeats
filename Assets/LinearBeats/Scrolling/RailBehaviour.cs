using System;
using JetBrains.Annotations;
using Lean.Pool;
using UnityEngine;

namespace LinearBeats.Scrolling
{
    [RequireComponent(typeof(Rigidbody))]
    public class RailBehaviour : MonoBehaviour
    {
        [CanBeNull] public RailObject RailObject { get; set; }
        protected virtual Vector3 Position => throw new InvalidOperationException();
        protected virtual Vector3 Scale => throw new InvalidOperationException();

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _rigidbody.MovePosition(Position);
            transform.localScale = Scale;

            if (RailObject?.Disabled ?? false) LeanPool.Despawn(this);
        }
    }
}
