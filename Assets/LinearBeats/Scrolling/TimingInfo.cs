using JetBrains.Annotations;
using LinearBeats.Judgement;
using LinearBeats.Media;
using LinearBeats.Time;

namespace LinearBeats.Scrolling
{
    public sealed class TimingInfo
    {
        [NotNull] public AudioTimingInfo AudioTimingInfo { get; }
        [NotNull] public FixedTime.Factory Factory { get; }
        [NotNull] public IDistanceConverter Converter { get; }
        [NotNull] public NoteJudgement Judgement { get; }

        public TimingInfo(
            [NotNull] AudioTimingInfo audioTimingInfo,
            [NotNull] FixedTime.Factory factory,
            [NotNull] IDistanceConverter converter,
            [NotNull] NoteJudgement judgement)
        {
            AudioTimingInfo = audioTimingInfo;
            Factory = factory;
            Converter = converter;
            Judgement = judgement;
        }
    }
}
