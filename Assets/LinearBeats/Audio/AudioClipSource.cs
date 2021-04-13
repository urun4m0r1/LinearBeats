using JetBrains.Annotations;
using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Audio
{
    public interface IAudioClip
    {
        Sample Current { get; }
        Sample Length { get; }
    }

    public sealed class AudioClipSource : IAudioClip
    {
        [NotNull] private readonly AudioSource _audioSource;
        public Sample Current => _audioSource.clip.samples;
        public Sample Length => _audioSource.clip.length;

        public AudioClipSource([NotNull] AudioSource audioSource) => _audioSource = audioSource;
    }
}
