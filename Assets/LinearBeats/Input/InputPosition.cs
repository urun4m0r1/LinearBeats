using System;

namespace LinearBeats.Input
{
    public struct InputPosition
    {
        private readonly byte? _row;
        private readonly byte? _col;

        public byte Row => _row ?? throw new NullReferenceException();
        public byte Col => _col ?? throw new NullReferenceException();
        public bool Exist => _row != null && _col != null;

        public InputPosition(byte? row, byte? col)
        {
            _row = row;
            _col = col;
        }
    }
}
