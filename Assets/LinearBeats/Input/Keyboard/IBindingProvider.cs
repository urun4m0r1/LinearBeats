using UnityEngine;

namespace LinearBeats.Input.Keyboard
{
    public interface IBindingProvider
    {
        KeyCode GetBinding(KeyType key);
        KeyCode GetBindingAlternative(KeyType key);
    }
}
