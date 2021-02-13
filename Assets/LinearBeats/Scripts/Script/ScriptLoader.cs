using System;
using System.Collections.Generic;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace LinearBeats.Script
{
    [Serializable]
    public sealed class ScriptLoader
    {
#pragma warning disable IDE0044
        [Required]
        [SerializeField]
        private Transform _notesHolder = null;
        [Required]
        [SerializeField]
        private GameObject _shortNotePrefab = null;
        [Required]
        [SerializeField]
        private Transform _dividerHolder = null;
        [Required]
        [SerializeField]
        private GameObject _dividerPrefab = null;
        [Required]
        [SerializeField]
        private AudioListener _audioListener = null;
        [Required]
        [SerializeField]
        private AudioMixerGroup[] _audioMixerGroups = null;
#pragma warning restore IDE0044

        public LinearBeatsScript Script { get; private set; }

        private string _resourcesPath;

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
                audioSources[i] = AddAudioChannelToGameObject(audioGameObject, Script.AudioChannels[i]);
            }
            return audioSources;

            GameObject CreateAudioGameObject(string name)
            {
                var audioObject = new GameObject(name);
                audioObject.transform.parent = _audioListener.transform;
                return audioObject;
            }

            AudioSource AddAudioChannelToGameObject(GameObject audioObject, AudioChannel audioChannel)
            {
                AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>(_resourcesPath + audioChannel.FileName);
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = _audioMixerGroups[audioChannel.Layer];
                return audioSource;
            }
        }

        public Queue<RailBehaviour>[] InstantiateNotes()
        {
            var notesBehaviours = new Queue<RailBehaviour>[Script.AudioChannels.Length];
            for (int i = 0; i < Script.AudioChannels.Length; ++i)
            {
                notesBehaviours[i] = new Queue<RailBehaviour>();
                if (Script.AudioChannels[i].Notes != null)
                {
                    foreach (var note in Script.AudioChannels[i].Notes)
                    {
                        GameObject noteObject = GameObject.Instantiate(
                            _shortNotePrefab,
                            GetNotePosition(note),
                            Quaternion.identity,
                            _notesHolder);
                        noteObject.transform.localScale = GetNoteSize(note);

                        NoteBehaviour noteBehaviour = noteObject.AddComponent<NoteBehaviour>();
                        noteBehaviour.Pulse = note.Pulse;
                        noteBehaviour.Note = note;
                        notesBehaviours[i].Enqueue(noteBehaviour);
                    }
                }
            }
            return notesBehaviours;

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

        public Queue<RailBehaviour> InstantiateDividers()
        {
            var dividerBehaviours = new Queue<RailBehaviour>();
            foreach (var divider in Script.Dividers)
            {
                GameObject dividerObject = GameObject.Instantiate(
                    _dividerPrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    _dividerHolder);
                RailBehaviour dividerBehaviour = dividerObject.AddComponent<RailBehaviour>();
                dividerBehaviour.Pulse = divider.Pulse;
                dividerBehaviours.Enqueue(dividerBehaviour);
            }
            return dividerBehaviours;
        }
    }
}