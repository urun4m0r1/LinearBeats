﻿namespace LinearBeats.Scrolling
{
    public sealed partial class PositionConverter
    {
        private readonly struct ScrollEventPosition
        {
            public Position Start { get; }
            public Position End { get; }
            public Position Duration { get; }
            public float? Amount { get; }

            public ScrollEventPosition(Position start, Position end, float? amount = null)
            {
                Start = start;
                End = end;
                Duration = end - start;
                Amount = amount;
            }
        }
    }
}
