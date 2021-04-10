using System;
using JetBrains.Annotations;

namespace LinearBeats.Time
{
    public sealed class FixedTimeFactory
    {
        [NotNull] private readonly ITimingConverter _converter;

        public FixedTimeFactory([NotNull] ITimingConverter converter) => _converter = converter;

        public FixedTime Create(Pulse value) => new FixedTime(_converter, value);
        public FixedTime Create(Sample value) => new FixedTime(_converter, value);
        public FixedTime Create(Second value) => new FixedTime(_converter, value);
    }
}
