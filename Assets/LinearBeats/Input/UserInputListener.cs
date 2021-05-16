using System;
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

        public override bool IsInputInvoked(byte row, byte col)
        {
            var keyCode = BindingProvider?.GetBinding(row, col) ?? KeyCode.None;
            var keyCodeAlternative = BindingProvider?.GetBindingAlternative(row, col) ?? KeyCode.None;
            return IsInvokedBy(keyCode) || IsInvokedBy(keyCodeAlternative);
        }

        public override bool IsSpecialInputInvoked()
        {
            var keyCode = BindingProvider?.GetBindingSpecial() ?? KeyCode.None;
            return IsInvokedBy(keyCode);
        }

        private bool IsInvokedBy(KeyCode keyCode) => _inputReceiver.GetInput(keyCode);
    }
}
