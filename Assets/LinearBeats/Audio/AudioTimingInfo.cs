using JetBrains.Annotations;
using LinearBeats.Time;

namespace LinearBeats.Audio
{
    public sealed class AudioTimingInfo
    {
        //TODO: 오프셋이 양수일시 노래만 늦게 나오고 스크롤이 멈춰있음
        public FixedTime Now => _fixedTimeFactory.Create(Current - _audioClip.Offset);
        public float Progress => Current / _audioClip.Length;
        public bool IsPlaying => _audioClip.IsPlaying;

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
