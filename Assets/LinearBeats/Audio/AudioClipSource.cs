using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Audio
{
    public interface IAudioClip
    {
        Sample Current { get; }
        Second Length { get; }
        Second Offset { get; }
        bool IsPlaying { get; }
    }

    public sealed class AudioClipSource : IAudioClip
    {
        public Sample Current => _audioSource.timeSamples;
        public Second Length => _audioSource.clip.length;
        public Second Offset { get; }
        public int SamplesPerSecond => _audioSource.clip.frequency;
        public bool IsPlaying => _audioSource.isPlaying;

        [NotNull] private readonly AudioSource _audioSource;

        public AudioClipSource([NotNull] AudioSource audioSource, Second? offset = null)
        {
            _audioSource = audioSource;
            Offset = offset ?? default;
        }
    }
}
