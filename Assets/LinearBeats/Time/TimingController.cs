using System;
using JetBrains.Annotations;
using UnityEngine;

namespace LinearBeats.Time
{
    public sealed class TimingController
    {
        public FixedTime CurrentTime => _fixedTimeFactory.Create(new Sample(_audioSource.timeSamples) + _offset);
        public float CurrentProgress => (CurrentTime / _length).Second;

        private readonly AudioSource _audioSource;
        private readonly FixedTimeFactory _fixedTimeFactory;
        private readonly FixedTime _length;
        private readonly FixedTime _offset;

        public TimingController([NotNull] FixedTimeFactory fixedTimeFactory,
            [NotNull] AudioSource audioSource,
            Second offset = default)
        {
            _audioSource = audioSource ? audioSource : throw new ArgumentNullException();
            _fixedTimeFactory = fixedTimeFactory ?? throw new ArgumentNullException();
            _length = fixedTimeFactory.Create(new Sample(audioSource.clip.samples));
            _offset = fixedTimeFactory.Create(offset);
        }
    }
}
