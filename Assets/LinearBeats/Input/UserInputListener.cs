using JetBrains.Annotations;
using LinearBeats.Input.Keyboard;
using UnityEngine;

namespace LinearBeats.Input
{
    public sealed class UserInputListener : InputListener
    {
        [CanBeNull] public static IBindingProvider BindingProvider { get; set; }

        [NotNull] private readonly IInputReceiver _inputReceiver;

        public UserInputListener([NotNull] IInputReceiver inputReceiver) => _inputReceiver = inputReceiver;

        public override bool IsInputInvoked(KeyType key)
        {
            var keyCode = BindingProvider?.GetBinding(key) ?? KeyCode.None;
            var keyCodeAlternative = BindingProvider?.GetBindingAlternative(key) ?? KeyCode.None;
            return IsInvokedBy(keyCode) || IsInvokedBy(keyCodeAlternative);
        }

        private bool IsInvokedBy(KeyCode keyCode) => _inputReceiver.GetInput(keyCode);
    }
}
