using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace LinearBeats.Media
{
    public sealed class AudioPlayerLoader : IMediaPlayerLoader
    {
        [ShowInInspector, ReadOnly] [NotNull] private readonly AudioListener _audioListener;
        [ShowInInspector, ReadOnly] [NotNull] private readonly IReadOnlyList<AudioMixerGroup> _audioMixerGroups;
        [ShowInInspector, ReadOnly] [NotNull] private readonly Func<string, AudioClip> _getClipFromFileName;

        public AudioPlayerLoader(
            [NotNull] AudioListener audioListener,
            [NotNull] IReadOnlyList<AudioMixerGroup> audioMixerGroups,
            [NotNull] Func<string, AudioClip> getClipFromFileName)
        {
            _audioListener = audioListener;
            _audioMixerGroups = audioMixerGroups;
            _getClipFromFileName = getClipFromFileName;
        }

        public Dictionary<ushort, IMediaPlayer> Load(IReadOnlyCollection<MediaChannel> mediaChannels)
        {
            var dict = new Dictionary<ushort, IMediaPlayer>();

            if (mediaChannels == null)
                return dict;

            if (mediaChannels.Any(v => string.IsNullOrWhiteSpace(v.FileName)))
                throw new InvalidScriptException("All audioChannels must have proper filename");

            foreach (var mediaChannel in mediaChannels)
            {
                var audioSource = CreateAudioSource(mediaChannel);
                var audioPlayer = new AudioPlayer(audioSource, mediaChannel.Offset);
                dict.Add(mediaChannel.Channel, audioPlayer);
            }

            return dict;
        }

        [NotNull] private AudioSource CreateAudioSource(MediaChannel audioChannel)
        {
            var audioObject = new GameObject(audioChannel.FileName);
            audioObject.transform.parent = _audioListener.transform;

            var audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = _getClipFromFileName(audioChannel.FileName);
            audioSource.outputAudioMixerGroup = _audioMixerGroups[audioChannel.Layer ?? default];

            audioSource.bypassEffects = true;
            audioSource.bypassListenerEffects = true;
            audioSource.bypassReverbZones = true;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.priority = 0;
            audioSource.pitch = 1;
            audioSource.panStereo = 0;

            audioSource.mute = false;
            audioSource.volume = 1;

            return audioSource;
        }
    }
}
