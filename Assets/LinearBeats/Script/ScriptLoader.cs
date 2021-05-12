using System;
using JetBrains.Annotations;
using Lean.Pool;
using LinearBeats.Scrolling;
using LinearBeats.Time;
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
        public AudioSource[] InstantiateAudioSource()
        {
            if (Script.AudioChannels == null) throw new InvalidOperationException();

            var audioSources = new AudioSource[Script.AudioChannels.Length];
            for (var i = 0; i < audioSources.Length; ++i)
            {
                var audioGameObject = CreateAudioGameObject(Script.AudioChannels[i].FileName);
                audioSources[i] = AddAudioSourcesToGameObject(audioGameObject, Script.AudioChannels[i]);
            }

            return audioSources;

            GameObject CreateAudioGameObject(string name)
            {
                var audioObject = new GameObject(name);
                audioObject.transform.parent = audioListener.transform;
                return audioObject;
            }

            AudioSource AddAudioSourcesToGameObject(GameObject audioObject, MediaChannel audioChannel)
            {
                var audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>(_resourcesPath + audioChannel.FileName);
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = audioMixerGroups[audioChannel.Layer];
                return audioSource;
            }
        }

        public void InstantiateAllNotes(TimingObject timingObject)
        {
            if (Script.Notes == null) return;

            foreach (var note in Script.Notes)
            {
                var noteObject = notesPool.Spawn(notesPool.transform);
                var noteBehaviour = noteObject.GetComponent<NoteBehaviour>();

                var ignoreOptions = ParseIgnoreOptions(note.IgnoreScrollEvent);
                noteBehaviour.RailObject = new NoteRail(timingObject, ignoreOptions, note.Trigger);
                noteBehaviour.NoteShape = note.Shape;
                noteBehaviour.Judgement = timingObject.Judgement;
            }
        }

        //TODO: 조금씩 생성시켜 사용하기
        public void InstantiateAllDividers(TimingObject timingObject)
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
