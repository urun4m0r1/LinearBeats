#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Judgement;
using LinearBeats.Script;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Utils.Unity;

namespace LinearBeats.Game
{
    public sealed class GameManager : SerializedMonoBehaviour
    {
#pragma warning disable IDE0044
        [Range(0.0001f, 10)]
        [SerializeField]
        private float _meterPerPulse = 0.01f;
        [SerializeField]
        private ScriptLoader _scriptLoader = null;
        [OdinSerialize]
        private TimingController _timingController = null;
        [OdinSerialize]
        private NoteJudgement _noteJudgement = null;
#pragma warning restore IDE0044

        private Queue<RailBehaviour> _dividerBehaviours = null;
        private Queue<NoteBehaviour>[] _notesBehaviours = null;
        private AudioSource[] _audioSources = null;
        private AudioSource _backgroundAudioSource = null;

        void Start()
        {
            InitScriptLoader();
            InitAudioSources();
            InitTimingController();
            InitGameObjectsScroll();

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

            void InitGameObjectsScroll()
            {
                _notesBehaviours = _scriptLoader.InstantiateNotes();
                _dividerBehaviours = _scriptLoader.InstantiateDividers();
            }
        }

        [DisableInEditorMode]
        [Button("StartGame")]
        public void StartGame()
        {
            _timingController.ResetTiming();

            foreach (var audioSource in _audioSources)
            {
                audioSource.Play();
            }
        }

        private void Update()
        {
            if (_backgroundAudioSource.isPlaying)
            {
                _timingController.UpdateTiming(_backgroundAudioSource.timeSamples);
                UpdateNoteJudge();

                void UpdateNoteJudge()
                {
                    foreach (var noteBehaviours in _notesBehaviours)
                    {
                        foreach (var noteBehaviour in noteBehaviours)
                        {
                            _noteJudgement.JudgeNote(noteBehaviour, _timingController.CurrentPulse);
                        }
                    }
                }
            }
            else if (_timingController.CurrentPulse != 0)
            {
                _timingController.ResetTiming();
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
                foreach (var noteBehaviours in _notesBehaviours)
                {
                    foreach (var noteBehaviour in noteBehaviours)
                    {
                        noteBehaviour.UpdateRailPosition(_timingController.CurrentPulse, _meterPerPulse);
                    }
                }
            }
        }
    }
}
