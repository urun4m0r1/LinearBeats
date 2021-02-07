using System;
using LinearBeats.Input;
using LinearBeats.Script;
using NUnit.Framework;
using UnityEngine;

public class InputTest
{
    public sealed class TestReceiver : IInputReceiver
    {
        public KeyCode MatchingKeyCode { get; set; } = KeyCode.None;

        bool IInputReceiver.GetInput(KeyCode keyCode)
        {
            return MatchingKeyCode == keyCode;
        }
    }

    [Test]
    public void InputPosition_Will_Throw_Exception_When_Input_Is_Null()
    {
        var input = new InputPosition(null, null);
        Assert.That(() => input.Row, Throws.Exception);
        Assert.That(() => input.Col, Throws.Exception);
        Assert.IsFalse(input.Exist);

        input = new InputPosition(10, null);
        Assert.AreEqual(10, input.Row);
        Assert.That(() => input.Col, Throws.Exception);
        Assert.IsFalse(input.Exist);

        input = new InputPosition(null, 20);
        Assert.That(() => input.Row, Throws.Exception);
        Assert.AreEqual(20, input.Col);
        Assert.IsFalse(input.Exist);

        input = new InputPosition(10, 20);
        Assert.AreEqual(10, input.Row);
        Assert.AreEqual(20, input.Col);
        Assert.IsTrue(input.Exist);
    }

    [Test]
    public void InputListener_Will_Work()
    {
        InputListener.BindingProvider = ScriptableObject.CreateInstance<KeyboardCustom>();

        var bindingReceiver = InputListener.BindingProvider as IBindingReceiver;
        bindingReceiver.SetBinding(0, 0, KeyCode.A);
        bindingReceiver.SetBindingAlternative(0, 0, KeyCode.B);
        bindingReceiver.SetBindingSpecial(KeyCode.C);

        var receiver = new TestReceiver();
        var listener = new InputListener(receiver);

        receiver.MatchingKeyCode = KeyCode.A;
        Assert.IsTrue(listener.IsBindingInvoked(0, 0));
        receiver.MatchingKeyCode = KeyCode.B;
        Assert.IsTrue(listener.IsBindingInvoked(0, 0));
        receiver.MatchingKeyCode = KeyCode.C;
        Assert.IsTrue(listener.IsSpecialBindingInvoked());

        receiver.MatchingKeyCode = KeyCode.Z;
        Assert.IsFalse(listener.IsBindingInvoked(0, 0));
        receiver.MatchingKeyCode = KeyCode.Z;
        Assert.IsFalse(listener.IsSpecialBindingInvoked());
    }

    [Test]
    public void InputListener_GetNote_Will_Work()
    {
        InputListener.BindingProvider = ScriptableObject.CreateInstance<KeyboardCustom>();

        var bindingReceiver = InputListener.BindingProvider as IBindingReceiver;
        bindingReceiver.SetBinding(0, 0, KeyCode.A);
        bindingReceiver.SetBinding(0, 1, KeyCode.B);
        bindingReceiver.SetBinding(0, 2, KeyCode.C);
        bindingReceiver.SetBinding(0, 3, KeyCode.D);
        bindingReceiver.SetBindingAlternative(0, 0, KeyCode.E);
        bindingReceiver.SetBindingAlternative(0, 1, KeyCode.F);
        bindingReceiver.SetBindingAlternative(0, 2, KeyCode.G);
        bindingReceiver.SetBindingAlternative(0, 3, KeyCode.H);

        bindingReceiver.SetBinding(1, 1, KeyCode.I);
        bindingReceiver.SetBindingAlternative(1, 1, KeyCode.J);

        var receiver = new TestReceiver();
        var listener = new InputListener(receiver);

        var note = new Note
        {
            PositionRow = 0,
            PositionCol = 1,
            SizeRow = 1,
            SizeCol = 2,
        };

        var input = new InputPosition(0, 1);
        receiver.MatchingKeyCode = KeyCode.B;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));
        receiver.MatchingKeyCode = KeyCode.F;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));

        input = new InputPosition(0, 2);
        receiver.MatchingKeyCode = KeyCode.C;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));
        receiver.MatchingKeyCode = KeyCode.G;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));

        input = new InputPosition(null, null);
        receiver.MatchingKeyCode = KeyCode.A;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));
        receiver.MatchingKeyCode = KeyCode.E;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));
        receiver.MatchingKeyCode = KeyCode.D;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));
        receiver.MatchingKeyCode = KeyCode.H;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));
        receiver.MatchingKeyCode = KeyCode.I;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));
        receiver.MatchingKeyCode = KeyCode.J;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));
        receiver.MatchingKeyCode = KeyCode.Z;
        Assert.AreEqual(input, listener.GetNoteInvoked(note));
    }
}
