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
using Utils.Extensions;

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
        [NotNull] private AudioSource[] _audioSources;
        [NotNull] private IDistanceConverter _distanceConverter;
        [NotNull] private AudioTimingInfo _audioTimingInfo;
        [NotNull] private TimingObject _timingObject;
        // ReSharper restore NotNullMemberIsNotInitialized

        private void Awake()
        {
            scriptLoader.LoadScript("Songs/Tutorial/", "Tutorial");

            _audioSources = scriptLoader.InstantiateAudioSource();
            var backgroundAudioSource = _audioSources[0];

            if (scriptLoader.Script.AudioChannels == null) throw new InvalidOperationException();
            if (scriptLoader.Script.Timing.BpmEvents == null) throw new InvalidOperationException();

            var timingConverter = new TimingConverter(
                scriptLoader.Script.Timing.BpmEvents,
                backgroundAudioSource.clip.frequency,
                scriptLoader.Script.Timing.StandardBpm,
                scriptLoader.Script.Timing.StandardPpqn);

            var fixedTimeFactory = new FixedTime.Factory(timingConverter);

            var positionConverter = new PositionConverter.Builder(timingConverter)
                .SetScrollEvent(scriptLoader.Script.Scrolling, ScrollEvent.SpeedBounce)
                .SetPositionScaler(ScalerMode.BpmRelative)
                .SetPositionNormalizer(NormalizerMode.Individual)
                .Build();

            var audioClipSource = new AudioClipSource(backgroundAudioSource, scriptLoader.Script.AudioChannels[0].Offset);

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
            foreach (var audioSource in _audioSources) audioSource.UnPause();
        }

        private void PauseGame()
        {
            foreach (var audioSource in _audioSources) audioSource.Pause();
        }

        public void ResetGame()
        {
            foreach (var audioSource in _audioSources) audioSource.Reset();

            scriptLoader.InstantiateAllNotes(_timingObject);
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
