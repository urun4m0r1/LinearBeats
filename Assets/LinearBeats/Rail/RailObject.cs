using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Scrolling;
using LinearBeats.Time;
using Sirenix.OdinInspector;

namespace LinearBeats.Rail
{
    public class RailObject
    {
        [ShowInInspector, ReadOnly] public virtual FixedTime StartTime { get; }
        [ShowInInspector, ReadOnly] public virtual FixedTime EndTime { get; }
        [ShowInInspector, ReadOnly] public FixedTime CurrentTime => _timingInfo.AudioTimingInfo.Current;
        [ShowInInspector, ReadOnly] public float StartPosition => Converter.GetRailPosition(_timingInfo.AudioTimingInfo.Current, StartTime, _ignoreFlags);
        [ShowInInspector, ReadOnly] public float EndPosition => Converter.GetRailPosition(_timingInfo.AudioTimingInfo.Current, EndTime, _ignoreFlags);

        [ShowInInspector, ReadOnly] [NotNull] private readonly TimingInfo _timingInfo;
        [NotNull] protected FixedTime.Factory Factory => _timingInfo.Factory;
        [NotNull] private IDistanceConverter Converter => _timingInfo.Converter;

        private readonly ScrollEvent _ignoreFlags;

        protected RailObject([NotNull] TimingInfo timingInfo, ScrollEvent ignoreFlags)
        {
            _timingInfo = timingInfo;
            _ignoreFlags = ignoreFlags;
        }
    }

    public sealed class DividerRail : RailObject
    {
        [ShowInInspector, ReadOnly] public override FixedTime StartTime { get; }
        [ShowInInspector, ReadOnly] public override FixedTime EndTime => StartTime;

        public DividerRail([NotNull] TimingInfo timingInfo, ScrollEvent ignoreFlags, Pulse startTime)
            : base(timingInfo, ignoreFlags) => StartTime = Factory.Create(startTime);
    }

    public sealed class NoteRail : RailObject
    {
        [ShowInInspector, ReadOnly] public override FixedTime StartTime { get; }
        [ShowInInspector, ReadOnly] public override FixedTime EndTime { get; }

        public NoteRail([NotNull] TimingInfo timingInfo, ScrollEvent ignoreFlags, Trigger trigger)
            : base(timingInfo, ignoreFlags)
        {
            StartTime = Factory.Create(trigger.Pulse);
            EndTime = StartTime + Factory.Create(trigger.Duration ?? 0);
        }
    }
}
