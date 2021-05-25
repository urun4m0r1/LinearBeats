using JetBrains.Annotations;
using LinearBeats.Time;
using Sirenix.OdinInspector;

namespace LinearBeats.Audio
{
    //TODO: 오프셋이 양수일시 노래만 늦게 나오고 스크롤이 멈춰있음
    public sealed class AudioTimingInfo
    {
        [ShowInInspector, ReadOnly, HideLabel]
        public FixedTime Now => _fixedTimeFactory.Create(Current - _audioClip.Offset);

        [ShowInInspector, ReadOnly, HorizontalGroup(0.2f, LabelWidth = 60)]
        public bool IsPlaying => _audioClip.IsPlaying;

        [ShowInInspector, ReadOnly, ProgressBar(0f, 1f), HorizontalGroup]
        public float Progress => Current / _audioClip.Length;

        private FixedTime Current => _fixedTimeFactory.Create(_audioClip.Current);
        [NotNull] private readonly IAudioClip _audioClip;
        [NotNull] private readonly FixedTime.Factory _fixedTimeFactory;

        public AudioTimingInfo([NotNull] IAudioClip audioClip,
            [NotNull] FixedTime.Factory fixedTimeFactory)
        {
            _audioClip = audioClip;
            _fixedTimeFactory = fixedTimeFactory;
        }
    }
}
