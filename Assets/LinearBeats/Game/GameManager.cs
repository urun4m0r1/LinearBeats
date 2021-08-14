using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Lean.Pool;
using LinearBeats.Media;
using LinearBeats.Judgement;
using LinearBeats.Controls;
using LinearBeats.Rail;
using LinearBeats.Script;
using LinearBeats.Scrolling;
using LinearBeats.Time;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace LinearBeats.Game
{
    //TODO: 클래스 쪼개기
    public sealed class GameManager : SerializedMonoBehaviour
    {
        [SerializeField, Range(24, 240)] private byte targetFps = 60;

        [SerializeField] private UnityEvent<string> onBpmChanged = new UnityEvent<string>();
        [SerializeField] private UnityEvent<float> onProgressChanged = new UnityEvent<float>();
        [SerializeField] private UnityEvent onGameReset = new UnityEvent();

        [SerializeField, Range(1f, 100f)] private float meterPerQuarterNote = 1f;

        [SerializeField] [CanBeNull] private JudgeRange judgeRange;
        [SerializeField] [CanBeNull] private JudgeEffectSpawner judgeEffectSpawner;
        [SerializeField] [CanBeNull] private LaneBeamContainer laneBeamContainer;

        [SerializeField] [CanBeNull] private LeanGameObjectPool notesPool;
        [SerializeField] [CanBeNull] private LeanGameObjectPool dividerPool;
        [SerializeField] [CanBeNull] private AudioListener audioListener;
        [SerializeField] [CanBeNull] private AudioMixerGroup[] audioMixerGroups;

        [SerializeField] [CanBeNull] private string resourcesPath = "Songs/Tutorial/";
        [SerializeField] [CanBeNull] private string scriptName = "Tutorial";

        [SerializeField] [CanBeNull] private InputManager inputManager;

        [ShowInInspector, ReadOnly] private LinearBeatsScript _script;

        [ShowInInspector, ReadOnly] [CanBeNull] private Dictionary<ushort, IMediaPlayer> _audioPlayers;
        [CanBeNull] private AudioPlayer _backgroundAudioPlayer;

        [ShowInInspector, ReadOnly] [CanBeNull] private TimingInfo _timingInfo;

        [ShowInInspector, ReadOnly] [CanBeNull] private InputReceiver _inputReceiver;

        private void Awake()
        {
            Application.targetFrameRate = targetFps;

            InitScript();
            InitAudioPlayers();
            InitTimingObjects();

            ResetGame();
        }

        private T GetResourceFromFileName<T>([NotNull] string filename) where T : UnityEngine.Object =>
            Resources.Load<T>(resourcesPath + filename);

        private void InitScript()
        {
            if (string.IsNullOrWhiteSpace(resourcesPath) || string.IsNullOrWhiteSpace(scriptName))
                throw new ArgumentNullException();

            var scriptAsset = GetResourceFromFileName<TextAsset>(scriptName);

            if (!scriptAsset || string.IsNullOrWhiteSpace(scriptAsset.text))
                throw new ArgumentNullException();

            var parser = new ScriptParser.Builder(scriptAsset.text)
                .SetNamingConvention(NamingConventionStyle.PascalCase)
                .SetScriptValidator(ScriptValidatorMode.VersionValidator)
                .Build();

            try
            {
                _script = parser.Parse();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void InitAudioPlayers()
        {
            if (!notesPool || !dividerPool || !audioListener ||
                audioMixerGroups == null ||
                string.IsNullOrWhiteSpace(resourcesPath))
                throw new ArgumentNullException();

            var audioLoader = new AudioPlayerLoader(audioListener, audioMixerGroups, GetResourceFromFileName<AudioClip>);

            _audioPlayers = audioLoader.Load(_script.AudioChannels);

            _backgroundAudioPlayer = _audioPlayers[0] as AudioPlayer;
            _backgroundAudioPlayer?.Stop();
        }

        private void InitTimingObjects()
        {
            if (_backgroundAudioPlayer == null || _script.AudioChannels == null || _script.Timing.BpmEvents == null)
                throw new ArgumentNullException();

            var timingConverter = new TimingConverter(
                _script.Timing.StandardBpm,
                _script.Timing.StandardPpqn,
                _backgroundAudioPlayer.SamplesPerSecond,
                _script.Timing.BpmEvents);

            var fixedTimeFactory = new FixedTime.Factory(timingConverter);
            var positionConverter = new PositionConverter.Builder(timingConverter)
                .SetScrollEvent(_script.Scrolling)
                .SetPositionScaler(ScalerMode.BpmRelative)
                .SetPositionNormalizer(NormalizerMode.Individual)
                .Build();

            var audioTimingInfo = new AudioTimingInfo(_backgroundAudioPlayer, fixedTimeFactory);
            var distanceConverter = new DistanceConverter(positionConverter, meterPerQuarterNote);

            if (!judgeRange || !judgeEffectSpawner || !inputManager) throw new ArgumentNullException();

            var pressedReceiver = new InputReceiver(inputManager, Input.GetKeyDown);
            _inputReceiver = new InputReceiver(inputManager, Input.GetKey);

            var noteJudgement = new NoteJudgement(pressedReceiver, judgeRange, judgeEffectSpawner);

            _timingInfo = new TimingInfo(audioTimingInfo, fixedTimeFactory, distanceConverter, noteJudgement);
        }


        public void PlayPauseGame(bool play)
        {
            if (play) ResumeGame();
            else PauseGame();
        }

        private void ResumeGame()
        {
            _backgroundAudioPlayer?.Play();
        }

        private void PauseGame()
        {
            if (_audioPlayers == null) return;

            foreach (var audioPlayer in _audioPlayers) audioPlayer.Value.Pause();
        }

        public void ResetGame()
        {
            var timingObjectLoader = new TimingObjectLoader(_timingInfo, notesPool, dividerPool);

            if (_audioPlayers == null) return;

            foreach (var audioPlayer in _audioPlayers) audioPlayer.Value.Stop();

            if (timingObjectLoader == null || _audioPlayers == null)
                throw new InvalidOperationException();

            foreach (var divider in _script.Dividers) timingObjectLoader.InstantiateDivider(divider);

            for (var i = 0; i < _script.Notes.Length; ++i)
            {
                var current = _script.Notes[i];
                var next = _script.Notes[i == _script.Notes.Length - 1 ? i : i + 1];

                timingObjectLoader.InstantiateNote(current, next, _audioPlayers);
            }

            onGameReset.Invoke();
        }

        private void Update()
        {
            if (!laneBeamContainer || _inputReceiver == null) return;

            laneBeamContainer.UpdateAll(_inputReceiver);

            if (_timingInfo == null || _backgroundAudioPlayer == null) return;

            _timingInfo.Converter.DistancePerQuarterNote = meterPerQuarterNote;

            onProgressChanged.Invoke(_backgroundAudioPlayer.Progress);
            onBpmChanged.Invoke(_timingInfo.AudioTimingInfo.Current.Bpm.ToString(CultureInfo.InvariantCulture));

            if (_backgroundAudioPlayer.Progress >= 1f) ResetGame();
        }
    }
}
