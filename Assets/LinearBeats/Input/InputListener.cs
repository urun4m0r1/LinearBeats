using LinearBeats.Input.Keyboard;
using LinearBeats.Script;
using UnityEngine;

namespace LinearBeats.Input
{
    public abstract class InputListener
    {
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
                if (IsKeyInvoked(i))
                    return i;

            return KeyType.None;
        }

        public abstract bool IsKeyInvoked(KeyType key);
    }
}
