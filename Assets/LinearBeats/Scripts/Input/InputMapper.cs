using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Input
{
    public sealed class InputMapper : MonoBehaviour
    {

        [Button]
        public void SetCustomBindingSpecial(KeyCode keyCode)
        {
            if (IsBindingReceiver())
            {
                GetBindingReceiver().SetBindingSpecial(keyCode);
            }
        }

        [Button]
        public void SetCustomBinding(byte row, byte col, KeyCode keyCode)
        {
            if (IsBindingReceiver())
            {
                GetBindingReceiver().SetBinding(row, col, keyCode);
            }
        }

        [Button]
        public void SetCustomBindingAlternative(byte row, byte col, KeyCode keyCode)
        {
            if (IsBindingReceiver())
            {
                GetBindingReceiver().SetBindingAlternative(row, col, keyCode);
            }
        }

        private static bool IsBindingReceiver()
        {
            return InputListener.BindingProvider is IBindingReceiver;
        }

        private static IBindingReceiver GetBindingReceiver()
        {
            return InputListener.BindingProvider as IBindingReceiver;
        }
    }
}
