using JetBrains.Annotations;
using LinearBeats.Time;

namespace LinearBeats.Scrolling
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
            internal abstract Position Normalize(float scaledPulse, int timingIndex);
        }

        private sealed class StandardNormalizer : PositionNormalizer
        {
            public StandardNormalizer([NotNull] ITimingModifier modifier) : base(modifier)
            {
            }

            internal override Position Normalize(float scaledPulse, int _) =>
                Modifier.Normalize(scaledPulse);
        }

        private sealed class IndividualNormalizer : PositionNormalizer
        {
            public IndividualNormalizer([NotNull] ITimingModifier modifier) : base(modifier)
            {
            }

            internal override Position Normalize(float scaledPulse, int timingIndex) =>
                Modifier.Flatten(scaledPulse, timingIndex);
        }
    }
}
