//TODO: Implement NetworkInputListener

namespace LinearBeats.Input
{
    public sealed class NetworkInputListener : InputListener
    {
        public override bool IsInputInvoked(byte row, byte col)
        {
            return false;
        }

        public override bool IsSpecialInputInvoked()
        {
            return false;
        }
    }
}
