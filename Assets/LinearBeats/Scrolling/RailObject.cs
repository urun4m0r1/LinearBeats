using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Time;

namespace LinearBeats.Scrolling
{
    public class RailObject
    {
        public virtual FixedTime StartTime { get; }
        public virtual FixedTime EndTime { get; }
        public FixedTime CurrentTime => _timingInfo.AudioTimingInfo.Now;
        public float StartPosition => Converter.GetRailPosition(_timingInfo.AudioTimingInfo.Now, StartTime, _ignoreFlags);
        public float EndPosition => Converter.GetRailPosition(_timingInfo.AudioTimingInfo.Now, EndTime, _ignoreFlags);


        [NotNull] private readonly TimingInfo _timingInfo;
        [NotNull] protected FixedTime.Factory Factory => _timingInfo.Factory;
        [NotNull] private IDistanceConverter Converter => _timingInfo.Converter;

        private readonly ScrollEvent _ignoreFlags;

        protected RailObject([NotNull] TimingInfo timingInfo, ScrollEvent ignoreFlags)
        {
            _timingInfo = timingInfo;
            _ignoreFlags = ignoreFlags;

            StartTime = Factory.Create(new Pulse(0));
            EndTime = Factory.Create(new Pulse(0));
        }
    }

    public sealed class DividerRail : RailObject
    {
        public override FixedTime StartTime { get; }
        public override FixedTime EndTime => StartTime;

        public DividerRail([NotNull] TimingInfo timingInfo, ScrollEvent ignoreFlags, Pulse startTime)
            : base(timingInfo, ignoreFlags) => StartTime = Factory.Create(startTime);
    }

    public sealed class NoteRail : RailObject
    {
        public override FixedTime StartTime { get; }
        public override FixedTime EndTime { get; }

        public NoteRail([NotNull] TimingInfo timingInfo, ScrollEvent ignoreFlags, Trigger trigger)
            : base(timingInfo, ignoreFlags)
        {
            StartTime = Factory.Create(trigger.Pulse);
            EndTime = StartTime + Factory.Create(trigger.Duration ?? 0);
        }
    }
}
