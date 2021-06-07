using LinearBeats.Input.Keyboard;
using LinearBeats.Script;

namespace LinearBeats.Input
{
    public abstract class InputListener
    {
        public KeyType GetNoteInvoked(Shape noteShape) => GetAnyInputInvokedIn(noteShape.Pos, noteShape.Size ?? 1);

        private KeyType GetAnyInputInvokedIn(KeyType key, byte keySize)
        {
            var keyEnd = key + keySize;

            for (var i = key; i < keyEnd; ++i)
                if (IsInputInvoked(i)) return i;

            return KeyType.None;
        }

        public abstract bool IsInputInvoked(KeyType key);
    }
}
