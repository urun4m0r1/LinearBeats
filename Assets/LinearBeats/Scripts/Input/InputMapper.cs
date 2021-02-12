#pragma warning disable IDE0051

using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Input
{
    public sealed class InputMapper : SerializedMonoBehaviour
    {
#pragma warning disable IDE0044
        [Required]
        [OdinSerialize]
        public Keyboard CurrentKeyboard
        {
            get => UserInputListener.BindingProvider as Keyboard;
            set => UserInputListener.BindingProvider = value;
        }
#pragma warning restore IDE0044

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

        private bool IsBindingReceiver()
        {
            return CurrentKeyboard is IBindingReceiver;
        }

        private IBindingReceiver GetBindingReceiver()
        {
            return CurrentKeyboard as IBindingReceiver;
        }
    }
}
