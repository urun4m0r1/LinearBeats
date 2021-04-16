using JetBrains.Annotations;
using LinearBeats.Audio;

namespace LinearBeats.Time
{
    public sealed class TimingController
    {
        public FixedTime CurrentTime => _fixedTimeFactory.Create(Current + _audioClip.Offset);
        public float CurrentProgress => Current / _audioClip.Length;

        private FixedTime Current => _fixedTimeFactory.Create(_audioClip.Current);
        [NotNull] private readonly IAudioClip _audioClip;
        [NotNull] private readonly FixedTime.Factory _fixedTimeFactory;

        public TimingController([NotNull] IAudioClip audioClip,
            [NotNull] FixedTime.Factory fixedTimeFactory)
        {
            _audioClip = audioClip;
            _fixedTimeFactory = fixedTimeFactory;
        }
    }
}
