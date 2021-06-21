using System;
using JetBrains.Annotations;
using LinearBeats.Script;
using UnityEngine;

namespace LinearBeats.Keyboard
{
    public sealed class InputReceiver
    {
        [NotNull] private readonly InputManager _inputManager;
        [NotNull] private readonly Func<KeyCode, bool> _inputFunction;

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
            var keyboard = _inputManager.Keyboard;

            if (!keyboard) return false;

            var keyCode = keyboard.GetBinding(key);
            var keyCodeAlternative = keyboard.GetBindingAlternative(key);
            return _inputFunction(keyCode) || _inputFunction(keyCodeAlternative);
        }
    }
}
