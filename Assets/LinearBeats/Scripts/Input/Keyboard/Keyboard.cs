/// In general keyboards (ANSI, ISO, JIS, etc.), there are up to 14 keys per bottom and middle line.
/// However, there are at least 12 keys in the lower row (ZXCV...), which is the standard for gameplay.
/// Therefore, there are 12 lanes for receiving keyboard input during gameplay.
/// As a result, up to 14 inputs must be processed in one lane considering duplicate key input.
/// This can be done by checking the alternate input on the keystroke.

using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LinearBeats.Input
{

    public struct Bindings
    {
        public KeyCode[,] Layout;
    }

    [CreateAssetMenu(menuName = "LinearBeats/Keyboard")]
    public class Keyboard : SerializedScriptableObject, IBindingProvider
    {
        public Bindings bindings;


        public const byte Rows = 2;
        public const byte Cols = 12;

        [Required]
        [PropertyOrder(-2)]
        [Title("Keyboard Information")]
        public string Name = "ANSI";

        [Required]
        [PropertyOrder(-1)]
        [MultiLineProperty]
        public string Description = "Standard US Keyboard Layout 101/104";

        [Title("Bindings")]
        [SerializeField]
        protected KeyCode _bindingSpecial = KeyCode.Space;

        [OdinSerialize]
        protected KeyCode[,] _bindingsLayout = new KeyCode[Rows, Cols]
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

        [Title("Bindings Alternative")]
        [OdinSerialize]
        protected KeyCode[,] _bindingsLayoutAlternative = new KeyCode[Rows, Cols]
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

        KeyCode IBindingProvider.GetBindingSpecial()
        {
            return _bindingSpecial;
        }

        KeyCode IBindingProvider.GetBinding(byte row, byte col)
        {
            return _bindingsLayout[row, col];
        }

        KeyCode IBindingProvider.GetBindingAlternative(byte row, byte col)
        {
            return _bindingsLayoutAlternative[row, col];
        }
    }
}
