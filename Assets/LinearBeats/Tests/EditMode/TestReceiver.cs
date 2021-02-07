using LinearBeats.Input;
using UnityEngine;

public sealed class TestReceiver : IInputReceiver
{
    private KeyCode invokedKeyCode = KeyCode.None;

    public void Invoke(KeyCode keyCode)
    {
        invokedKeyCode = keyCode;
    }

    bool IInputReceiver.GetInput(KeyCode keyCode)
    {
        return invokedKeyCode == keyCode;
    }
}
