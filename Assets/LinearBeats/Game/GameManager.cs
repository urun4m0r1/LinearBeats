using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Lean.Pool;
using LinearBeats.Audio;
using LinearBeats.Judgement;
using LinearBeats.Script;
using LinearBeats.Scrolling;
using LinearBeats.Time;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace LinearBeats.Game
{
    public sealed class GameManager : SerializedMonoBehaviour
    {
        [SerializeField] private UnityEvent<string> onBpmChanged = new UnityEvent<string>();
        [SerializeField] private UnityEvent<float> onProgressChanged = new UnityEvent<float>();
        [SerializeField] private UnityEvent onGameReset = new UnityEvent();

        [SerializeField, Range(1f, 100f)] private float meterPerQuarterNote = 1f;

        [SerializeField] [CanBeNull] private JudgeRange judgeRange;
        [SerializeField] [CanBeNull] private LaneEffect laneEffect;

        [SerializeField] [CanBeNull] private LeanGameObjectPool notesPool;
        [SerializeField] [CanBeNull] private LeanGameObjectPool dividerPool;
        [SerializeField] [CanBeNull] private AudioListener audioListener;
        [SerializeField] [CanBeNull] private AudioMixerGroup[] audioMixerGroups;

        [SerializeField] [CanBeNull] private string resourcesPath = "Songs/Tutorial/";
        [SerializeField] [CanBeNull] private string scriptName = "Tutorial";

        [ShowInInspector, ReadOnly] private LinearBeatsScript _script;
        [ShowInInspector, ReadOnly] [CanBeNull] private TimingObject _timingObject;
        [ShowInInspector, ReadOnly] [CanBeNull] private TimingObjectLoader _timingObjectLoader;
        [ShowInInspector, ReadOnly] [CanBeNull] private Dictionary<ushort, AudioPlayer> _audioPlayers;
        [ShowInInspector, ReadOnly] [CanBeNull] private AudioPlayer _backgroundAudioPlayer;
        [ShowInInspector, ReadOnly] [CanBeNull] private AudioTimingInfo _audioTimingInfo;
        [ShowInInspector, ReadOnly] [CanBeNull] private IDistanceConverter _distanceConverter;

        private void Awake()
        {
            InitScript();
            InitAudioPlayers();
            InitTimingObjects();

            ResetGame();
        }

        private void InitScript()
        {
            if (string.IsNullOrWhiteSpace(resourcesPath) || string.IsNullOrWhiteSpace(scriptName))
                throw new ArgumentNullException();

            var scriptAsset = Resources.Load(resourcesPath + scriptName) as TextAsset;

            if (!scriptAsset || string.IsNullOrWhiteSpace(scriptAsset.text))
                throw new ArgumentNullException();

            var parser = new ScriptParser.Builder(scriptAsset.text)
                .SetNamingConvention(NamingConventionStyle.PascalCase)
                .SetScriptValidator(ScriptValidatorMode.Standard)
                .Build();

            try
            {
                _script = parser.Parse();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Application.Quit();
            }
        }

        private void InitAudioPlayers()
        {
            if (!notesPool || !dividerPool || !audioListener ||
                audioMixerGroups == null ||
                string.IsNullOrWhiteSpace(resourcesPath))
                throw new ArgumentNullException();

            var audioLoader = new AudioLoader(audioListener, audioMixerGroups);

            _audioPlayers = audioLoader.InstantiateAudioSources(_script.AudioChannels, resourcesPath);

            _backgroundAudioPlayer = _audioPlayers[0];
        }

        private void InitTimingObjects()
        {
            if (_backgroundAudioPlayer == null || _script.AudioChannels == null || _script.Timing.BpmEvents == null)
                throw new ArgumentNullException();

            var audioClipSource = new AudioClipSource(
                _backgroundAudioPlayer.AudioSource,
                _script.AudioChannels[0].Offset);

            var timingConverter = new TimingConverter(
                _script.Timing.BpmEvents,
                audioClipSource.SamplesPerSecond,
                _script.Timing.StandardBpm,
                _script.Timing.StandardPpqn);

            var fixedTimeFactory = new FixedTime.Factory(timingConverter);
            var positionConverter = new PositionConverter.Builder(timingConverter)
                .SetScrollEvent(_script.Scrolling, ScrollEvent.All)
                .SetPositionScaler(ScalerMode.BpmRelative)
                .SetPositionNormalizer(NormalizerMode.Individual)
                .Build();

            _audioTimingInfo = new AudioTimingInfo(audioClipSource, fixedTimeFactory);
            _distanceConverter = new DistanceConverter(positionConverter, meterPerQuarterNote);

            if (!judgeRange || !laneEffect) throw new ArgumentNullException();

            var noteJudgement = new NoteJudgement(judgeRange, laneEffect);

            _timingObject = new TimingObject(fixedTimeFactory, _distanceConverter, noteJudgement);

            _timingObjectLoader = new TimingObjectLoader(_timingObject, notesPool, dividerPool);
        }


        public void PlayPauseGame(bool play)
        {
            if (play) StartGame();
            else PauseGame();
        }

        private void StartGame()
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
            if (_audioPlayers == null) return;

            foreach (var audioPlayer in _audioPlayers) audioPlayer.Value.Stop();

            if (_timingObjectLoader == null || _audioPlayers == null)
                throw new InvalidOperationException();

            foreach (var divider in _script.Dividers) _timingObjectLoader.InstantiateDivider(divider);

            for (var i = 0; i < _script.Notes.Length; ++i)
            {
                var current = _script.Notes[i];
                var next = _script.Notes[i == _script.Notes.Length - 1 ? i : i + 1];

                _timingObjectLoader.InstantiateNote(current, next, _audioPlayers);
            }

            onGameReset.Invoke();
        }

        private void Update()
        {
            if (_audioTimingInfo?.Progress >= 1f) ResetGame();

            if (_timingObject != null) _timingObject.Current = _audioTimingInfo?.Now ?? default;
            if (_distanceConverter != null) _distanceConverter.DistancePerQuarterNote = meterPerQuarterNote;

            onProgressChanged.Invoke(_audioTimingInfo?.Progress ?? 0f);
            onBpmChanged.Invoke(_timingObject?.Current.Bpm.ToString(CultureInfo.InvariantCulture) ?? "000");
        }
    }
}
