using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Input.Keyboard
{
    /// <summary>
    /// In general keyboards (ANSI, ISO, JIS, etc.), there are up to 14 keys per bottom and middle line.
    /// However, there are at least 12 keys in the lower row (Z, X, C, V, ...), which is the standard for gameplay.
    /// Therefore, there are 12 lanes for receiving keyboard input during gameplay.
    /// As a result, up to 14 inputs must be processed in one lane considering duplicate key input.
    /// This can be done by checking the alternate input on the keystroke.
    /// </summary>
    [CreateAssetMenu(menuName = "LinearBeats/Keyboard")]
    public class Keyboard : SerializedScriptableObject, IBindingProvider
    {
        [ShowInInspector, ReadOnly] public static readonly byte Length = 12;

        [OdinSerialize, DisableContextMenu]
        [NotNull] public virtual string Name { get; protected set; } = "";

        [OdinSerialize, DisableContextMenu]
        [NotNull] public virtual string Description { get; protected set; } = "";

        [OdinSerialize, DisableContextMenu, ListDrawerSettings(IsReadOnly = true), Title("Bindings"), PropertyOrder(1)]
        [NotNull] protected KeyCode[] BindingsLayout =
        {
            KeyCode.Space,
            KeyCode.LeftShift,
            KeyCode.Z,
            KeyCode.X,
            KeyCode.C,
            KeyCode.V,
            KeyCode.B,
            KeyCode.N,
            KeyCode.M,
            KeyCode.Comma,
            KeyCode.Period,
            KeyCode.Slash,
            KeyCode.RightShift,
            KeyCode.LeftAlt,
            KeyCode.RightAlt,
        };

        [OdinSerialize, DisableContextMenu, ListDrawerSettings(IsReadOnly = true), Title("Bindings Alternative"), PropertyOrder(2)]
        [NotNull] protected KeyCode[] BindingsLayoutAlternative =
        {
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
            KeyCode.None,
        };

        public KeyCode GetBinding(KeyType key) => BindingsLayout[(int) key];
        public KeyCode GetBindingAlternative(KeyType key) => BindingsLayoutAlternative[(int) key];
    }
}
