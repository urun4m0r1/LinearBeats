using System;
using System.Collections.Generic;
using Lean.Pool;
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

            AudioSource AddAudioSourcesToGameObject(GameObject audioObject, AudioChannel audioChannel)
            {
                var audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>(_resourcesPath + audioChannel.FileName);
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = _audioMixerGroups[audioChannel.Layer];
                return audioSource;
            }
        }

        public bool TryInstantiateNote(uint index, out NoteBehaviour noteBehaviour)
        {
            noteBehaviour = null;
            if (index < Script.Notes.Length)
            {
                Note note = Script.Notes[index];
                GameObject noteObject = _notesPool.Spawn(
                    GetNotePosition(note),
                    Quaternion.identity,
                    _notesPool.transform);
                noteObject.transform.localScale = GetNoteSize(note);

                noteBehaviour = noteObject.GetComponent<NoteBehaviour>();
                noteBehaviour.Pulse = note.Pulse;
                noteBehaviour.Note = note;
            }
            return noteBehaviour != null;

            Vector3 GetNotePosition(Note note)
            {
                return new Vector3(GetNoteCol(), GetNoteRow(), 0f);

                float GetNoteCol()
                {
                    return note.PositionCol - 6f;
                }

                float GetNoteRow()
                {
                    return note.PositionRow * 2f;
                }
            }

            Vector3 GetNoteSize(Note note)
            {
                return new Vector3(GetNoteWidth(), GetNoteHeight(), 1f);

                float GetNoteWidth()
                {
                    return note.SizeCol;
                }

                float GetNoteHeight()
                {
                    return note.SizeRow == 1 ? 1 : 20;
                }
            }
        }

        public bool TryInstantiateDivider(uint index, out RailBehaviour dividerBehaviour)
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
                dividerBehaviour.Pulse = divider.Pulse;
            }
            return dividerBehaviour != null;
        }
    }
}
