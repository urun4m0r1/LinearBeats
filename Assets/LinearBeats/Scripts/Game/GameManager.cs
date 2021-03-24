#pragma warning disable IDE0051
#pragma warning disable IDE0090

using System.Collections.Generic;
using Lean.Pool;
using LinearBeats.Judgement;
using LinearBeats.Script;
using LinearBeats.Time;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using Utils.Extensions;

namespace LinearBeats.Game
{
    public sealed class GameManager : SerializedMonoBehaviour
    {
#pragma warning disable IDE0044
        [SerializeField]
        private UnityEvent _onGameReset = new UnityEvent();
        [Range(1f, 100f)]
        [SerializeField]
        private float _meterPerSecond = 1f;
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

        private readonly Dictionary<uint, RailBehaviour> _dividerBehaviours = new Dictionary<uint, RailBehaviour>();
        private readonly Dictionary<uint, NoteBehaviour> _noteBehaviours = new Dictionary<uint, NoteBehaviour>();
        private AudioSource[] _audioSources = null;
        private AudioSource _backgroundAudioSource = null;
        private FixedTimeFactory _fixedTimeFactory;
        private uint nextNoteLoadIndex = 0;

        void Start()
        {
            InitScriptLoader();
            InitAudioSources();
            InitTimingController();

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
                TimingConverter converter = new TimingConverter(
                    _scriptLoader.Script.Timing,
                    _audioSources[0].clip.frequency);

                _fixedTimeFactory = new FixedTimeFactory(converter);

                _timingController.InitTiming(
                    _fixedTimeFactory.Create((Sample)_backgroundAudioSource.clip.samples),
                    _fixedTimeFactory.Create(_scriptLoader.Script.AudioChannels[0].Offset));
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
            foreach (var audioSource in _audioSources)
            {
                audioSource.Reset();
            }

            _timingController.ResetTiming();

            nextNoteLoadIndex = 0;
            //FIXME: 기존 버퍼 초기화
            BufferNotes(_noteLoadBufferSize);

            _onGameReset.Invoke();
        }

        private void FixedUpdate()
        {
            if (_backgroundAudioSource.isPlaying)
            {
                _timingController.UpdateTiming(_fixedTimeFactory.Create((Sample)_backgroundAudioSource.timeSamples));
                UpdateNoteJudge();
            }

            void UpdateNoteJudge()
            {
                var judgedNoteIndexes = new List<uint>();
                foreach (var noteBehaviour in _noteBehaviours)
                {
                    bool noteJudged = _noteJudgement.JudgeNote(
                        noteBehaviour.Value,
                        _timingController.CurrentTime);

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
        private void Update()
        {
            if (_backgroundAudioSource.isPlaying)
            {
                if (_noteBehaviours.Count < _noteLoadBufferSize)
                {
                    BufferNotes(_noteLoadBufferSize - (uint)_noteBehaviours.Count);
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
                    dividerBehaviour.Value.UpdateRailPosition(_timingController.CurrentTime, _meterPerSecond);
                }
            }

            void UpdateNotePosition()
            {
                foreach (var noteBehaviour in _noteBehaviours)
                {
                    noteBehaviour.Value.UpdateRailPosition(_timingController.CurrentTime, _meterPerSecond);
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
                    if (!_dividerBehaviours.ContainsKey(i))
                    {
                        TryAddDividerBehaviour(i);
                    }
                }
            }
            nextNoteLoadIndex += bufferSize;

            void TryAddNoteBehaviour(uint i)
            {
                if (_scriptLoader.TryInstantiateNote(i, out NoteBehaviour noteBehaviour, _fixedTimeFactory))
                {
                    _noteBehaviours.Add(i, noteBehaviour);
                }
            }

            void TryAddDividerBehaviour(uint i)
            {
                if (_scriptLoader.TryInstantiateDivider(i, out RailBehaviour dividerBehaviour, _fixedTimeFactory))
                {
                    _dividerBehaviours.Add(i, dividerBehaviour);
                }
            }
        }
    }
}
