#pragma warning disable IDE0090

using System;
using LinearBeats.Visuals;

namespace LinearBeats.Time
{
    public class FixedTimeFactory
    {
        public PositionConverter PositionConverter { get; }

        public FixedTimeFactory(PositionConverter positionConverter)
        {
            PositionConverter = positionConverter ?? throw new ArgumentNullException();
        }

        public FixedTime Create(Pulse value) => new FixedTime(PositionConverter, value);
        public FixedTime Create(Sample value) => new FixedTime(PositionConverter, value);
        public FixedTime Create(Second value) => new FixedTime(PositionConverter, value);
    }
}
