using JetBrains.Annotations;
using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Audio
{
    public interface IAudioClip
    {
        Sample Current { get; }
        Second Length { get; }
        Second Offset { get; }
    }

    public sealed class AudioClipSource : IAudioClip
    {
        public Sample Current => _audioSource.timeSamples;
        public Second Length => _audioSource.clip.length;
        public Second Offset { get; }

        [NotNull] private readonly AudioSource _audioSource;

        public AudioClipSource([NotNull] AudioSource audioSource, Second offset = default)
        {
            _audioSource = audioSource;
            Offset = offset;
        }
    }
}
