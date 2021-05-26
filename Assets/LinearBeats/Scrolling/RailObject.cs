using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Time;

namespace LinearBeats.Scrolling
{
    public class RailObject
    {
        public virtual FixedTime StartTime { get; }
        public virtual FixedTime EndTime { get; }
        public FixedTime CurrentTime => _timingObject.Current;
        public float StartPosition => Converter.GetRailPosition(_timingObject.Current, StartTime, _ignoreFlags);
        public float EndPosition => Converter.GetRailPosition(_timingObject.Current, EndTime, _ignoreFlags);


        [NotNull] private readonly TimingObject _timingObject;
        [NotNull] protected FixedTime.Factory Factory => _timingObject.Factory;
        [NotNull] private IDistanceConverter Converter => _timingObject.Converter;

        private readonly ScrollEvent _ignoreFlags;

        protected RailObject([NotNull] TimingObject timingObject, ScrollEvent ignoreFlags)
        {
            _timingObject = timingObject;
            _ignoreFlags = ignoreFlags;

            StartTime = Factory.Create(new Pulse(0));
            EndTime = Factory.Create(new Pulse(0));
        }
    }

    public sealed class DividerRail : RailObject
    {
        public override FixedTime StartTime { get; }
        public override FixedTime EndTime => StartTime;

        public DividerRail([NotNull] TimingObject timingObject, ScrollEvent ignoreFlags, Pulse startTime)
            : base(timingObject, ignoreFlags) => StartTime = Factory.Create(startTime);
    }

    public sealed class NoteRail : RailObject
    {
        public override FixedTime StartTime { get; }
        public override FixedTime EndTime { get; }

        public NoteRail([NotNull] TimingObject timingObject, ScrollEvent ignoreFlags, Trigger trigger)
            : base(timingObject, ignoreFlags)
        {
            StartTime = Factory.Create(trigger.Pulse);
            EndTime = StartTime + Factory.Create(trigger.Duration);
        }
    }
}
