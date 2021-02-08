#pragma warning disable IDE0090

using LinearBeats.Script;

namespace LinearBeats.Input
{
    public abstract class InputListener
    {
        public InputPosition GetNoteInvoked(Note note)
        {
            return GetAnyInputInvokedIn(
             note.PositionRow,
             note.PositionCol,
             note.SizeRow,
             note.SizeCol);
        }

        public InputPosition GetAnyInputInvokedIn(
            byte rowStart,
            byte colStart,
            byte rowSize,
            byte colSize)
        {
            byte rowEnd = (byte)(rowStart + rowSize);
            byte colEnd = (byte)(colStart + colSize);

            for (var row = rowStart; row < rowEnd; ++row)
            {
                for (var col = colStart; col < colEnd; ++col)
                {
                    if (IsInputInvoked(row, col))
                    {
                        return new InputPosition(row, col);
                    }
                }
            }
            return new InputPosition(null, null);
        }

        public abstract bool IsInputInvoked(byte row, byte col);

        public abstract bool IsSpecialInputInvoked();
    }
}
