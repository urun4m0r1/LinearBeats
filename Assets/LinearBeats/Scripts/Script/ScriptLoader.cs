using System;
using System.Collections.Generic;
using LinearBeats.Visuals;
using UnityEngine;
using UnityEngine.Audio;

namespace LinearBeats.Script
{
    [Serializable]
    public sealed class ScriptLoader
    {
        public LinearBeatsScript Script { get; private set; }

        private readonly string _resourcesPath;

        public ScriptLoader(string resourcesPath)
        {
            _resourcesPath = resourcesPath;
        }

        public void LoadScript(string scriptPath)
        {
            Script = ScriptParser.ParseFromResources(_resourcesPath + scriptPath);
        }

        public AudioSource[] InstantiateAudioSource(AudioMixerGroup[] audioMixerGroups, Transform audioHolder)
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
                audioObject.transform.parent = audioHolder;
                return audioObject;
            }

            AudioSource AddAudioChannelToGameObject(GameObject audioObject, AudioChannel audioChannel)
            {
                AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>(_resourcesPath + audioChannel.FileName);
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = audioMixerGroups[audioChannel.Layer];
                return audioSource;
            }
        }

        public Queue<NoteBehaviour> InstantiateNotes(GameObject shortNotePrefab, Transform notesHolder)
        {
            var noteBehaviours = new Queue<NoteBehaviour>();
            foreach (var audioChannel in Script.AudioChannels)
            {
                if (audioChannel.Notes != null)
                {
                    foreach (var note in audioChannel.Notes)
                    {
                        GameObject noteObject = GameObject.Instantiate(
                            shortNotePrefab,
                            GetNotePosition(note),
                            Quaternion.identity,
                            notesHolder);
                        noteObject.transform.localScale = GetNoteSize(note);

                        NoteBehaviour noteBehaviour = noteObject.AddComponent<NoteBehaviour>();
                        noteBehaviour.PositionMultiplyer = GetPositionMultiplyerOnPulse(note.Pulse);
                        noteBehaviour.Pulse = note.Pulse;
                        noteBehaviours.Enqueue(noteBehaviour);
                    }
                }
            }
            return noteBehaviours;

            Vector3 GetNotePosition(Note note)
            {
                return new Vector3(GetNoteCol(), GetNoteRow(), 0f);

                float GetNoteCol()
                {
                    return note.PositionCol - 6f;
                }

                float GetNoteRow()
                {
                    return note.PositionRow;
                }
            }

            Vector3 GetNoteSize(Note note)
            {
                return new Vector3(GetNoteWidth(), GetNoteHeight(), 0f);

                float GetNoteWidth()
                {
                    return note.SizeCol;
                }

                float GetNoteHeight()
                {
                    return note.SizeRow == 1 ? 0.1f : note.SizeRow;
                }
            }
        }

        public Queue<RailBehaviour> InstantiateDividers(GameObject dividerPrefab, Transform dividerHolder)
        {
            var dividerBehaviours = new Queue<RailBehaviour>();
            foreach (var divider in Script.Dividers)
            {
                GameObject dividerObject = GameObject.Instantiate(
                    dividerPrefab,
                    new Vector3(0f, 0f, 0f),
                    Quaternion.identity,
                    dividerHolder);
                RailBehaviour dividerBehaviour = dividerObject.AddComponent<RailBehaviour>();
                dividerBehaviour.Pulse = divider.Pulse;
                dividerBehaviour.PositionMultiplyer = GetPositionMultiplyerOnPulse(divider.Pulse);
                dividerBehaviours.Enqueue(dividerBehaviour);
            }
            return dividerBehaviours;
        }

        private float GetPositionMultiplyerOnPulse(ulong pulse)
        {
            float positionMultiplyer = 0f;
            foreach (var timing in Script.Timings)
            {
                if (timing.Pulse <= pulse)
                {
                    positionMultiplyer = timing.Bpm / Script.Timings[0].Bpm;
                }
            }
            return positionMultiplyer;
        }
    }
}