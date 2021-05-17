using System;
using System.Globalization;
using JetBrains.Annotations;
using LinearBeats.Audio;
using LinearBeats.Judgement;
using LinearBeats.Script;
using LinearBeats.Scrolling;
using LinearBeats.Time;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace LinearBeats.Game
{
    public sealed class GameManager : SerializedMonoBehaviour
    {
        [SerializeField] private UnityEvent<string> onBpmChanged = new UnityEvent<string>();
        [SerializeField] private UnityEvent<float> onProgressChanged = new UnityEvent<float>();
        [SerializeField] private UnityEvent onGameReset = new UnityEvent();
        [Range(1f, 100f)] [SerializeField] private float meterPerQuarterNote = 1f;

        [SerializeField] [CanBeNull] private ScriptLoader scriptLoader;
        [SerializeField] [CanBeNull] private JudgeRange judgeRange;
        [SerializeField] [CanBeNull] private LaneEffect laneEffect;

        // ReSharper disable NotNullMemberIsNotInitialized
        [NotNull] private AudioPlayer[] _audioPlayers;
        [NotNull] private AudioPlayer _backgroundAudioPlayer;
        [NotNull] private IDistanceConverter _distanceConverter;
        [NotNull] private AudioTimingInfo _audioTimingInfo;
        [NotNull] private TimingObject _timingObject;
        // ReSharper restore NotNullMemberIsNotInitialized

        private void Awake()
        {
            scriptLoader.LoadScript("Songs/Tutorial/", "Tutorial");

            _audioPlayers = scriptLoader.InstantiateAudioSource();
            _backgroundAudioPlayer = _audioPlayers[0];

            if (scriptLoader.Script.AudioChannels == null) throw new InvalidOperationException();
            if (scriptLoader.Script.Timing.BpmEvents == null) throw new InvalidOperationException();

            var audioClipSource = new AudioClipSource(
                _backgroundAudioPlayer.AudioSource,
                scriptLoader.Script.AudioChannels[0].Offset);

            var timingConverter = new TimingConverter(
                scriptLoader.Script.Timing.BpmEvents,
                audioClipSource.SamplesPerSecond,
                scriptLoader.Script.Timing.StandardBpm,
                scriptLoader.Script.Timing.StandardPpqn);

            var fixedTimeFactory = new FixedTime.Factory(timingConverter);

            var positionConverter = new PositionConverter.Builder(timingConverter)
                .SetScrollEvent(scriptLoader.Script.Scrolling, ScrollEvent.SpeedBounce)
                .SetPositionScaler(ScalerMode.BpmRelative)
                .SetPositionNormalizer(NormalizerMode.Individual)
                .Build();

            var noteJudgement = new NoteJudgement(judgeRange, laneEffect);

            _distanceConverter = new DistanceConverter(positionConverter, meterPerQuarterNote);
            _timingObject = new TimingObject(fixedTimeFactory, _distanceConverter, noteJudgement);
            _audioTimingInfo = new AudioTimingInfo(audioClipSource, fixedTimeFactory);

            ResetGame();
        }

        public void PlayPauseGame(bool play)
        {
            if (play) StartGame();
            else PauseGame();
        }

        private void StartGame()
        {
            _backgroundAudioPlayer.Play();
        }

        private void PauseGame()
        {
            foreach (var audioPlayer in _audioPlayers) audioPlayer.Pause();
        }

        public void ResetGame()
        {
            foreach (var audioPlayer in _audioPlayers) audioPlayer.Stop();

            scriptLoader.InstantiateAllNotes(_timingObject, _audioPlayers);
            scriptLoader.InstantiateAllDividers(_timingObject);

            onGameReset.Invoke();
        }

        private void Update()
        {
            if (_audioTimingInfo.Progress >= 1f) ResetGame();

            _timingObject.Current = _audioTimingInfo.Now;
            _distanceConverter.DistancePerQuarterNote = meterPerQuarterNote;

            onProgressChanged.Invoke(_audioTimingInfo.Progress);
            onBpmChanged.Invoke(_timingObject.Current.Bpm.ToString(CultureInfo.InvariantCulture));
        }
    }
}
