using JetBrains.Annotations;
using LinearBeats.Time;
using Sirenix.OdinInspector;

namespace LinearBeats.Scrolling
{
    public interface IDistanceConverter
    {
        float GetRailPosition(FixedTime currentTime, FixedTime targetTime, ScrollEvent ignoreFlags = ScrollEvent.None);
        float DistancePerQuarterNote { get; set; }
    }

    public sealed class DistanceConverter : IDistanceConverter
    {
        [ShowInInspector, ReadOnly] public float DistancePerQuarterNote { get; set; }

        [ShowInInspector, ReadOnly] [NotNull] private readonly IPositionConverter _positionConverter;

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
