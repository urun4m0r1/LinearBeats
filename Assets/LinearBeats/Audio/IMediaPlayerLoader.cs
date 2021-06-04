using System.Collections.Generic;
using JetBrains.Annotations;
using LinearBeats.Script;

namespace LinearBeats.Audio
{
    public interface IMediaPlayerLoader<T>
    {
        Dictionary<ushort, T> LoadMediaPlayer([NotNull] IReadOnlyCollection<MediaChannel> mediaChannels);
    }
}
