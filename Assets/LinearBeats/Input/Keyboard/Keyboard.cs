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
        [ShowInInspector, ReadOnly] public const byte Rows = 2;
        [ShowInInspector, ReadOnly] public const byte Cols = 12;

        [OdinSerialize, DisableContextMenu] [NotNull]
        public virtual string Name { get; protected set; } = "";

        [OdinSerialize, DisableContextMenu] [NotNull]
        public virtual string Description { get; protected set; } = "";

        [OdinSerialize] protected KeyCode BindingSpecial = KeyCode.Space;

        //TODO: 오딘 선택창 나오게 하기
        [OdinSerialize, DisableContextMenu, TableMatrix(IsReadOnly = true), Title("Bindings"), PropertyOrder(1)]
        protected KeyCode[,] BindingsLayout =
        {
            {
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
            },
            {
                KeyCode.A,
                KeyCode.S,
                KeyCode.D,
                KeyCode.F,
                KeyCode.G,
                KeyCode.H,
                KeyCode.J,
                KeyCode.K,
                KeyCode.L,
                KeyCode.Semicolon,
                KeyCode.Quote,
                KeyCode.Return,
            },
        };

        [OdinSerialize, DisableContextMenu, TableMatrix(IsReadOnly = true), Title("Bindings Alternative"), PropertyOrder(2)]
        protected KeyCode[,] BindingsLayoutAlternative =
        {
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
            },
            {
                KeyCode.CapsLock,
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
            },
        };

        public KeyCode GetBindingSpecial() => BindingSpecial;
        public KeyCode GetBinding(byte row, byte col) => BindingsLayout[row, col];
        public KeyCode GetBindingAlternative(byte row, byte col) => BindingsLayoutAlternative[row, col];
    }
}
