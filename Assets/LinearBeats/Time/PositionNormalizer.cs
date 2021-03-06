﻿using JetBrains.Annotations;

namespace LinearBeats.Time
{
    public enum NormalizerMode
    {
        Standard,
        Individual,
    }

    public sealed partial class PositionConverter
    {
        private abstract class PositionNormalizer
        {
            [NotNull] protected readonly ITimingModifier Modifier;
            protected PositionNormalizer([NotNull] ITimingModifier modifier) => Modifier = modifier;
            internal abstract Position Normalize(Pulse pulse, int? timingIndex = null);
        }

        private sealed class StandardNormalizer : PositionNormalizer
        {
            public StandardNormalizer([NotNull] ITimingModifier modifier) : base(modifier)
            {
            }

            internal override Position Normalize(Pulse pulse, int? _ = null) =>
                Modifier.NormalizeWithStandardPpqn(pulse);
        }

        private sealed class IndividualNormalizer : PositionNormalizer
        {
            public IndividualNormalizer([NotNull] ITimingModifier modifier) : base(modifier)
            {
            }

            internal override Position Normalize(Pulse pulse, int? timingIndex = null) =>
                Modifier.NormalizeWithPpqn(pulse, timingIndex ?? Modifier.GetTimingIndex(pulse));
        }
    }
}
