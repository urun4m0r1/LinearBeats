using JetBrains.Annotations;
using LinearBeats.Script;

namespace LinearBeats.Media
{
    public interface IMediaPlayer
    {
        void Play([CanBeNull] Second? playbackPosition = null, [CanBeNull] Second? playLength = null);
        void Resume();
        void Pause();
        void Stop();
    }
}
