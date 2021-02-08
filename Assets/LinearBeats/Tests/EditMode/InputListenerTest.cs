using LinearBeats.Input;
using LinearBeats.Script;
using NUnit.Framework;
using UnityEngine;

public class InputListenerTest
{
    private static IBindingReceiver GetBindingReceiver()
    {
        InputListener.BindingProvider = ScriptableObject.CreateInstance<KeyboardCustom>();
        return InputListener.BindingProvider as IBindingReceiver;
    }

    [Test]
    public void Can_Handle_Alternative_Input()
    {
        IBindingReceiver bindingReceiver = GetBindingReceiver();

        #region Given
        byte row = 1;
        byte col = 6;

        KeyCode keyCode = KeyCode.A;
        KeyCode keyCodeAlternative = KeyCode.B;

        bindingReceiver.SetBinding(row, col, keyCode);
        bindingReceiver.SetBindingAlternative(row, col, keyCodeAlternative);

        var receiver = new TestReceiver();
        var listener = new InputListener(receiver);
        #endregion

        #region When
        bool IsInvokedWithInput(KeyCode keyCode)
        {
            receiver.Invoke(keyCode);
            return listener.IsBindingInvoked(row, col);
        }
        #endregion

        #region Then
        Assert.IsTrue(IsInvokedWithInput(keyCode));
        Assert.IsTrue(IsInvokedWithInput(keyCodeAlternative));
        Assert.IsFalse(IsInvokedWithInput(KeyCode.None));
        #endregion

        InputListener.BindingProvider = null;
    }

    [Test]
    public void Can_Handle_Special_Input()
    {
        IBindingReceiver bindingReceiver = GetBindingReceiver();

        #region Given
        KeyCode keyCodeSpecial = KeyCode.A;

        bindingReceiver.SetBindingSpecial(keyCodeSpecial);

        var receiver = new TestReceiver();
        var listener = new InputListener(receiver);
        #endregion

        #region When
        bool IsSpecialInvokedWithInput(KeyCode keyCode)
        {
            receiver.Invoke(keyCode);
            return listener.IsSpecialBindingInvoked();
        }
        #endregion

        #region Then
        Assert.IsTrue(IsSpecialInvokedWithInput(keyCodeSpecial));
        Assert.IsFalse(IsSpecialInvokedWithInput(KeyCode.None));
        #endregion

        InputListener.BindingProvider = null;
    }

    private struct Boundary
    {
        public byte Row { get; private set; }
        public byte Col { get; private set; }
        public KeyCode Binding { get; private set; }

        public Boundary(byte row, byte col, KeyCode bind)
        {
            Row = row;
            Col = col;
            Binding = bind;
        }
    }

    [Test]
    public void Can_Handle_Input_With_Note_Size()
    {
        IBindingReceiver bindingReceiver = GetBindingReceiver();

        #region Given
        var RowIn = new Boundary(0, 2, KeyCode.A);
        var RowOut = new Boundary(1, 2, KeyCode.B);
        var ColMinLeft = new Boundary(0, 0, KeyCode.C);
        var ColMinRight = new Boundary(0, 1, KeyCode.D);
        var ColMaxLeft = new Boundary(0, 3, KeyCode.E);
        var ColMaxRight = new Boundary(0, 4, KeyCode.F);

        SetBoundariesBinding();

        void SetBoundariesBinding()
        {
            SetBoundaryBinding(RowIn);
            SetBoundaryBinding(RowOut);
            SetBoundaryBinding(ColMaxLeft);
            SetBoundaryBinding(ColMinRight);
            SetBoundaryBinding(ColMaxLeft);
            SetBoundaryBinding(ColMaxRight);

            void SetBoundaryBinding(Boundary boundary)
            {
                bindingReceiver.SetBinding(boundary.Row, boundary.Col, boundary.Binding);
            }
        }

        var note = new Note
        {
            PositionRow = RowIn.Row,
            PositionCol = ColMinRight.Col,
            SizeRow = (byte)(RowOut.Row - RowIn.Row),
            SizeCol = (byte)(ColMaxRight.Col - ColMinRight.Col),
        };

        var receiver = new TestReceiver();
        var listener = new InputListener(receiver);
        #endregion

        #region When
        bool IsBoundaryInvokedInNoteRange(Boundary boundary)
        {
            receiver.Invoke(boundary.Binding);
            return listener.GetNoteInvoked(note).Exist;
        }
        #endregion

        #region Then
        Assert.IsTrue(IsBoundaryInvokedInNoteRange(RowIn));
        Assert.IsFalse(IsBoundaryInvokedInNoteRange(RowOut));
        Assert.IsFalse(IsBoundaryInvokedInNoteRange(ColMinLeft));
        Assert.IsTrue(IsBoundaryInvokedInNoteRange(ColMinRight));
        Assert.IsTrue(IsBoundaryInvokedInNoteRange(ColMaxLeft));
        Assert.IsFalse(IsBoundaryInvokedInNoteRange(ColMaxRight));
        #endregion

        InputListener.BindingProvider = null;
    }
}
