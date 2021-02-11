#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Game;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using Utils.Unity;

namespace LinearBeats.Script
{

    public sealed class ScriptLoader : SerializedMonoBehaviour
    {
        public Transform rail = null;
        public Transform notesHolder = null;
        public GameObject shortNotePrefab = null;
        public Transform dividerHolder = null;
        public GameObject dividerPrefab = null;
        public float meterPerPulse = 0.01f;

        public AudioListener audioListener = null;
        public VideoPlayer videoPlayer = null;

        private readonly string _resourcesPath = "Songs/Tutorial/";
        public LinearBeatsScript Script { get; private set; }
        public TimingController _timingController = null;

        public AudioMixerGroup[] audioMixerGroups = null;


        public Queue<RailBehaviour> DividerBehaviours { get; private set; } = new Queue<RailBehaviour>();
        public Queue<NoteBehaviour> NoteBehaviours { get; private set; } = new Queue<NoteBehaviour>();
        public List<AudioSource> AudioSources { get; private set; } = new List<AudioSource>();

        private void OnEnable()
        {
            var componentInitializer = new ComponentInitializer(gameObject);
            componentInitializer.TryInitComponent(ref videoPlayer);
            componentInitializer.TryInitComponent(ref audioListener);
        }

        private void Start()
        {
            Script = ParseScriptFromResourcesPath("Songs/Tutorial/Tutorial");
            InstantiateGameObjects();

            int[] audioFrequencies = new int[AudioSources.Count];
            for (var i = 0; i < AudioSources.Count; ++i)
            {
                audioFrequencies[i] = AudioSources[i].clip.frequency;
            }
            _timingController.InitiateTimingData(
                Script.Timings,
                audioFrequencies,
                Script.Metadata.PulsesPerQuarterNote);
        }

        private static LinearBeatsScript ParseScriptFromResourcesPath(string path)
        {
            var scriptAsset = Resources.Load(path) as TextAsset;
            return new ScriptParser(scriptAsset.text).Parse();
        }

        private void InstantiateGameObjects()
        {
            //TODO: Object Pooling
            foreach (var audioChannel in Script.AudioChannels)
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
            AudioSources.Add(audioSource);

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
                    GameObject noteObject = GameObject.Instantiate(
                        shortNotePrefab,
                        GetNotePosition(note),
                        Quaternion.identity,
                        notesHolder);
                    noteObject.transform.localScale = GetNoteSize(note);

                    NoteBehaviour noteBehaviour = noteObject.AddComponent<NoteBehaviour>();
                    noteBehaviour.PositionMultiplyer = GetPositionMultiplyerOnPulse(note.Pulse);
                    noteBehaviour.Pulse = note.Pulse;
                    NoteBehaviours.Enqueue(noteBehaviour);
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
            foreach (var timing in Script.Timings)
            {
                if (timing.Pulse <= pulse)
                {
                    positionMultiplyer = timing.Bpm / Script.Timings[0].Bpm;
                }
            }
            return positionMultiplyer;
        }

        private void InstantiateDividers()
        {
            foreach (var divider in Script.Dividers)
            {
                GameObject dividerObject = GameObject.Instantiate(
                    dividerPrefab,
                    GetDividerPosition(divider),
                    Quaternion.identity,
                    dividerHolder);
                RailBehaviour dividerBehaviour = dividerObject.AddComponent<RailBehaviour>();
                dividerBehaviour.Pulse = divider.Pulse;
                dividerBehaviour.PositionMultiplyer = GetPositionMultiplyerOnPulse(divider.Pulse);
                DividerBehaviours.Enqueue(dividerBehaviour);
            }

            Vector3 GetDividerPosition(Divider divider)
            {
                return new Vector3(0f, 0f, divider.Pulse * meterPerPulse);
            }
        }
    }
}
