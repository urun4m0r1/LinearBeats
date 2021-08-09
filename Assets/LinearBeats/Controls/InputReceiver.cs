using System;
using JetBrains.Annotations;
using LinearBeats.Script;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Controls
{
    public sealed class InputReceiver
    {
        [ShowInInspector, ReadOnly] [NotNull] private readonly InputManager _inputManager;
        [ShowInInspector, ReadOnly] [NotNull] private readonly Func<KeyCode, bool> _inputFunction;

        public InputReceiver([NotNull] InputManager inputManager, [NotNull] Func<KeyCode, bool> inputFunction)
        {
            _inputManager = inputManager;
            _inputFunction = inputFunction;
        }

        public KeyType GetFirstKeyInvokedInNote(Shape noteShape, float? noteProgress)
        {
            var keyOffset = Mathf.RoundToInt((noteShape.Dst - noteShape.Pos) * noteProgress ?? 0);
            var keyStart = noteShape.Pos + (byte) keyOffset;
            var keyEnd = keyStart + (noteShape.Size ?? 1);

            return GetFirstKeyInvokedIn(keyStart, keyEnd);
        }

        private KeyType GetFirstKeyInvokedIn(KeyType keyStart, KeyType keyEnd)
        {
            for (var i = keyStart; i < keyEnd; ++i)
                if (GetBindingOrAlternative(i))
                    return i;

            return KeyType.None;
        }

        public bool GetBindingOrAlternative(KeyType key)
        {
            var bindings = _inputManager.Bindings;

            if (!bindings) return false;

            var keyCode = bindings.GetBinding(key);
            var keyCodeAlternative = bindings.GetBindingAlternative(key);
            return _inputFunction(keyCode) || _inputFunction(keyCodeAlternative);
        }
    }
}
