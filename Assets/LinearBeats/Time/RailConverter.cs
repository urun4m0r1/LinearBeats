namespace LinearBeats.Time
{
    public class RailConverter
    {
        private IPositionConverter PositionConverter { get; }
        private float MeterPerQuarterNote { get; }
        private float BpmMultiplier { get; }
        private float NoteDisappearOffset { get; }

        public RailConverter(IPositionConverter positionConverter,
            float meterPerQuarterNote,
            float bpmMultiplier,
            float noteDisappearOffset)
        {
            PositionConverter = positionConverter;
            MeterPerQuarterNote = meterPerQuarterNote;
            BpmMultiplier = bpmMultiplier;
            NoteDisappearOffset = noteDisappearOffset;
        }

        public (float, float) GetRailPosition(FixedTime currentTime,
            FixedTime startTime,
            FixedTime duration,
            ScrollEvent ignoreFlags)
        {
            //TODO: 롱노트, 슬라이드노트 처리 방법 생각하기 (시작점 끝점에 노트생성해 중간은 쉐이더로 처리 or 노트길이를 잘 조절해보기)

            var startPos = -100f;
            var endPos = -100f;

            if (currentTime.Second - NoteDisappearOffset >= startTime) return (startPos, endPos);

            var current = PositionConverter.Convert(currentTime, ignoreFlags);

            var start = PositionConverter.Convert(startTime, ignoreFlags);
            var startPositionInMeter = MeterPerQuarterNote * (start - current);
            startPos = startPositionInMeter * BpmMultiplier;

            if (duration.Pulse == 0)
            {
                endPos = startPos;
            }
            else
            {
                var end = PositionConverter.Convert(startTime + duration, ignoreFlags);
                var endPositionInMeter = MeterPerQuarterNote * (end - current);
                endPos = endPositionInMeter * BpmMultiplier;
            }

            return (startPos, endPos);
        }
    }
}
