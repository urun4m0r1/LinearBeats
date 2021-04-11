using System;
using Lean.Pool;
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

        public bool TryInstantiateNote(uint index, out NoteBehaviour noteBehaviour, FixedTimeFactory fixedTimeFactory)
        {
            noteBehaviour = null;
            if (index < Script.Notes.Length)
            {
                Note note = Script.Notes[index];
                GameObject noteObject = _notesPool.Spawn(
                    GetNotePosition(note.Shape),
                    Quaternion.identity,
                    _notesPool.transform);
                noteObject.transform.localScale = GetNoteSize(note.Shape);

                noteBehaviour = noteObject.GetComponent<NoteBehaviour>();
                noteBehaviour.StartTime = fixedTimeFactory.Create(note.Trigger.Pulse);
                noteBehaviour.Duration = fixedTimeFactory.Create(note.Trigger.Duration);
                noteBehaviour.Note = note;
            }
            return noteBehaviour != null;

            Vector3 GetNotePosition(Shape noteShape)
            {
                return new Vector3(GetNoteCol(), GetNoteRow(), 0f);

                float GetNoteCol()
                {
                    return noteShape.PosCol - 6f;
                }

                float GetNoteRow()
                {
                    return noteShape.PosRow * 2f;
                }
            }

            Vector3 GetNoteSize(Shape noteShape)
            {
                return new Vector3(GetNoteWidth(), GetNoteHeight(), 1f);

                float GetNoteWidth()
                {
                    return noteShape.SizeCol;
                }

                float GetNoteHeight()
                {
                    return noteShape.SizeRow == 1 ? 1 : 20;
                }
            }
        }

        public bool TryInstantiateDivider(uint index, out RailBehaviour dividerBehaviour, FixedTimeFactory fixedTimeFactory)
        {
            dividerBehaviour = null;
            if (index < Script.Dividers.Length)
            {
                Divider divider = Script.Dividers[index];
                GameObject dividerObject = _dividerPool.Spawn(
                    Vector3.zero,
                    Quaternion.identity,
                    _dividerPool.transform);

                dividerBehaviour = dividerObject.GetComponent<RailBehaviour>();
                dividerBehaviour.StartTime = fixedTimeFactory.Create(divider.Pulse);
            }
            return dividerBehaviour != null;
        }
    }
}
