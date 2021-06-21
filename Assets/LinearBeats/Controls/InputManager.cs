using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace LinearBeats.Controls
{
    public sealed class InputManager : SerializedMonoBehaviour
    {
        [OdinSerialize] [CanBeNull] public Bindings Bindings { get; set; }
    }
}
