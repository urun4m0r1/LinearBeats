using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lean.Pool;
using LinearBeats.Audio;
using LinearBeats.Scrolling;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace LinearBeats.Script
{
    public sealed class ScriptLoader
    {
        [ShowInInspector, ReadOnly] [NotNull] private readonly LeanGameObjectPool _notesPool;
        [ShowInInspector, ReadOnly] [NotNull] private readonly LeanGameObjectPool _dividerPool;
        [ShowInInspector, ReadOnly] [NotNull] private readonly AudioListener _audioListener;
        [ShowInInspector, ReadOnly] [NotNull] private readonly IReadOnlyList<AudioMixerGroup> _audioMixerGroups;

        public ScriptLoader(
            [NotNull] LeanGameObjectPool notesPool,
            [NotNull] LeanGameObjectPool dividerPool,
            [NotNull] AudioListener audioListener,
            [NotNull] IReadOnlyList<AudioMixerGroup> audioMixerGroups)
        {
            _notesPool = notesPool;
            _dividerPool = dividerPool;
            _audioListener = audioListener;
            _audioMixerGroups = audioMixerGroups;
        }

        [NotNull] public Dictionary<ushort, AudioPlayer> InstantiateAudioSource(
            ref LinearBeatsScript script,
            [NotNull] string resourcesPath)
        {
            var dict = new Dictionary<ushort, AudioPlayer>();
            foreach (var audioChannel in script.AudioChannels ?? throw new ArgumentNullException())
            {
                var audioSource = CreateAudioSource(audioChannel);
                var audioPlayer = new AudioPlayer(audioSource, audioChannel.Offset);
                dict.Add(audioChannel.Channel, audioPlayer);
            }

            return dict;

            AudioSource CreateAudioSource(MediaChannel audioChannel)
            {
                var audioObject = new GameObject(audioChannel.FileName);
                audioObject.transform.parent = _audioListener.transform;

                var audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>(resourcesPath + audioChannel.FileName);
                audioSource.playOnAwake = false;
                audioSource.bypassEffects = true;
                audioSource.bypassListenerEffects = true;
                audioSource.bypassReverbZones = true;
                audioSource.outputAudioMixerGroup = _audioMixerGroups[audioChannel.Layer];
                return audioSource;
            }
        }

        public void InstantiateAllNotes(
            ref LinearBeatsScript script,
            [NotNull] TimingObject timingObject,
            [NotNull] Dictionary<ushort, AudioPlayer> audioPlayers)
        {
            if (script.Notes == null) return;

            for (var i = 0; i < script.Notes.Length; ++i)
            {
                var note = script.Notes[i];
                var noteObject = _notesPool.Spawn(_notesPool.transform);
                var noteBehaviour = noteObject.GetComponent<NoteBehaviour>();

                var ignoreOptions = ParseIgnoreOptions(note.IgnoreScrollEvent);
                noteBehaviour.RailObject = new NoteRail(timingObject, ignoreOptions, note.Trigger);
                noteBehaviour.NoteShape = note.Shape;
                noteBehaviour.Judgement = timingObject.Judgement;
                noteBehaviour.AudioPlayer = audioPlayers[note.Trigger.Channel];

                if (i + 1 == script.Notes.Length) continue;

                var intervalPulse = script.Notes[i + 1].Trigger.Pulse - note.Trigger.Pulse;
                noteBehaviour.AudioLength = timingObject.Factory.Create(intervalPulse);
            }
        }

        //TODO: 조금씩 생성시켜 사용하기
        public void InstantiateAllDividers(
            ref LinearBeatsScript script,
            [NotNull] TimingObject timingObject)
        {
            if (script.Dividers == null) return;

            foreach (var divider in script.Dividers)
            {
                var dividerObject = _dividerPool.Spawn(_dividerPool.transform);
                var dividerBehaviour = dividerObject.GetComponent<DividerBehaviour>();

                var ignoreOptions = ParseIgnoreOptions(divider.IgnoreScrollEvent);
                dividerBehaviour.RailObject = new DividerRail(timingObject, ignoreOptions, divider.Pulse);
            }
        }

        private ScrollEvent ParseIgnoreOptions([CanBeNull] string options)
        {
            var result = ScrollEvent.None;

            var allOptions = (ScrollEvent[]) Enum.GetValues(typeof(ScrollEvent));
            foreach (var option in allOptions) ParseFlag(ref result, option);

            return result;

            //TODO: Remove boxing
            void ParseFlag(ref ScrollEvent target, ScrollEvent option)
            {
                if (options?.Contains(option.ToString()) ?? false) target |= option;
            }
        }
    }
}
