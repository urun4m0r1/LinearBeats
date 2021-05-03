using JetBrains.Annotations;
using LinearBeats.Time;

namespace LinearBeats.Scrolling
{
    public readonly struct RailObject
    {
        [NotNull] private readonly IDistanceConverter _converter;
        public FixedTime StartTime { get; }
        public FixedTime EndTime => _duration == null ? StartTime : StartTime + (FixedTime) _duration;

        private readonly ScrollEvent _ignoreFlags;
        private readonly FixedTime? _duration;

        public RailObject([NotNull] IDistanceConverter converter,
            FixedTime startTime,
            FixedTime? duration = null,
            ScrollEvent ignoreFlags = ScrollEvent.None)
        {
            _converter = converter;
            StartTime = startTime;
            _duration = duration;
            _ignoreFlags = ignoreFlags;
        }

        public float GetStartPosition(FixedTime currentTime) =>
            _converter.GetRailPosition(currentTime, StartTime, _ignoreFlags);

        public float GetEndPosition(FixedTime currentTime) =>
            _converter.GetRailPosition(currentTime, EndTime, _ignoreFlags);

        public float ScaleLength(float length) =>
            length * _converter.DistancePerQuarterNote;
    }
}
