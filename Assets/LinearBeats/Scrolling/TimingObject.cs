using JetBrains.Annotations;
using LinearBeats.Judgement;
using LinearBeats.Time;

namespace LinearBeats.Scrolling
{
    public sealed class TimingObject
    {
        public FixedTime Current { get; set; }
        [NotNull] public FixedTime.Factory Factory { get; }
        [NotNull] public IDistanceConverter Converter { get; }
        [NotNull] public NoteJudgement Judgement { get; }

        public TimingObject([NotNull] FixedTime.Factory factory,
            [NotNull] IDistanceConverter converter,
            [NotNull] NoteJudgement judgement)
        {
            Current = factory.Create(new Pulse(0f));
            Factory = factory;
            Converter = converter;
            Judgement = judgement;
        }
    }
}
