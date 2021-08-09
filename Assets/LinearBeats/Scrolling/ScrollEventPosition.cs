using Sirenix.OdinInspector;

namespace LinearBeats.Scrolling
{
    public sealed partial class PositionConverter
    {
        private readonly struct ScrollEventPosition
        {
            [ShowInInspector, ReadOnly, HorizontalGroup(LabelWidth = 30)] public Position Start { get; }
            [ShowInInspector, ReadOnly, HorizontalGroup] public Position End { get; }
            [ShowInInspector, ReadOnly, HorizontalGroup] public Position Duration { get; }
            [ShowInInspector, ReadOnly] public float? Amount { get; }

            public ScrollEventPosition(Position start, Position end, float? amount = null)
            {
                Start = start;
                End = end;
                Duration = end - start;
                Amount = amount;
            }
        }
    }
}
