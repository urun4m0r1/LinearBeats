#pragma warning disable IDE0090
#pragma warning disable IDE0051

using LinearBeats.Script;

namespace LinearBeats.Input
{

    public static class InputHandler
    {
        private static readonly UserInputListener s_pressedListener = new UserInputListener(new PressedReceiver());
        private static readonly UserInputListener s_holdingListener = new UserInputListener(new HoldingReceiver());

        public static bool IsHolding(byte row, byte col)
        {
            return s_holdingListener.IsInputInvoked(row, col);
        }

        public static bool IsNotePressed(Note note)
        {
            return s_pressedListener.GetNoteInvoked(note).Exist;
        }
    }
}
