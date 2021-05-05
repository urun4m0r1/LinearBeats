using JetBrains.Annotations;
using LinearBeats.Audio;

namespace LinearBeats.Time
{
    public sealed class AudioTimingInfo
    {
        public FixedTime Now => _fixedTimeFactory.Create(Current + _audioClip.Offset);
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
