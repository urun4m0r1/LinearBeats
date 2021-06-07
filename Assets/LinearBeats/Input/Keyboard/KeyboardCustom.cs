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

        public void SetBindingSpecial(KeyCode keyCode) => BindingSpecial = keyCode;
        public void SetBinding(byte row, byte col, KeyCode keyCode) => BindingsLayout[row, col] = keyCode;
        public void SetBindingAlternative(byte row, byte col, KeyCode keyCode)
            => BindingsLayoutAlternative[row, col] = keyCode;
    }
}
