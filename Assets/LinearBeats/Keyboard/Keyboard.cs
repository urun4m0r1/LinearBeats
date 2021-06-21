using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Keyboard
{
    /// <summary>
    /// In general keyboards (ANSI, ISO, JIS, etc.), there are up to 14 keys per bottom and middle line.
    /// However, there are at least 12 keys in the lower row (Z, X, C, V, ...), which is the standard for gameplay.
    /// Therefore, there are 12 lanes for receiving keyboard input during gameplay.
    /// As a result, up to 14 inputs must be processed in one lane considering duplicate key input.
    /// This can be done by checking the alternate input on the keystroke.
    /// </summary>
    [CreateAssetMenu(menuName = "LinearBeats/Keyboard")]
    public class Keyboard : SerializedScriptableObject
    {
        [ShowInInspector, ReadOnly] public static readonly byte Length = 12;

        [OdinSerialize, DisableContextMenu]
        [NotNull] public virtual string Name { get; protected set; } = "";

        [OdinSerialize, DisableContextMenu]
        [NotNull] public virtual string Description { get; protected set; } = "";

        [OdinSerialize, DisableContextMenu, DictionaryDrawerSettings(IsReadOnly = true)]
        [NotNull] protected Dictionary<KeyType, KeyCode> BindingsLayout = new Dictionary<KeyType, KeyCode>
        {
            [KeyType.LShift] = KeyCode.LeftShift,
            [KeyType.Z] = KeyCode.Z,
            [KeyType.X] = KeyCode.X,
            [KeyType.C] = KeyCode.C,
            [KeyType.V] = KeyCode.V,
            [KeyType.B] = KeyCode.B,
            [KeyType.N] = KeyCode.N,
            [KeyType.M] = KeyCode.M,
            [KeyType.Comma] = KeyCode.Comma,
            [KeyType.Period] = KeyCode.Period,
            [KeyType.Slash] = KeyCode.Slash,
            [KeyType.RShift] = KeyCode.RightShift,
            [KeyType.Space] = KeyCode.Space,
            [KeyType.LAlt] = KeyCode.LeftAlt,
            [KeyType.RAlt] = KeyCode.RightAlt,
        };

        [OdinSerialize, DisableContextMenu, DictionaryDrawerSettings(IsReadOnly = true)]
        [NotNull] protected Dictionary<KeyType, KeyCode> BindingsLayoutAlternative = new Dictionary<KeyType, KeyCode>
        {
            [KeyType.LShift] = KeyCode.None,
            [KeyType.Z] = KeyCode.None,
            [KeyType.X] = KeyCode.None,
            [KeyType.C] = KeyCode.None,
            [KeyType.V] = KeyCode.None,
            [KeyType.B] = KeyCode.None,
            [KeyType.N] = KeyCode.None,
            [KeyType.M] = KeyCode.None,
            [KeyType.Comma] = KeyCode.None,
            [KeyType.Period] = KeyCode.None,
            [KeyType.Slash] = KeyCode.None,
            [KeyType.RShift] = KeyCode.None,
            [KeyType.Space] = KeyCode.None,
            [KeyType.LAlt] = KeyCode.None,
            [KeyType.RAlt] = KeyCode.None,
        };

        public KeyCode GetBinding(KeyType key) => BindingsLayout[key];
        public KeyCode GetBindingAlternative(KeyType key) => BindingsLayoutAlternative[key];
    }
}
