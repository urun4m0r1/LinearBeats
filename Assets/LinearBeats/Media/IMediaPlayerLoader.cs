using System.Collections.Generic;
using JetBrains.Annotations;
using LinearBeats.Script;

namespace LinearBeats.Media
{
    public interface IMediaPlayerLoader
    {
        [NotNull] Dictionary<ushort, IMediaPlayer> Load([CanBeNull] IReadOnlyCollection<MediaChannel> mediaChannels);
    }
}
