using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using UnityEngine;
using UnityEngine.Video;

namespace LinearBeats.Audio
{
    public sealed class VideoPlayerLoader : IMediaPlayerLoader<BgaPlayer>
    {
        public VideoPlayerLoader() { }

        [NotNull] public Dictionary<ushort, BgaPlayer> LoadMediaPlayer(IReadOnlyCollection<MediaChannel> videoChannels)
        {
            if (videoChannels.Any(v => string.IsNullOrWhiteSpace(v.FileName)))
                throw new InvalidScriptException("All audioChannels must have proper filename");

            var dict = new Dictionary<ushort, BgaPlayer>();
            foreach (var videoChannel in videoChannels)
            {
                var videoPlayer = CreateVideoPlayer(videoChannel);
                var bgaPlayer = new BgaPlayer(videoPlayer, videoChannel.Offset);
                dict.Add(videoChannel.Channel, bgaPlayer);
            }

            return dict;
        }

        [NotNull] private VideoPlayer CreateVideoPlayer(MediaChannel videoChannel)
        {
            var videoObject = new GameObject(videoChannel.FileName);
            //videoObject.transform.parent = _audioListener.transform;

            var videoPlayer = videoObject.AddComponent<VideoPlayer>();
            /*
            videoPlayer.clip = _getAudioClipFromFileName(videoChannel.FileName);
            videoPlayer.outputAudioMixerGroup = _audioMixerGroups[videoChannel.Layer ?? default];

            videoPlayer.bypassEffects = true;
            videoPlayer.bypassListenerEffects = true;
            videoPlayer.bypassReverbZones = true;
            videoPlayer.playOnAwake = false;
            videoPlayer.loop = false;
            videoPlayer.priority = 0;
            videoPlayer.pitch = 1;
            videoPlayer.panStereo = 0;

            videoPlayer.mute = false;
            videoPlayer.volume = 1;

            */
            return videoPlayer;
        }
    }
}
