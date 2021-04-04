#pragma warning disable IDE0090

using NUnit.Framework;

public static class TestUtils
{
    public static void AreEqual(float expected, float actual)
    {
        Assert.AreEqual(expected, actual, 0.0001);
    }
}
