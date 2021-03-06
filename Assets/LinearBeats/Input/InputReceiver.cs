using UnityEngine;
using static UnityEngine.Input;

namespace LinearBeats.Input
{
    public interface IInputReceiver
    {
        bool GetInput(KeyCode keyCode);
    }

    public sealed class PressedReceiver : IInputReceiver
    {
        bool IInputReceiver.GetInput(KeyCode keyCode) => GetKeyDown(keyCode);
    }

    public sealed class ReleasedReceiver : IInputReceiver
    {
        bool IInputReceiver.GetInput(KeyCode keyCode) => GetKeyUp(keyCode);
    }

    public sealed class HoldingReceiver : IInputReceiver
    {
        bool IInputReceiver.GetInput(KeyCode keyCode) => GetKey(keyCode);
    }
}
