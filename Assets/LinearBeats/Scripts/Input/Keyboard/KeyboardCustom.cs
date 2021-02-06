using UnityEngine;

namespace LinearBeats.Input
{
    [CreateAssetMenu(menuName = "LinearBeats/KeyboardCustom")]
    public sealed class KeyboardCustom : Keyboard
    {
        public void SetBindingSpecial(KeyCode keyCode)
        {
            _bindingSpecial = keyCode;
        }

        public void SetBinding(byte row, byte col, KeyCode keyCode)
        {
            _bindingsLayout[row, col] = keyCode;
        }

        public void SetBindingAlternative(byte row, byte col, KeyCode keyCode)
        {
            _bindingsLayoutAlternative[row, col] = keyCode;
        }
    }
}
