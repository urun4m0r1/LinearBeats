#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Input;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using Utils.Unity;

namespace LinearBeats.Script
{
    public sealed class ScriptLoader : SerializedMonoBehaviour
    {
        public float meterPerPulse = 0.01f; //TODO: Dynamic resize
        public Transform rail = null;
        public Transform notesHolder = null;
        public GameObject shortNotePrefab = null;
        public Transform dividerHolder = null;
        public GameObject dividerPrefab = null;
        [Required]
        public AudioListener audioListener = null;
        [Required]
        public VideoPlayer videoPlayer = null;
        public AudioMixerGroup[] audioMixerGroups = null;
        private readonly List<AudioSource> audioSources = new List<AudioSource>();
        float timePerQuarterNote = 0f;
        float timePerPulse = 0f;
        float sampleRatePerPulse = 0f;
        float audioFrequency = 0f;
        float currentBpm = 0f;
        readonly string resourcesPath = "Songs/Tutorial/";
        private int timingIndex = 0;

        private ulong currentPulse = 0;
        [SerializeField]
        private readonly bool verbose = false;

        private LinearBeatsScript script;

        private readonly Queue<GameObject> dividerObjects = new Queue<GameObject>();

        private readonly Queue<GameObject> noteObjects = new Queue<GameObject>();
        private readonly Dictionary<Judge, ulong> judgeOffsetTable = new Dictionary<Judge, ulong>
        {
            [Judge.Perfect] = 40,
            [Judge.Great] = 60,
            [Judge.Good] = 100,
            [Judge.Miss] = 130,
            [Judge.Null] = 0,
        };

        private void OnEnable()
        {
            var componentInitializer = new ComponentInitializer(gameObject);
            componentInitializer.TryInitComponent(ref videoPlayer);
            componentInitializer.TryInitComponent(ref audioListener);
        }

        private void Start()
        {
            var scriptAsset = Resources.Load("Songs/Tutorial/Tutorial") as TextAsset;
            script = new ScriptParser(scriptAsset.text).Parse();

            InstantiateDividers();

            foreach (var audioChannel in script.AudioChannels)
            {
                InstantiateAudioSource(audioChannel);
                InstantiateNotes(audioChannel);
            }
        }

        private void InstantiateDividers()
        {
            foreach (var divider in script.Dividers)
            {
                var position = new Vector3(0f, 0f, divider.Pulse * meterPerPulse);
                GameObject dividerObject = Instantiate(dividerPrefab, position, Quaternion.identity, dividerHolder);
                dividerObjects.Enqueue(dividerObject);
            }
        }

        private void InstantiateAudioSource(AudioChannel audioChannel)
        {
            GameObject audioGameObject = CreateAudioGameObject(audioChannel.FileName);
            AudioSource audioSource = AddAudioChannelToGameObject(audioGameObject, audioChannel);
            audioSources.Add(audioSource);
            audioFrequency = audioSource.clip.frequency;
        }

        private GameObject CreateAudioGameObject(string name)
        {
            var audioObject = new GameObject(name);
            audioObject.transform.parent = audioListener.transform;
            return audioObject;
        }

        private AudioSource AddAudioChannelToGameObject(GameObject audioObject, AudioChannel audioChannel)
        {
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = Resources.Load<AudioClip>(resourcesPath + audioChannel.FileName);
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = audioMixerGroups[audioChannel.Layer];
            return audioSource;
        }

        private void InstantiateNotes(AudioChannel audioChannel)
        {
            if (audioChannel.Notes != null)
            {
                foreach (var note in audioChannel.Notes)
                {
                    var position = new Vector3(note.PositionCol - 6f, note.PositionRow, note.Pulse * meterPerPulse);
                    float height = note.SizeRow == 1 ? 0.1f : note.SizeRow;
                    float length = note.PulseDuration == 0 ? 0.1f : note.PulseDuration * meterPerPulse;
                    var scale = new Vector3(note.SizeCol, height, length);

                    GameObject noteObject = Instantiate(shortNotePrefab, position, Quaternion.identity, notesHolder);
                    noteObject.transform.localScale = scale;
                    noteObjects.Enqueue(noteObject);
                }
            }
        }

        [DisableInEditorMode]
        [Button("PlayAllAudioSource")]
        public void PlayAllAudioSource()
        {
            timingIndex = 0;
            currentBpm = script.Metadata.BpmInit;
            CalculateTimingData();

            foreach (var audioSource in audioSources)
            {
                audioSource.Play();
            }
        }

        private void CalculateTimingData()
        {
            timePerQuarterNote = 60f / currentBpm;
            timePerPulse = timePerQuarterNote / script.Metadata.PulsesPerQuarterNote;
            sampleRatePerPulse = audioFrequency * timePerPulse;
            if (verbose)
            {
                Debug.Log("currentBpm: " + currentBpm);
                Debug.Log("pulsesPerQuarterNote: " + script.Metadata.PulsesPerQuarterNote);
                Debug.Log("timePerQuarterNote: " + timePerQuarterNote * 1000 + "ms/quarterNote");
                Debug.Log("timePerPulse: " + timePerPulse * 1000 + "ms/pulse");
                Debug.Log("sampleRatePerPulse: " + sampleRatePerPulse + "Hz/pulse");
                Debug.Log("meterPerPulse: " + meterPerPulse + "m/pulse");
            }
        }

        private void Update()
        {
            UpdateTiming();
            UpdateCurrentPulse();
            UpdateRailPosition();
            UpdateNoteJudgement();
        }

        private void UpdateTiming()
        {
            if (timingIndex < script.Timings.Length)
            {
                var timing = script.Timings[timingIndex];
                if (currentPulse >= timing.Pulse)
                {
                    timingIndex++;
                    currentBpm = timing.Bpm;
                    CalculateTimingData();
                }
            }
        }

        private void UpdateCurrentPulse()
        {
            //FIXME: BPM Jumps
            currentPulse = (ulong)(audioSources[0].timeSamples / sampleRatePerPulse);
        }

        private void UpdateRailPosition()
        {
            float currentMeter = meterPerPulse * currentPulse;
            rail.position = new Vector3(0f, 0f, -currentMeter);
        }

        private void UpdateNoteJudgement()
        {
            foreach (var audioChannel in script.AudioChannels)
            {
                if (audioChannel.Notes == null) continue;
                foreach (var note in audioChannel.Notes)
                {
                    Judge noteJudgement = InputHandler.JudgeNote(note, currentPulse, judgeOffsetTable);
                    DisplayNoteJudgement(note, noteJudgement);
                }
            }
        }

        private void DisplayNoteJudgement(Note note, Judge noteJudgement)
        {
            if (noteJudgement != Judge.Null && noteJudgement != Judge.Miss)
            {
                Debug.Log($"{noteJudgement}Row:{note.PositionRow}, Col:{note.PositionCol} / Note: {note.Pulse}, At: {currentPulse}");
            }
        }
    }
}
