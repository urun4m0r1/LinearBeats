#pragma warning disable IDE0051
#pragma warning disable IDE0090

using System.Collections.Generic;
using Lean.Pool;
using LinearBeats.Judgement;
using LinearBeats.Script;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Utils.Unity;

namespace LinearBeats.Game
{
    public sealed class GameManager : SerializedMonoBehaviour
    {
#pragma warning disable IDE0044
        [SerializeField]
        private UnityEvent _gameFinishied = new UnityEvent();
        [Range(0.0001f, 10)]
        [SerializeField]
        private float _meterPerPulse = 0.01f;
        [Range(1, 128)]
        [SerializeField]
        private uint _noteLoadBufferSize = 4;
        [SerializeField]
        private ScriptLoader _scriptLoader = null;
        [OdinSerialize]
        private TimingController _timingController = null;
        [OdinSerialize]
        private NoteJudgement _noteJudgement = null;
#pragma warning restore IDE0044

        private Queue<RailBehaviour> _dividerBehaviours = null;
        private readonly Dictionary<uint, NoteBehaviour> _noteBehaviours = new Dictionary<uint, NoteBehaviour>();
        private AudioSource[] _audioSources = null;
        private AudioSource _backgroundAudioSource = null;

        private uint nextNoteLoadIndex = 0;

        void Start()
        {
            InitScriptLoader();
            InitAudioSources();
            InitTimingController();
            InitGameObjects();
            ResetGame();

            void InitScriptLoader()
            {
                _scriptLoader.LoadScript("Songs/Tutorial/", "Tutorial");
            }

            void InitAudioSources()
            {
                _audioSources = _scriptLoader.InstantiateAudioSource();
                _backgroundAudioSource = _audioSources[0];
            }

            void InitTimingController()
            {
                _timingController.InitTiming(
                    _scriptLoader.Script.Timings,
                    AudioUtils.GetSamplesPerTimes(_audioSources),
                    _scriptLoader.Script.Metadata.PulsesPerQuarterNote,
                    _backgroundAudioSource.clip.samples);
            }

            void InitGameObjects()
            {
                _dividerBehaviours = _scriptLoader.InstantiateDividers();
            }
        }

        public void PlayPauseGame(bool play)
        {
            if (play) StartGame();
            else PauseGame();
        }

        public void StartGame()
        {
            foreach (var audioSource in _audioSources)
            {
                audioSource.UnPause();
            }
        }

        public void PauseGame()
        {
            foreach (var audioSource in _audioSources)
            {
                audioSource.Pause();
            }
        }

        public void ResetGame()
        {
            _gameFinishied.Invoke();

            foreach (var audioSource in _audioSources)
            {
                audioSource.Stop();
                audioSource.Play();
                audioSource.Pause();
            }

            _timingController.ResetTiming();
            nextNoteLoadIndex = 0;
            BufferNotes(_noteLoadBufferSize);
        }

        private void Update()
        {
            if (_backgroundAudioSource.isPlaying)
            {
                _timingController.UpdateTiming(_backgroundAudioSource.timeSamples);
                UpdateNoteJudge();

                if (_noteBehaviours.Count < _noteLoadBufferSize)
                {
                    BufferNotes(_noteLoadBufferSize - (uint)_noteBehaviours.Count);
                }

                void UpdateNoteJudge()
                {
                    List<uint> judgedNoteIndexes = new List<uint>();
                    foreach (var noteBehaviour in _noteBehaviours)
                    {
                        bool noteJudged = _noteJudgement.JudgeNote(noteBehaviour.Value, _timingController.CurrentPulse);
                        if (noteJudged)
                        {
                            judgedNoteIndexes.Add(noteBehaviour.Key);
                        }
                    }
                    foreach (var judgedNoteIndex in judgedNoteIndexes)
                    {
                        LeanPool.Despawn(_noteBehaviours[judgedNoteIndex]);
                        _noteBehaviours.Remove(judgedNoteIndex);
                    }
                }
            }
            else if (_backgroundAudioSource.timeSamples >= _backgroundAudioSource.clip.samples)
            {
                ResetGame();
            }
            UpdateDividerPosition();
            UpdateNotePosition();

            void UpdateDividerPosition()
            {
                foreach (var dividerBehaviour in _dividerBehaviours)
                {
                    dividerBehaviour.UpdateRailPosition(_timingController.CurrentPulse, _meterPerPulse);
                }
            }

            void UpdateNotePosition()
            {
                foreach (var noteBehaviour in _noteBehaviours)
                {
                    noteBehaviour.Value.UpdateRailPosition(_timingController.CurrentPulse, _meterPerPulse);
                }
            }
        }

        private void BufferNotes(uint bufferSize)
        {
            if (nextNoteLoadIndex < _scriptLoader.Script.Notes.Length)
            {
                for (var i = nextNoteLoadIndex; i < nextNoteLoadIndex + bufferSize; ++i)
                {
                    if (!_noteBehaviours.ContainsKey(i))
                    {
                        TryAddNoteBehaviour(i);
                    }
                }
            }
            nextNoteLoadIndex += bufferSize;

            void TryAddNoteBehaviour(uint i)
            {
                if (_scriptLoader.TryInstantiateNote(i, out NoteBehaviour noteBehaviour))
                {
                    _noteBehaviours.Add(i, noteBehaviour);
                }
            }
        }
    }
}
