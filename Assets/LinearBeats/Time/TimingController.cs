using JetBrains.Annotations;
using LinearBeats.Audio;

namespace LinearBeats.Time
{
    public sealed class TimingController
    {
        public FixedTime CurrentTime => _fixedTimeFactory.Create(_audioClip.Current + _offset);
        public float CurrentProgress => _audioClip.Current / _length;

        [NotNull] private readonly IAudioClip _audioClip;
        [NotNull] private readonly FixedTime.Factory _fixedTimeFactory;
        private readonly FixedTime _offset;
        private readonly FixedTime _length;

        public TimingController([NotNull] IAudioClip audioClip,
            [NotNull] FixedTime.Factory fixedTimeFactory,
            Second offset = default)
        {
            _audioClip = audioClip;

            _fixedTimeFactory = fixedTimeFactory;
            _offset = fixedTimeFactory.Create(offset);
            _length = fixedTimeFactory.Create(audioClip.Length);
        }
    }
}
