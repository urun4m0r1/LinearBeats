using System;
using JetBrains.Annotations;
using LinearBeats.Visuals;

namespace LinearBeats.Time
{
    public sealed class FixedTimeFactory
    {
        private readonly PositionConverter _positionConverter;

        public FixedTimeFactory([NotNull] PositionConverter positionConverter) =>
            _positionConverter = positionConverter ?? throw new ArgumentNullException();

        public FixedTime Create(Pulse value) => new FixedTime(_positionConverter, value);
        public FixedTime Create(Sample value) => new FixedTime(_positionConverter, value);
        public FixedTime Create(Second value) => new FixedTime(_positionConverter, value);
    }
}
