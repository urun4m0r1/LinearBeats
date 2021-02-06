using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Input
{
    public sealed class InputMapper : MonoBehaviour
    {

        [Button]
        public void SetCustomBindingSpecial(KeyCode keyCode)
        {
            if (IsKeyboardCustom())
            {
                GetKeyboardCustom().SetBindingSpecial(keyCode);
            }
        }

        [Button]
        public void SetCustomBinding(byte row, byte col, KeyCode keyCode)
        {
            if (IsKeyboardCustom())
            {
                GetKeyboardCustom().SetBinding(row, col, keyCode);
            }
        }

        [Button]
        public void SetCustomBindingAlternative(byte row, byte col, KeyCode keyCode)
        {
            if (IsKeyboardCustom())
            {
                GetKeyboardCustom().SetBindingAlternative(row, col, keyCode);
            }
        }

        private bool IsKeyboardCustom()
        {
            return InputListener.BindingProvider is KeyboardCustom;
        }

        private KeyboardCustom GetKeyboardCustom()
        {
            return InputListener.BindingProvider as KeyboardCustom;
        }
    }
}
