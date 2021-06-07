using LinearBeats.Input.Keyboard;
using LinearBeats.Script;

namespace LinearBeats.Input
{
    public static class InputHandler
    {
        private static readonly UserInputListener PressedListener = new UserInputListener(new PressedReceiver());
        private static readonly UserInputListener HoldingListener = new UserInputListener(new HoldingReceiver());

        public static bool IsHolding(KeyType key) => HoldingListener.IsInputInvoked(key);
        public static bool IsNotePressed(Shape noteShape) => PressedListener.GetNoteInvoked(noteShape) != KeyType.None;
    }
}
