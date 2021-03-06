#pragma warning disable IDE0051
#pragma warning disable IDE0090

using System.Collections.Generic;
using System.Globalization;
using Lean.Pool;
using LinearBeats.Audio;
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
        [SerializeField] private UnityEvent<string> _onBpmChanged = new UnityEvent<string>();
        [SerializeField] private UnityEvent<float> _onProgressChanged = new UnityEvent<float>();
        [SerializeField] private UnityEvent _onGameReset = new UnityEvent();
        [Range(1f, 100f)] [SerializeField] private float _meterPerQuarterNote = 1f;
        [Range(1f, 1000f)] [SerializeField] private float _standardBpm = 100f;
        [Range(1, 128)] [SerializeField] private uint _noteLoadBufferSize = 4;
        [SerializeField] private ScriptLoader _scriptLoader = null;
        [OdinSerialize] private TimingController _timingController = null;
        [OdinSerialize] private NoteJudgement _noteJudgement = null;
#pragma warning restore IDE0044

        private readonly Dictionary<uint, RailBehaviour> _dividerBehaviours = new Dictionary<uint, RailBehaviour>();
        private readonly Dictionary<uint, NoteBehaviour> _noteBehaviours = new Dictionary<uint, NoteBehaviour>();
        private AudioSource[] _audioSources = null;
        private AudioSource _backgroundAudioSource;
        private FixedTime.Factory _fixedTimeFactory;
        private uint nextNoteLoadIndex = 0;
        private IPositionConverter _positionConverter;

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
                var converter = new TimingConverter(
                    _scriptLoader.Script.Timing.BpmEvents,
                    _audioSources[0].clip.frequency,
                    _scriptLoader.Script.Timing.StandardBpm,
                    _scriptLoader.Script.Timing.StandardPpqn);

                _fixedTimeFactory = new FixedTime.Factory(converter);

                _positionConverter = new PositionConverter.Builder(converter)
                    .SetScrollEvent(ScrollEvent.Stop, _scriptLoader.Script.Scrolling.StopEvents)
                    .SetScrollEvent(ScrollEvent.Jump, _scriptLoader.Script.Scrolling.JumpEvents)
                    .SetScrollEvent(ScrollEvent.BackJump, _scriptLoader.Script.Scrolling.BackJumpEvents)
                    .SetScrollEvent(ScrollEvent.Rewind, _scriptLoader.Script.Scrolling.RewindEvents)
                    .SetScrollEvent(ScrollEvent.Speed, _scriptLoader.Script.Scrolling.SpeedEvents)
                    .SetScrollEvent(ScrollEvent.SpeedBounce, _scriptLoader.Script.Scrolling.SpeedBounceEvents)
                    .SetPositionScaler(ScalerMode.BpmRelative)
                    .SetPositionNormalizer(NormalizerMode.Individual)
                    .Build();

                var audioClipSource = new AudioClipSource(_backgroundAudioSource,
                    _scriptLoader.Script.AudioChannels[0].Offset);

                _timingController = new TimingController(audioClipSource, _fixedTimeFactory);
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

            nextNoteLoadIndex = 0;
            //FIXME: 기존 버퍼 초기화
            BufferNotes(_noteLoadBufferSize);

            _onGameReset.Invoke();
        }

        private void FixedUpdate()
        {
            if (_backgroundAudioSource.isPlaying)
            {
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
            _onProgressChanged.Invoke(_timingController.CurrentProgress);
            _onBpmChanged.Invoke(_timingController.CurrentTime.Bpm.ToString(CultureInfo.InvariantCulture));

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
                    dividerBehaviour.Value.UpdateRailPosition(_positionConverter,
                        _timingController.CurrentTime,
                        _meterPerQuarterNote,
                        (_standardBpm / _scriptLoader.Script.Timing.StandardBpm) ?? 1f);
                }
            }

            void UpdateNotePosition()
            {
                foreach (var noteBehaviour in _noteBehaviours)
                {
                    noteBehaviour.Value.UpdateRailPosition(_positionConverter,
                        _timingController.CurrentTime,
                        _meterPerQuarterNote,
                        (_standardBpm / _scriptLoader.Script.Timing.StandardBpm) ?? 1f);
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
