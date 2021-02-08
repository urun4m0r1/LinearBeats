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
        float[] samplesPerPulse = null;
        float[] sampleWhenBpmChanged = null;
        float audioFrequency = 0f;
        readonly string resourcesPath = "Songs/Tutorial/";
        private int timingIndex = 0;

        private ulong currentPulse = 0;

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
            script = ParseScriptFromResourcesPath("Songs/Tutorial/Tutorial");

            CalculateTimingData();
            InstantiateGameObjects();
        }

        private static LinearBeatsScript ParseScriptFromResourcesPath(string path)
        {
            var scriptAsset = Resources.Load(path) as TextAsset;
            return new ScriptParser(scriptAsset.text).Parse();
        }

        private void CalculateTimingData()
        {
            Debug.Log("pulsesPerQuarterNote: " + script.Metadata.PulsesPerQuarterNote);
            Debug.Log("meterPerPulse: " + meterPerPulse + "m/pulse");

            samplesPerPulse = new float[script.Timings.Length];
            sampleWhenBpmChanged = new float[script.Timings.Length];
            for (var i = 0; i < script.Timings.Length; ++i)
            {
                float timePerQuarterNote = 60f / script.Timings[i].Bpm;
                float timePerPulse = timePerQuarterNote / script.Metadata.PulsesPerQuarterNote;
                samplesPerPulse[i] = audioFrequency * timePerPulse;
                sampleWhenBpmChanged[i] = samplesPerPulse[timingIndex] * script.Timings[i].Pulse;

                Debug.Log("bpm: " + script.Timings[i].Bpm);
                Debug.Log("- timePerQuarterNote: " + timePerQuarterNote * 1000 + "ms/quarterNote");
                Debug.Log("- timePerPulse: " + timePerPulse * 1000 + "ms/pulse");
                Debug.Log("- samplesPerPulse: " + samplesPerPulse[i] + "Hz/pulse");
                Debug.Log("- sampleWhenBpmChanged: " + sampleWhenBpmChanged[i] + "Hz");
            }
            DisplayBpm();
        }

        private void DisplayBpm()
        {
            Debug.Log("currentBpm: " + script.Timings[timingIndex].Bpm);
        }

        private void InstantiateGameObjects()
        {
            foreach (var audioChannel in script.AudioChannels)
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
            audioSources.Add(audioSource);
            audioFrequency = audioSource.clip.frequency;

            GameObject CreateAudioGameObject(string name)
            {
                var audioObject = new GameObject(name);
                audioObject.transform.parent = audioListener.transform;
                return audioObject;
            }

            AudioSource AddAudioChannelToGameObject(GameObject audioObject, AudioChannel audioChannel)
            {
                AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>(resourcesPath + audioChannel.FileName);
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
                    noteObjects.Enqueue(noteObject);
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

        private void InstantiateDividers()
        {
            foreach (var divider in script.Dividers)
            {
                GameObject dividerObject = Instantiate(
                    dividerPrefab,
                    GetDividerPosition(divider),
                    Quaternion.identity,
                    dividerHolder);
                dividerObjects.Enqueue(dividerObject);
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
            timingIndex = 0;

            foreach (var audioSource in audioSources)
            {
                audioSource.Play();
            }
        }

        private void Update()
        {
            UpdateCurrentPulse();
            UpdateRailPosition();
            UpdateNoteJudgement();
        }

        private void UpdateCurrentPulse()
        {
            UpdateTiming();
            float sampleElapsedAfterBpmChanged = audioSources[0].timeSamples - sampleWhenBpmChanged[timingIndex];
            ulong pulseElapsedAfterBpmChanged = (ulong)(sampleElapsedAfterBpmChanged / samplesPerPulse[timingIndex]);
            currentPulse = script.Timings[timingIndex].Pulse + pulseElapsedAfterBpmChanged;

            void UpdateTiming()
            {
                bool isNextTimingIndexSmallerThanTimingLength = timingIndex < script.Timings.Length - 1;
                if (isNextTimingIndexSmallerThanTimingLength)
                {
                    Timing nextTiming = script.Timings[timingIndex + 1];
                    if (currentPulse >= nextTiming.Pulse)
                    {
                        ++timingIndex;
                        DisplayBpm();
                    }
                }
            }
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

            static void DisplayNoteJudgement(Note note, Judge noteJudgement)
            {
                if (noteJudgement != Judge.Null && noteJudgement != Judge.Miss)
                {
                    Debug.Log($"{noteJudgement}Row:{note.PositionRow}, Col:{note.PositionCol} " +
                    "/ Note: {note.Pulse}, At: {currentPulse}");
                }
            }
        }
    }
}
