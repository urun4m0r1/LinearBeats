using JetBrains.Annotations;
using LinearBeats.Time;
using UnityEngine;

namespace LinearBeats.Audio
{
    public sealed class AudioPlayer
    {
        [NotNull] public AudioSource AudioSource { get; }
        private readonly Second _offset;

        public AudioPlayer([NotNull] AudioSource audioSource, Second offset)
        {
            AudioSource = audioSource;
            _offset = offset;
        }

        public void Play(Second start, Second length)
        {
            Play(start);
            AudioSource.SetScheduledEndTime(AudioSettings.dspTime + length);
        }

        public void Play(Second start = default)
        {
            var startSecond = start - _offset;
            AudioSource.Stop();
            if (startSecond < 0)
            {
                AudioSource.PlayDelayed(-startSecond);
            }
            else
            {
                AudioSource.time = startSecond;
                AudioSource.Play();
            }
        }

        public void Resume()
        {
            AudioSource.UnPause();
        }

        public void Pause()
        {
            AudioSource.Pause();
        }

        public void Stop()
        {
            Play();
            AudioSource.Pause();
        }
    }
}
