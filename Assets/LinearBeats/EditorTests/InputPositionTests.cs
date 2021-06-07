using LinearBeats.Input;
using NUnit.Framework;
/*
public class InputPositionTests
{
    [Test]
    public void Will_Throw_Exception_When_Any_Input_Is_Null()
    {
        #region Given
        byte row = 1;
        byte col = 6;
        #endregion

        #region When
        var rowColNull = new InputPosition(null, null);
        var colNull = new InputPosition(row, null);
        var rowNull = new InputPosition(null, col);
        var rowCol = new InputPosition(row, col);
        #endregion

        #region Then
        Assert.That(() => rowColNull.Col, Throws.Exception);
        Assert.That(() => rowColNull.Row, Throws.Exception);
        Assert.That(() => colNull.Col, Throws.Exception);
        Assert.That(() => rowNull.Row, Throws.Exception);

        Assert.IsFalse(rowColNull.Exist);
        Assert.IsFalse(colNull.Exist);
        Assert.IsFalse(rowNull.Exist);
        Assert.IsTrue(rowCol.Exist);

        Assert.AreEqual(row, colNull.Row);
        Assert.AreEqual(col, rowNull.Col);
        Assert.AreEqual(row, rowCol.Row);
        Assert.AreEqual(col, rowCol.Col);
        #endregion
    }
}
*/
