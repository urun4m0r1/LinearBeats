using System;
using UnityEngine;
using Utils.Extensions;

namespace Utils.Unity
{
    public sealed class ComponentInitializer
    {
        private readonly GameObject _gameObject = null;

        public ComponentInitializer(GameObject gameObject)
        {
            _gameObject = gameObject ?? throw new ArgumentNullException();
        }

        public void TryInitComponent<T>(ref T component) where T : Component
        {
            if (component.IsNullOrEmptyRefType() && !_gameObject.TryGetComponent(out component))
            {
                component = _gameObject.AddComponent<T>();
            }
        }
    }
}
