using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Input.Keyboard
{
    [CreateAssetMenu(menuName = "LinearBeats/KeyboardCustom")]
    public sealed class KeyboardCustom : Keyboard, IBindingReceiver
    {
        [ShowInInspector, ReadOnly]
        public override string Name { get; protected set; } = "Custom";

        [ShowInInspector, ReadOnly]
        public override string Description { get; protected set; } = "Custom keyboard bindings";

        public void SetBinding(KeyType key, KeyCode keyCode) => BindingsLayout[(int) key] = keyCode;
        public void SetBindingAlternative(KeyType key, KeyCode keyCode) => BindingsLayoutAlternative[(int) key] = keyCode;
    }
}
