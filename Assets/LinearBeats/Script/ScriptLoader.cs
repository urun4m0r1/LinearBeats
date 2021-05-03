using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lean.Pool;
using LinearBeats.Scrolling;
using LinearBeats.Time;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace LinearBeats.Script
{
    [Serializable]
    public sealed class ScriptLoader
    {
#pragma warning disable IDE0044
        [Required]
        [SerializeField]
        private LeanGameObjectPool _notesPool = null;
        [Required]
        [SerializeField]
        private LeanGameObjectPool _dividerPool = null;
        [Required]
        [SerializeField]
        private AudioListener _audioListener = null;
        [Required]
        [SerializeField]
        private AudioMixerGroup[] _audioMixerGroups = null;
#pragma warning restore IDE0044

        public LinearBeatsScript Script { get; private set; }

        private string _resourcesPath = null;

        public void LoadScript(string resourcesPath, string scriptName)
        {
            _resourcesPath = resourcesPath;
            Script = ScriptParser.ParseFromResources(_resourcesPath + scriptName);
        }

        public AudioSource[] InstantiateAudioSource()
        {
            var audioSources = new AudioSource[Script.AudioChannels.Length];
            for (var i = 0; i < audioSources.Length; ++i)
            {
                GameObject audioGameObject = CreateAudioGameObject(Script.AudioChannels[i].FileName);
                audioSources[i] = AddAudioSourcesToGameObject(audioGameObject, Script.AudioChannels[i]);
            }
            return audioSources;

            GameObject CreateAudioGameObject(string name)
            {
                var audioObject = new GameObject(name);
                audioObject.transform.parent = _audioListener.transform;
                return audioObject;
            }

            AudioSource AddAudioSourcesToGameObject(GameObject audioObject, MediaChannel audioChannel)
            {
                var audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>(_resourcesPath + audioChannel.FileName);
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = _audioMixerGroups[audioChannel.Layer];
                return audioSource;
            }
        }

        public bool TryInstantiateNote(uint index, out NoteBehaviour noteBehaviour,
            FixedTime.Factory fixedTimeFactory,
            DistanceConverter converter)
        {
            noteBehaviour = null;
            if (index < Script.Notes.Length)
            {
                Note note = Script.Notes[index];
                GameObject noteObject = _notesPool.Spawn(_notesPool.transform);

                noteBehaviour = noteObject.GetComponent<NoteBehaviour>();

                var rail = new RailObject(converter,
                    fixedTimeFactory.Create(note.Trigger.Pulse),
                    fixedTimeFactory.Create(note.Trigger.Duration),
                    ParseIgnoreOptions(note.IgnoreTimingEvent ?? ""));

                noteBehaviour.RailObject = rail;
                noteBehaviour.Note = note;
            }
            return noteBehaviour != null;

        }

        public bool TryInstantiateDivider(uint index, out RailBehaviour dividerBehaviour,
            FixedTime.Factory fixedTimeFactory,
            DistanceConverter converter)
        {
            dividerBehaviour = null;
            if (index < Script.Dividers.Length)
            {
                Divider divider = Script.Dividers[index];
                GameObject dividerObject = _notesPool.Spawn(_dividerPool.transform);

                dividerBehaviour = dividerObject.GetComponent<RailBehaviour>();

                var rail = new RailObject(converter,
                    fixedTimeFactory.Create(divider.Pulse),
                    fixedTimeFactory.Create(new Pulse(0f)),
                    ParseIgnoreOptions(divider.TimingEventIgnore ?? ""));

                dividerBehaviour.RailObject = rail;
            }
            return dividerBehaviour != null;
        }

        [NotNull]
        private ScrollEvent ParseIgnoreOptions([NotNull] string options)
        {
            var result = ScrollEvent.None;

            var allOptions = (ScrollEvent[]) Enum.GetValues(typeof(ScrollEvent));
            foreach (var option in allOptions) ParseFlag(ref result, option);

            return result;

            //TODO: Remove boxing
            void ParseFlag(ref ScrollEvent target, ScrollEvent option) =>
                target = options.Contains(option.ToString()) ? target | option : target;
        }
    }
}
