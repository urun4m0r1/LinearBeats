using LinearBeats.Script;

namespace LinearBeats.Media
{
    public interface IMediaPlayer
    {
        void Play(Second start = default, Second? length = null);
        void Resume();
        void Pause();
        void Stop();
    }
}
