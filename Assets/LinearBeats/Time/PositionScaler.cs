using JetBrains.Annotations;

namespace LinearBeats.Time
{
    public enum ScalerMode
    {
        RegularInterval,
        BpmRelative,
        ConstantSpeed,
    }

    public sealed partial class PositionConverter
    {
        private abstract class PositionScaler
        {
            [NotNull] protected readonly ITimingModifier Modifier;
            protected PositionScaler([NotNull] ITimingModifier modifier) => Modifier = modifier;
            internal abstract Pulse Scale(Pulse pulse, int? timingIndex = null);
        }

        private sealed class RegularIntervalScaler : PositionScaler
        {
            public RegularIntervalScaler([NotNull] ITimingModifier modifier) : base(modifier)
            {
            }

            internal override Pulse Scale(Pulse pulse, int? _ = null) => pulse;
        }

        private sealed class PositionRelativeScaler : PositionScaler
        {
            public PositionRelativeScaler([NotNull] ITimingModifier modifier) : base(modifier)
            {
            }

            internal override Pulse Scale(Pulse pulse, int? timingIndex = null) =>
                Modifier.ScaleWithBpm(pulse, timingIndex ?? Modifier.GetTimingIndex(pulse));
        }

        private sealed class ConstantSpeedScaler : PositionScaler
        {
            public ConstantSpeedScaler([NotNull] ITimingModifier modifier) : base(modifier)
            {
            }

            internal override Pulse Scale(Pulse pulse, int? timingIndex = null) =>
                Modifier.ScaleWithBpmInversed(pulse, timingIndex ?? Modifier.GetTimingIndex(pulse));
        }
    }
}
