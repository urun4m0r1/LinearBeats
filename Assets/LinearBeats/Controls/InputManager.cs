using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace LinearBeats.Controls
{
    public sealed class InputManager : SerializedMonoBehaviour
    {
        //TODO: 키 변경 설정 기능 추가
        [OdinSerialize] [CanBeNull] public Bindings Bindings { get; set; }
    }
}
