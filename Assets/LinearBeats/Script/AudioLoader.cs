using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace LinearBeats.Script
{
    public sealed class AudioLoader
    {
        [ShowInInspector, ReadOnly] [NotNull] private readonly AudioListener _audioListener;
        [ShowInInspector, ReadOnly] [NotNull] private readonly IReadOnlyList<AudioMixerGroup> _audioMixerGroups;
        [ShowInInspector, ReadOnly] [NotNull] private readonly Func<string, AudioClip> _getAudioClipFromFileName;

        public AudioLoader(
            [NotNull] AudioListener audioListener,
            [NotNull] IReadOnlyList<AudioMixerGroup> audioMixerGroups,
            [NotNull] Func<string, AudioClip> getAudioClipFromFileName)
        {
            _audioListener = audioListener;
            _audioMixerGroups = audioMixerGroups;
            _getAudioClipFromFileName = getAudioClipFromFileName;
        }

        [NotNull]
        public Dictionary<ushort, AudioPlayer> InstantiateAudioSources(
            [NotNull] IReadOnlyCollection<MediaChannel> audioChannels)
        {
            if (audioChannels.Any(v => string.IsNullOrWhiteSpace(v.FileName)))
                throw new InvalidScriptException("All audioChannels must have proper filename");

            var dict = new Dictionary<ushort, AudioPlayer>();
            foreach (var audioChannel in audioChannels)
            {
                var audioSource = CreateAudioSource(audioChannel);
                var audioPlayer = new AudioPlayer(audioSource, audioChannel.Offset);
                dict.Add(audioChannel.Channel, audioPlayer);
            }

            return dict;
        }

        [NotNull] private AudioSource CreateAudioSource(MediaChannel audioChannel)
        {
            var audioObject = new GameObject(audioChannel.FileName);
            audioObject.transform.parent = _audioListener.transform;

            var audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = _getAudioClipFromFileName(audioChannel.FileName);
            audioSource.outputAudioMixerGroup = _audioMixerGroups[audioChannel.Layer ?? 0];

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
