using UnityEngine;

namespace LinearBeats.Input
{
    [CreateAssetMenu(menuName = "LinearBeats/KeyboardCustom")]
    public sealed class KeyboardCustom : Keyboard, IBindingReceiver
    {
        void IBindingReceiver.SetBindingSpecial(KeyCode keyCode) => _bindingSpecial = keyCode;
        void IBindingReceiver.SetBinding(byte row, byte col, KeyCode keyCode) => _bindingsLayout[row, col] = keyCode;
        void IBindingReceiver.SetBindingAlternative(byte row, byte col, KeyCode keyCode)
            => _bindingsLayoutAlternative[row, col] = keyCode;
    }
}
