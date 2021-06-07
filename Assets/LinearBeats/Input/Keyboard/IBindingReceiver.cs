using UnityEngine;

namespace LinearBeats.Input.Keyboard
{
    public interface IBindingReceiver
    {
        void SetBindingSpecial(KeyCode keyCode);
        void SetBinding(byte row, byte col, KeyCode keyCode);
        void SetBindingAlternative(byte row, byte col, KeyCode keyCode);
    }
}
