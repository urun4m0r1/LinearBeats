namespace LinearBeats.Time
{
    public sealed partial class PositionConverter
    {
        private readonly struct TimingEventPosition
        {
            public Position Start { get; }
            public Position End { get; }
            public Position Duration { get; }

            public TimingEventPosition(Position start, Position end)
            {
                Start = start;
                End = end;
                Duration = end - start;
            }
        }
    }
}
