#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System;
using System.Collections.Generic;
using LinearBeats.Game;
using LinearBeats.Input;
using Sirenix.OdinInspector;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using Utils.Unity;

namespace LinearBeats.Script
{

    public sealed class ScriptLoader : SerializedMonoBehaviour
    {
        public float meterPerPulse = 0.01f;
        public Transform rail = null;
        public Transform notesHolder = null;
        public GameObject shortNotePrefab = null;
        public Transform dividerHolder = null;
        public GameObject dividerPrefab = null;
        public AudioListener audioListener = null;
        public VideoPlayer videoPlayer = null;

        private readonly string _resourcesPath = "Songs/Tutorial/";
        private LinearBeatsScript _script;

        private TimingController _timingController = null;

        private readonly List<AudioSource> _audioSources = new List<AudioSource>();
        public AudioMixerGroup[] audioMixerGroups = null;


        private readonly Queue<RailBehaviour> _dividerBehaviours = new Queue<RailBehaviour>();
        private readonly Queue<NoteBehaviour> _noteBehaviours = new Queue<NoteBehaviour>();

        private void OnEnable()
        {
            var componentInitializer = new ComponentInitializer(gameObject);
            componentInitializer.TryInitComponent(ref videoPlayer);
            componentInitializer.TryInitComponent(ref audioListener);
        }

        private void Start()
        {
            _script = ParseScriptFromResourcesPath("Songs/Tutorial/Tutorial");
            InstantiateGameObjects();

            int[] audioFrequencies = new int[_audioSources.Count];
            for (var i = 0; i < _audioSources.Count; ++i)
            {
                audioFrequencies[i] = _audioSources[i].clip.frequency;
            }
            _timingController = new TimingController(_script.Timings, audioFrequencies, _script.Metadata.PulsesPerQuarterNote);
        }

        private static LinearBeatsScript ParseScriptFromResourcesPath(string path)
        {
            var scriptAsset = Resources.Load(path) as TextAsset;
            return new ScriptParser(scriptAsset.text).Parse();
        }

        private void InstantiateGameObjects()
        {
            //TODO: Object Pooling
            foreach (var audioChannel in _script.AudioChannels)
            {
                InstantiateAudioSource(audioChannel);
                InstantiateNotes(audioChannel);
            }

            InstantiateDividers();
        }

        private void InstantiateAudioSource(AudioChannel audioChannel)
        {
            GameObject audioGameObject = CreateAudioGameObject(audioChannel.FileName);
            AudioSource audioSource = AddAudioChannelToGameObject(audioGameObject, audioChannel);
            _audioSources.Add(audioSource);

            GameObject CreateAudioGameObject(string name)
            {
                var audioObject = new GameObject(name);
                audioObject.transform.parent = audioListener.transform;
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

        private void InstantiateNotes(AudioChannel audioChannel)
        {
            if (audioChannel.Notes != null)
            {
                foreach (var note in audioChannel.Notes)
                {
                    GameObject noteObject = Instantiate(
                        shortNotePrefab,
                        GetNotePosition(note),
                        Quaternion.identity,
                        notesHolder);
                    noteObject.transform.localScale = GetNoteSize(note);

                    NoteBehaviour noteBehaviour = noteObject.AddComponent<NoteBehaviour>();
                    noteBehaviour.PositionMultiplyer = GetPositionMultiplyerOnPulse(note.Pulse);
                    noteBehaviour.Pulse = note.Pulse;
                    _noteBehaviours.Enqueue(noteBehaviour);
                }
            }

            Vector3 GetNotePosition(Note note)
            {
                return new Vector3(GetNoteCol(), GetNoteRow(), GetNotePos());

                float GetNoteCol()
                {
                    return note.PositionCol - 6f;
                }

                float GetNoteRow()
                {
                    return note.PositionRow;
                }

                float GetNotePos()
                {
                    return note.Pulse * meterPerPulse;
                }
            }

            Vector3 GetNoteSize(Note note)
            {
                return new Vector3(GetNoteWidth(), GetNoteHeight(), GetNoteLength());

                float GetNoteWidth()
                {
                    return note.SizeCol;
                }

                float GetNoteHeight()
                {
                    return note.SizeRow == 1 ? 0.1f : note.SizeRow;
                }

                float GetNoteLength()
                {
                    return note.PulseDuration == 0 ? 0.1f : note.PulseDuration * meterPerPulse;
                }
            }
        }

        private float GetPositionMultiplyerOnPulse(ulong pulse)
        {
            float positionMultiplyer = 0f;
            foreach (var timing in _script.Timings)
            {
                if (timing.Pulse <= pulse)
                {
                    positionMultiplyer = timing.Bpm / _script.Timings[0].Bpm;
                }
            }
            return positionMultiplyer;
        }

        private void InstantiateDividers()
        {
            foreach (var divider in _script.Dividers)
            {
                GameObject dividerObject = Instantiate(
                    dividerPrefab,
                    GetDividerPosition(divider),
                    Quaternion.identity,
                    dividerHolder);
                RailBehaviour dividerBehaviour = dividerObject.AddComponent<RailBehaviour>();
                dividerBehaviour.Pulse = divider.Pulse;
                dividerBehaviour.PositionMultiplyer = GetPositionMultiplyerOnPulse(divider.Pulse);
                _dividerBehaviours.Enqueue(dividerBehaviour);
            }

            Vector3 GetDividerPosition(Divider divider)
            {
                return new Vector3(0f, 0f, divider.Pulse * meterPerPulse);
            }
        }


        [DisableInEditorMode]
        [Button("PlayAllAudioSource")]
        public void PlayAllAudioSource()
        {
            _timingController.ResetTiming();

            foreach (var audioSource in _audioSources)
            {
                audioSource.Play();
            }
        }

        private void Update()
        {
            OnUpdate(_timingController, _script.AudioChannels);
        }

        private void OnUpdate(TimingController timingController, AudioChannel[] audioChannels)
        {
            timingController.UpdateCurrentPulse(_audioSources[0].timeSamples);
            RailScroll.UpdateRailPosition(_noteBehaviours, timingController.CurrentPulse, meterPerPulse);
            RailScroll.UpdateRailPosition(_dividerBehaviours, timingController.CurrentPulse, meterPerPulse);

            foreach (var audioChannel in audioChannels)
            {
                if (audioChannel.Notes == null) continue;
                NoteJudgement.UpdateNoteJudgement(audioChannel.Notes, timingController.CurrentPulse);
            }
        }
    }
}
