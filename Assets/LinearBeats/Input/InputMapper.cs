//TODO: Implement InputMapper

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
        public Keyboard.Keyboard CurrentKeyboard
        {
            get => UserInputListener.BindingProvider as Keyboard.Keyboard;
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

        private bool IsBindingReceiver() => CurrentKeyboard is IBindingReceiver;
        private IBindingReceiver GetBindingReceiver() => CurrentKeyboard as IBindingReceiver;
    }
}
