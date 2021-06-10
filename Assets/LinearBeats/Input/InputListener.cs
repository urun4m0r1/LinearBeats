using LinearBeats.Input.Keyboard;
using LinearBeats.Script;

namespace LinearBeats.Input
{
    public abstract class InputListener
    {
        public KeyType GetNoteInvoked(Shape noteShape, float progress) =>
            GetAnyInputInvokedIn(noteShape.Pos, noteShape.Dst, noteShape.Size ?? 1, progress);

        private KeyType GetAnyInputInvokedIn(KeyType pos, KeyType? dst, byte keySize, float progress)
        {
            var keyDist = (dst - pos) * progress;
            var keyStart = pos + (byte) (keyDist ?? 0);
            var keyEnd = keyStart + keySize;

            return AnyInputInvokedIn(keyStart, keyEnd);
        }

        private KeyType AnyInputInvokedIn(KeyType keyStart, KeyType keyEnd)
        {
            for (var i = keyStart; i < keyEnd; ++i)
                if (IsInputInvoked(i))
                    return i;

            return KeyType.None;
        }

        public abstract bool IsInputInvoked(KeyType key);
    }
}
