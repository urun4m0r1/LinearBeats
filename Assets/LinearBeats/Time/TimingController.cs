using System;
using JetBrains.Annotations;
using UnityEngine;

namespace LinearBeats.Time
{
    public sealed class TimingController
    {
        public FixedTime CurrentTime => _fixedTimeFactory.Create(new Sample(_audioSource.timeSamples) + _offset);
        public float CurrentProgress => CurrentTime.NormalizedPulse / _length.NormalizedPulse;

        [NotNull] private readonly AudioSource _audioSource;
        [NotNull] private readonly FixedTimeFactory _fixedTimeFactory;
        private readonly FixedTime _offset;
        private readonly FixedTime _length;

        public TimingController([NotNull] AudioSource audioSource,
            [NotNull] FixedTimeFactory fixedTimeFactory,
            Second offset = default)
        {
            _audioSource = audioSource;

            _fixedTimeFactory = fixedTimeFactory;
            _offset = fixedTimeFactory.Create(offset);
            _length = fixedTimeFactory.Create(new Sample(audioSource.clip.samples));
        }
    }
}
