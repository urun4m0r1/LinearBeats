using JetBrains.Annotations;
using LinearBeats.Script;
using UnityEngine;

namespace LinearBeats.Media
{
    public sealed class AudioPlayer : IMediaPlayer
    {
        public Second Offset { get; }

        [NotNull] public AudioSource Source { get; }
        public int SamplesPerSecond { get; }

        public AudioPlayer([NotNull] AudioSource source, Second? offset = null)
        {
            Source = source;
            SamplesPerSecond = source.clip.frequency;
            Offset = offset ?? default;
        }

        public void Play(Second start = default, Second length = default)
        {
            var startSecond = start - Offset;
            Source.Stop();
            if (startSecond < 0)
            {
                Source.PlayDelayed(-startSecond);
            }
            else
            {
                Source.time = startSecond;
                Source.Play();
            }

            if (length != default) Source.SetScheduledEndTime(AudioSettings.dspTime + length);
        }

        public void Resume()
        {
            Source.UnPause();
        }

        public void Pause()
        {
            Source.Pause();
        }

        public void Stop()
        {
            Play();
            Source.Pause();
        }
    }
}
