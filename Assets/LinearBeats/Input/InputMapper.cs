using LinearBeats.Input.Keyboard;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Input
{
    //TODO: Implement InputMapper
    public sealed class InputMapper : SerializedMonoBehaviour
    {
        [Required]
        [OdinSerialize]
        public Keyboard.Keyboard CurrentKeyboard
        {
            get => UserInputListener.BindingProvider as Keyboard.Keyboard;
            set => UserInputListener.BindingProvider = value;
        }

        [Button]
        public void SetCustomBinding(KeyType key, KeyCode keyCode)
        {
            if (IsBindingReceiver()) GetBindingReceiver().SetBinding(key, keyCode);
        }

        [Button]
        public void SetCustomBindingAlternative(KeyType key, KeyCode keyCode)
        {
            if (IsBindingReceiver()) GetBindingReceiver().SetBindingAlternative(key, keyCode);
        }

        private bool IsBindingReceiver() => CurrentKeyboard is IBindingReceiver;
        private IBindingReceiver GetBindingReceiver() => CurrentKeyboard as IBindingReceiver;
    }
}
