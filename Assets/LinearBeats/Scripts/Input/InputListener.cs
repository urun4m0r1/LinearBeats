using LinearBeats.Script;
using System;
using UnityEngine;

namespace LinearBeats.Input
{
    public sealed class PressedListener : InputListener
    {
        public static readonly InputListener Instance = new InputListener(new PressedReceiver());
    }
    public sealed class ReleasedListener : InputListener
    {
        public static readonly InputListener Instance = new InputListener(new ReleasedReceiver());
    }
    public sealed class HoldingListener : InputListener
    {
        public static readonly InputListener Instance = new InputListener(new HoldingReceiver());
    }

    public class InputListener
    {
        public static IBindingProvider BindingProvider { get; set; } = null;

        private readonly IInputReceiver _inputReceiver = null;

        protected InputListener() { }

        public InputListener(IInputReceiver inputReceiver)
        {
            _inputReceiver = inputReceiver ?? throw new ArgumentNullException();
        }

        public bool IsSpecialBindingInvoked()
        {
            KeyCode keyCode = BindingProvider?.GetBindingSpecial() ?? KeyCode.None;
            return _inputReceiver.GetInput(keyCode);
        }

        public InputPosition GetNoteInvoked(Note note)
        {
            return GetAnyBindingInvokedIn(note.PositionRow, note.PositionCol, note.SizeRow, note.SizeCol);
        }

        private InputPosition GetAnyBindingInvokedIn(byte rowStart, byte colStart, byte rowSize, byte colSize)
        {
            byte rowEnd = (byte)(rowStart + rowSize);
            byte colEnd = (byte)(colStart + colSize);

            for (var row = rowStart; row < rowEnd; ++row)
            {
                for (var col = colStart; col < colEnd; ++col)
                {
                    if (IsBindingInvoked(row, col))
                    {
                        return new InputPosition(row, col);
                    }
                }
            }
            return new InputPosition(null, null);
        }

        public bool IsBindingInvoked(byte row, byte col)
        {
            return IsBinnedBindingInvoked(row, col) || IsAlternativeBindingInvoked(row, col);
        }

        private bool IsBinnedBindingInvoked(byte row, byte col)
        {
            KeyCode keyCode = BindingProvider?.GetBinding(row, col) ?? KeyCode.None;
            return _inputReceiver.GetInput(keyCode);
        }

        private bool IsAlternativeBindingInvoked(byte row, byte col)
        {
            KeyCode keyCode = BindingProvider?.GetBindingAlternative(row, col) ?? KeyCode.None;
            return _inputReceiver.GetInput(keyCode);
        }
    }
}
