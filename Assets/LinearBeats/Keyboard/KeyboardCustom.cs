using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Keyboard
{
    [CreateAssetMenu(menuName = "LinearBeats/KeyboardCustom")]
    public sealed class KeyboardCustom : Keyboard
    {
        [ShowInInspector, ReadOnly]
        public override string Name { get; protected set; } = "Custom";

        [ShowInInspector, ReadOnly]
        public override string Description { get; protected set; } = "Custom keyboard bindings";

        public void SetBinding(KeyType key, KeyCode keyCode) => BindingsLayout[key] = keyCode;
        public void SetBindingAlternative(KeyType key, KeyCode keyCode) => BindingsLayoutAlternative[key] = keyCode;
    }
}
