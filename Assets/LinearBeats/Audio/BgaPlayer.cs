using JetBrains.Annotations;
using LinearBeats.Script;
using UnityEngine.Video;
namespace LinearBeats.Audio
{
    public sealed class BgaPlayer
    {
        [NotNull] public VideoPlayer VideoPlayer { get; }
        private readonly Second _offset;

        public BgaPlayer([NotNull] VideoPlayer videoPlayer, Second? offset = null)
        {
            VideoPlayer = videoPlayer;
            _offset = offset ?? default;
        }

        public void Play(Second start = default, Second length = default)
        {
            var startSecond = start - _offset;
            VideoPlayer.Stop();
            if (startSecond < 0)
            {
                //VideoPlayer.PlayDelayed(-startSecond);
            }
            else
            {
                VideoPlayer.time = startSecond;
                VideoPlayer.Play();
            }

            //if (length != default) VideoPlayer.SetScheduledEndTime(AudioSettings.dspTime + length);
        }

        public void Resume()
        {
            VideoPlayer.Play();
        }

        public void Pause()
        {
            VideoPlayer.Pause();
        }

        public void Stop()
        {
            Play();
            VideoPlayer.Pause();
        }
    }
}
