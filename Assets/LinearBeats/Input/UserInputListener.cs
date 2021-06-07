using System;
using LinearBeats.Input.Keyboard;
using UnityEngine;

namespace LinearBeats.Input
{
    public sealed class UserInputListener : InputListener
    {
        public static IBindingProvider BindingProvider { get; set; }

        private readonly IInputReceiver _inputReceiver;

        public UserInputListener(IInputReceiver inputReceiver)
        {
            _inputReceiver = inputReceiver ?? throw new ArgumentNullException();
        }

        public override bool IsInputInvoked(KeyType key)
        {
            var keyCode = BindingProvider?.GetBinding(key) ?? KeyCode.None;
            var keyCodeAlternative = BindingProvider?.GetBindingAlternative(key) ?? KeyCode.None;
            return IsInvokedBy(keyCode) || IsInvokedBy(keyCodeAlternative);
        }

        private bool IsInvokedBy(KeyCode keyCode) => _inputReceiver.GetInput(keyCode);
    }
}
