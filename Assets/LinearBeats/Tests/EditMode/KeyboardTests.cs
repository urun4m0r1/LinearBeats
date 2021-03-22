using LinearBeats.Input;
using NUnit.Framework;
using UnityEngine;

public class KeyboardTests
{
    [Test]
    public void Name_And_Description_Can_Be_Modified()
    {
        #region Given
        Keyboard keyboard = ScriptableObject.CreateInstance<Keyboard>();

        var name = "Hello";
        var description = "World";
        #endregion

        #region When
        keyboard.Name = name;
        keyboard.Description = description;
        #endregion

        #region Then
        Assert.AreEqual(name, keyboard.Name);
        Assert.AreEqual(description, keyboard.Description);
        #endregion
    }

    [Test]
    public void Can_Set_And_Get_Binding()
    {
        #region Given
        byte row = 1;
        byte col = 6;

        KeyCode keyCode = KeyCode.A;
        KeyCode keyCodeAlternative = KeyCode.B;
        KeyCode keyCodeSpecial = KeyCode.C;

        KeyboardCustom keyboard = ScriptableObject.CreateInstance<KeyboardCustom>();
        var receiver = keyboard as IBindingReceiver;
        var provider = keyboard as IBindingProvider;
        #endregion

        #region When
        receiver.SetBinding(row, col, keyCode);
        receiver.SetBindingAlternative(row, col, keyCodeAlternative);
        receiver.SetBindingSpecial(keyCodeSpecial);
        #endregion

        #region Then
        Assert.AreEqual(keyCode, provider.GetBinding(row, col));
        Assert.AreEqual(keyCodeAlternative, provider.GetBindingAlternative(row, col));
        Assert.AreEqual(keyCodeSpecial, provider.GetBindingSpecial());
        #endregion
    }
}
