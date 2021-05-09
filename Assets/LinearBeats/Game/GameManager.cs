using System;
using System.Collections.Generic;
using System.Globalization;
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
        [SerializeField] private UnityEvent<string> onBpmChanged = new UnityEvent<string>();
        [SerializeField] private UnityEvent<float> onProgressChanged = new UnityEvent<float>();
        [SerializeField] private UnityEvent onGameReset = new UnityEvent();
        [Range(1f, 100f)] [SerializeField] private float meterPerQuarterNote = 1f;
        [SerializeField] private ScriptLoader scriptLoader;

        [SerializeField] private LaneEffect laneEffect;
        [DictionaryDrawerSettings(IsReadOnly = true)] [OdinSerialize]
        private Dictionary<Judge, float> _judgeRangeInSeconds = new Dictionary<Judge, float>
        {
            [Judge.Perfect] = 0.033f,
            [Judge.Great] = 0.066f,
            [Judge.Good] = 0.133f,
            [Judge.Bad] = 0.150f,
        };


        private AudioSource[] _audioSources;
        private IDistanceConverter _distanceConverter;
        private AudioTimingInfo _audioTimingInfo;
        private TimingObject _timingObject;

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

            var noteJudgement = new NoteJudgement(_judgeRangeInSeconds, laneEffect);

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
