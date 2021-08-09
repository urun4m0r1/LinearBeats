using JetBrains.Annotations;
using LinearBeats.Judgement;
using LinearBeats.Media;
using LinearBeats.Time;
using Sirenix.OdinInspector;

namespace LinearBeats.Scrolling
{
    public sealed class TimingInfo
    {
        [ShowInInspector, ReadOnly] [NotNull] public AudioTimingInfo AudioTimingInfo { get; }
        [NotNull] public FixedTime.Factory Factory { get; }
        [ShowInInspector, ReadOnly] [NotNull] public IDistanceConverter Converter { get; }
        [ShowInInspector, ReadOnly] [NotNull] public NoteJudgement Judgement { get; }

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
