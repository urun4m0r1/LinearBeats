using UnityEngine;

namespace LinearBeats.Input.Keyboard
{
    public interface IBindingReceiver
    {
        void SetBinding(KeyType key, KeyCode keyCode);
        void SetBindingAlternative(KeyType key, KeyCode keyCode);
    }
}
