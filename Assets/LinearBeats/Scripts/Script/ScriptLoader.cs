#pragma warning disable IDE0090
#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Game;
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
        public AudioListener audioListener = null;
        public VideoPlayer videoPlayer = null;
        public AudioMixerGroup[] audioMixerGroups = null;

        private LinearBeatsScript _script;

        private ulong _currentPulse = 0;
        private int _timingIndex = 0;
        private float[] _samplesPerPulses = null;
        private float[] _sampleTimeOnBpmChanges = null;
        private float _audioFrequency = 0f;
        private readonly string _resourcesPath = "Songs/Tutorial/";

        private readonly List<AudioSource> _audioSources = new List<AudioSource>();
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
            CalculateTimingData();
        }

        private static LinearBeatsScript ParseScriptFromResourcesPath(string path)
        {
            var scriptAsset = Resources.Load(path) as TextAsset;
            return new ScriptParser(scriptAsset.text).Parse();
        }

        private void InstantiateGameObjects()
        {
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
            _audioFrequency = audioSource.clip.frequency;

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
                _dividerBehaviours.Enqueue(dividerBehaviour);
            }

            Vector3 GetDividerPosition(Divider divider)
            {
                return new Vector3(0f, 0f, divider.Pulse * meterPerPulse);
            }
        }

        private void CalculateTimingData()
        {
            Debug.Log("pulsesPerQuarterNote: " + _script.Metadata.PulsesPerQuarterNote);
            Debug.Log("meterPerPulse: " + meterPerPulse + "m/pulse");

            _samplesPerPulses = new float[_script.Timings.Length];
            _sampleTimeOnBpmChanges = new float[_script.Timings.Length];
            for (var i = 0; i < _script.Timings.Length; ++i)
            {
                float timePerQuarterNote = 60f / _script.Timings[i].Bpm;
                float timePerPulse = timePerQuarterNote / _script.Metadata.PulsesPerQuarterNote;
                _samplesPerPulses[i] = _audioFrequency * timePerPulse;
                _sampleTimeOnBpmChanges[i] = _samplesPerPulses[_timingIndex] * _script.Timings[i].Pulse;

                Debug.Log("bpm: " + _script.Timings[i].Bpm);
                Debug.Log("- timePerQuarterNote: " + timePerQuarterNote * 1000 + "ms/quarterNote");
                Debug.Log("- timePerPulse: " + timePerPulse * 1000 + "ms/pulse");
                Debug.Log("- samplesPerPulse: " + _samplesPerPulses[i] + "Hz/pulse");
                Debug.Log("- sampleWhenBpmChanged: " + _sampleTimeOnBpmChanges[i] + "Hz");
            }
            DisplayBpm();
        }

        private void DisplayBpm()
        {
            Debug.Log("currentBpm: " + _script.Timings[_timingIndex].Bpm);
        }


        [DisableInEditorMode]
        [Button("PlayAllAudioSource")]
        public void PlayAllAudioSource()
        {
            _timingIndex = 0;

            foreach (var audioSource in _audioSources)
            {
                audioSource.Play();
            }
        }

        private void Update()
        {
            UpdateCurrentPulse();
            UpdateGameObjectsPosition();
            UpdateNoteJudgement();
        }

        private void UpdateCurrentPulse()
        {
            UpdateTiming();
            float sampleElapsedAfterBpmChanged = _audioSources[0].timeSamples - _sampleTimeOnBpmChanges[_timingIndex];
            ulong pulseElapsedAfterBpmChanged = (ulong)(sampleElapsedAfterBpmChanged / _samplesPerPulses[_timingIndex]);
            _currentPulse = _script.Timings[_timingIndex].Pulse + pulseElapsedAfterBpmChanged;

            void UpdateTiming()
            {
                bool isNextTimingIndexSmallerThanTimingLength = _timingIndex < _script.Timings.Length - 1;
                if (isNextTimingIndexSmallerThanTimingLength)
                {
                    Timing nextTiming = _script.Timings[_timingIndex + 1];
                    if (_currentPulse >= nextTiming.Pulse)
                    {
                        ++_timingIndex;
                        DisplayBpm();
                    }
                }
            }
        }

        private void UpdateGameObjectsPosition()
        {
            UpdateGameObjectsZPosition(_noteBehaviours);
            UpdateGameObjectsZPosition(_dividerBehaviours);

            void UpdateGameObjectsZPosition<T>(Queue<T> objects) where T : RailBehaviour
            {
                foreach (var target in objects)
                {
                    target.SetZPosition(meterPerPulse * (target.Pulse - _currentPulse));
                }
            }
        }

        private void UpdateNoteJudgement()
        {
            foreach (var audioChannel in _script.AudioChannels)
            {
                if (audioChannel.Notes == null) continue;
                foreach (var note in audioChannel.Notes)
                {
                    Judge noteJudgement = InputHandler.JudgeNote(note, _currentPulse);
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
