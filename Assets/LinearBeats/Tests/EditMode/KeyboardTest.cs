using LinearBeats.Input;
using NUnit.Framework;
using UnityEngine;

public class KeyboardTest
{
    [Test]
    public void Keyboard_Name_Description_Can_Be_Modified()
    {
        Keyboard keyboard = ScriptableObject.CreateInstance<Keyboard>();

        keyboard.Name = "Hello";
        keyboard.Description = "World";

        Assert.AreEqual("Hello", keyboard.Name);
        Assert.AreEqual("World", keyboard.Description);
    }

    [Test]
    public void Keyboard_IBindingProvider_Will_Work()
    {
        Keyboard keyboard = ScriptableObject.CreateInstance<Keyboard>();
        var receiver = keyboard as IBindingReceiver;
        var provider = keyboard as IBindingProvider;

        Assert.IsNull(receiver);

        Assert.AreEqual(KeyCode.Return, provider.GetBinding(1, 11));
        Assert.AreEqual(KeyCode.None, provider.GetBindingAlternative(0, 4));
        Assert.AreEqual(KeyCode.Space, provider.GetBindingSpecial());
    }

    [Test]
    public void Keyboard_IBindingReceiver_Will_Work()
    {
        KeyboardCustom keyboard = ScriptableObject.CreateInstance<KeyboardCustom>();
        var receiver = keyboard as IBindingReceiver;
        var provider = keyboard as IBindingProvider;

        receiver.SetBinding(0, 0, KeyCode.A);
        receiver.SetBindingAlternative(0, 0, KeyCode.B);
        receiver.SetBindingSpecial(KeyCode.C);

        Assert.AreEqual(KeyCode.A, provider.GetBinding(0, 0));
        Assert.AreEqual(KeyCode.B, provider.GetBindingAlternative(0, 0));
        Assert.AreEqual(KeyCode.C, provider.GetBindingSpecial());
    }
}
