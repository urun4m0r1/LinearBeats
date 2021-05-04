using JetBrains.Annotations;

namespace LinearBeats.Time
{
    public sealed class TimingObject
    {
        public FixedTime Current { get; set; }
        [NotNull] public FixedTime.Factory Factory { get; }
        [NotNull] public IDistanceConverter Converter { get; }

        public TimingObject([NotNull] FixedTime.Factory factory, [NotNull] IDistanceConverter converter)
        {
            Current = factory.Create(new Pulse(0f));
            Factory = factory;
            Converter = converter;
        }
    }
}
