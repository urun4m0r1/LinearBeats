using UnityEngine;

namespace LinearBeats.Input.Keyboard
{
    public interface IBindingProvider
    {
        KeyCode GetBindingSpecial();
        KeyCode GetBinding(byte row, byte col);
        KeyCode GetBindingAlternative(byte row, byte col);
    }
}
