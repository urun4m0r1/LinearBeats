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
    [Serializable]
    public sealed class ScriptLoader
    {
        [Required] [SerializeField] private LeanGameObjectPool notesPool;
        [Required] [SerializeField] private LeanGameObjectPool dividerPool;
        [Required] [SerializeField] private AudioListener audioListener;
        [Required] [SerializeField] private AudioMixerGroup[] audioMixerGroups;

        public LinearBeatsScript Script { get; private set; }

        private string _resourcesPath;

        public void LoadScript(string resourcesPath, string scriptName)
        {
            _resourcesPath = resourcesPath;
            Script = ScriptParser.ParseFromResources(_resourcesPath + scriptName);
        }

        [NotNull]
        public Dictionary<ushort, AudioPlayer> InstantiateAudioSource()
        {
            var dict = new Dictionary<ushort, AudioPlayer>();
            foreach (var audioChannel in Script.AudioChannels ?? throw new ArgumentNullException())
            {
                var audioSource = CreateAudioSource(audioChannel);
                var audioPlayer = new AudioPlayer(audioSource, audioChannel.Offset);
                dict.Add(audioChannel.Channel, audioPlayer);
            }

            return dict;

            AudioSource CreateAudioSource(MediaChannel audioChannel)
            {
                var audioObject = new GameObject(audioChannel.FileName);
                audioObject.transform.parent = audioListener.transform;

                var audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>(_resourcesPath + audioChannel.FileName);
                audioSource.playOnAwake = false;
                audioSource.bypassEffects = true;
                audioSource.bypassListenerEffects = true;
                audioSource.bypassReverbZones = true;
                audioSource.outputAudioMixerGroup = audioMixerGroups[audioChannel.Layer];
                return audioSource;
            }
        }

        public void InstantiateAllNotes(
            [NotNull] TimingObject timingObject,
            [NotNull] Dictionary<ushort, AudioPlayer> audioPlayers)
        {
            if (Script.Notes == null) return;

            for (var i = 0; i < Script.Notes.Length; ++i)
            {
                var note = Script.Notes[i];
                var noteObject = notesPool.Spawn(notesPool.transform);
                var noteBehaviour = noteObject.GetComponent<NoteBehaviour>();

                var ignoreOptions = ParseIgnoreOptions(note.IgnoreScrollEvent);
                noteBehaviour.RailObject = new NoteRail(timingObject, ignoreOptions, note.Trigger);
                noteBehaviour.NoteShape = note.Shape;
                noteBehaviour.Judgement = timingObject.Judgement;
                noteBehaviour.AudioPlayer = audioPlayers[note.Trigger.Channel];

                if (i + 1 == Script.Notes.Length) continue;

                var intervalPulse = Script.Notes[i + 1].Trigger.Pulse - note.Trigger.Pulse;
                noteBehaviour.AudioLength = timingObject.Factory.Create(intervalPulse);
            }
        }

        //TODO: 조금씩 생성시켜 사용하기
        public void InstantiateAllDividers([NotNull] TimingObject timingObject)
        {
            if (Script.Dividers == null) return;

            foreach (var divider in Script.Dividers)
            {
                var dividerObject = dividerPool.Spawn(dividerPool.transform);
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
