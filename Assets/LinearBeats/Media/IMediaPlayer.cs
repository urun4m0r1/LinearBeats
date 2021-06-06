using LinearBeats.Script;

namespace LinearBeats.Media
{
    public interface IMediaPlayer
    {
        public Second Offset { get; }
        void Play(Second start = default, Second length = default);
        void Resume();
        void Pause();
        void Stop();
    }
}
