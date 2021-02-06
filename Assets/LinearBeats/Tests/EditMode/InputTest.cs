using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LinearBeats.Input;

public class InputTest
{
    [Test]
    public void CustomKeyboard_GetSet_Will_Be_Same()
    {
        IBindingProvider custom = ScriptableObject.CreateInstance<KeyboardCustom>();
        (custom as KeyboardCustom).SetBinding(0, 0, KeyCode.A);
        Assert.AreEqual(KeyCode.A, custom.GetBinding(0, 0));
        (custom as KeyboardCustom).SetBindingAlternative(0, 0, KeyCode.B);
        Assert.AreEqual(KeyCode.B, custom.GetBindingAlternative(0, 0));
        (custom as KeyboardCustom).SetBindingSpecial(KeyCode.C);
        Assert.AreEqual(KeyCode.C, custom.GetBindingSpecial());
    }
}
