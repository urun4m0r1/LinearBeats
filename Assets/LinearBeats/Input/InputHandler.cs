using LinearBeats.Input.Keyboard;
using LinearBeats.Script;

namespace LinearBeats.Input
{
    public static class InputHandler
    {
        private static readonly UserInputListener PressedListener = new UserInputListener(new PressedReceiver());
        private static readonly UserInputListener HoldingListener = new UserInputListener(new HoldingReceiver());

        public static bool IsHolding(KeyType key) => HoldingListener.IsKeyInvoked(key);
        public static bool IsNotePressed(Shape noteShape, float? noteProgress) =>
            PressedListener.GetFirstKeyInvokedInNote(noteShape, noteProgress) != KeyType.None;
    }
}
