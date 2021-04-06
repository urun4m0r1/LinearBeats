using System;
using JetBrains.Annotations;

namespace LinearBeats.Time
{
    public sealed class FixedTimeFactory
    {
        private readonly IPositionConverter _converter;

        public FixedTimeFactory([NotNull] IPositionConverter converter) =>
            _converter = converter ?? throw new ArgumentNullException();

        public FixedTime Create(Pulse value) => new FixedTime(_converter, value);
        public FixedTime Create(Sample value) => new FixedTime(_converter, value);
        public FixedTime Create(Second value) => new FixedTime(_converter, value);
    }
}
