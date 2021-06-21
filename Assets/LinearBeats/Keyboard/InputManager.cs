using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace LinearBeats.Keyboard
{
    public sealed class InputManager : SerializedMonoBehaviour
    {
        [OdinSerialize] [CanBeNull] public Keyboard Keyboard { get; set; }
    }
}
