//TODO: Implement AutoInputListener

namespace LinearBeats.Input
{
    public sealed class AutoInputListener : InputListener
    {
        public override bool IsInputInvoked(byte row, byte col)
        {
            return true;
        }

        public override bool IsSpecialInputInvoked()
        {
            return true;
        }
    }
}
