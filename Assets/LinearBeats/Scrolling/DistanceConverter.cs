using JetBrains.Annotations;
using LinearBeats.Time;

namespace LinearBeats.Scrolling
{
    public interface IDistanceConverter
    {
        float GetRailPosition(FixedTime currentTime, FixedTime targetTime, ScrollEvent ignoreFlags = ScrollEvent.None);
        float DistancePerQuarterNote { get; set; }
    }

    public sealed class DistanceConverter : IDistanceConverter
    {
        [NotNull] private readonly IPositionConverter _positionConverter;
        public float DistancePerQuarterNote { get; set; }

        public DistanceConverter([NotNull] IPositionConverter positionConverter, float distancePerQuarterNote)
        {
            _positionConverter = positionConverter;
            DistancePerQuarterNote = distancePerQuarterNote;
        }

        public float GetRailPosition(FixedTime currentTime, FixedTime targetTime, ScrollEvent ignoreFlags = ScrollEvent.None)
        {
            var current = _positionConverter.Convert(currentTime, ignoreFlags);
            var target = _positionConverter.Convert(targetTime, ignoreFlags);
            var pos = DistancePerQuarterNote * (target - current);
            return pos;
        }
    }
}
