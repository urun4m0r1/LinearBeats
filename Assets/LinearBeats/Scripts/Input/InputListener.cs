#pragma warning disable IDE0090

using LinearBeats.Script;

namespace LinearBeats.Input
{
    public abstract class InputListener
    {
        public InputPosition GetNoteInvoked(Shape noteShape)
        {
            return GetAnyInputInvokedIn(
             noteShape.PosRow,
             noteShape.PosCol,
             noteShape.SizeRow,
             noteShape.SizeCol);
        }

        public InputPosition GetAnyInputInvokedIn(
            byte rowStart,
            byte colStart,
            byte rowSize,
            byte colSize)
        {
            var rowEnd = (byte)(rowStart + rowSize);
            var colEnd = (byte)(colStart + colSize);

            for (var row = rowStart; row < rowEnd; ++row)
            {
                for (var col = colStart; col < colEnd; ++col)
                {
                    if (IsInputInvoked(row, col)) return new InputPosition(row, col);
                }
            }
            return new InputPosition(null, null);
        }

        public abstract bool IsInputInvoked(byte row, byte col);

        public abstract bool IsSpecialInputInvoked();
    }
}
