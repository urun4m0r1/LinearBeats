#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Judgement;
using LinearBeats.Script;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Audio;
using Utils.Unity;

namespace LinearBeats.Game
{
    public sealed class GameManager : SerializedMonoBehaviour
    {
#pragma warning disable IDE0044
        [SerializeField]
        private ScriptLoader _scriptLoader = null;
        [SerializeField]
        private RailScroll _railScroll = null;
        [OdinSerialize]
        private NoteJudgement _noteJudgement = null;
#pragma warning restore IDE0044

        private TimingController _timingController = null;
        private Queue<RailBehaviour> _dividerBehaviours = null;
        private Queue<RailBehaviour>[] _noteBehaviours = null;
        private AudioSource[] _audioSources = null;
        private AudioSource _backgroundAudioSource = null;

        void Start()
        {
            InitScriptLoader();
            InitAudioSources();
            InitGameView();
            ResetGameView();
        }

        private void InitScriptLoader()
        {
            _scriptLoader.LoadScript("Songs/Tutorial/", "Tutorial");
        }

        private void InitAudioSources()
        {
            _audioSources = _scriptLoader.InstantiateAudioSource();
            _backgroundAudioSource = _audioSources[0];
        }

        private void InitGameView()
        {
            InitTimingController();
            InitRailScroll();
        }

        private void InitTimingController()
        {
            _timingController = new TimingController(
                _scriptLoader.Script.Timings,
                AudioUtils.GetSamplesPerTimes(_audioSources),
                _scriptLoader.Script.Metadata.PulsesPerQuarterNote);
        }

        private void InitRailScroll()
        {
            _noteBehaviours = _scriptLoader.InstantiateNotes();
            _dividerBehaviours = _scriptLoader.InstantiateDividers();

            _railScroll.AddRailBehaviours(_dividerBehaviours);
            for (int i = 0; i < _audioSources.Length; ++i)
            {
                _railScroll.AddRailBehaviours(_noteBehaviours[i]);
            }
        }

        private void ResetGameView()
        {
            _timingController.ResetTiming();
            _railScroll.UpdateRailPosition(0);
        }

        [DisableInEditorMode]
        [Button("StartGame")]
        public void StartGame()
        {
            ResetGameView();

            foreach (var audioSource in _audioSources)
            {
                audioSource.Play();
            }
        }


        void Update()
        {
            if (_backgroundAudioSource.isPlaying)
            {
                UpdateGameView();
            }
            else
            {
                if (_timingController.CurrentPulse != 0)
                {
                    ResetGameView();
                }
            }
        }

        private void UpdateGameView()
        {
            _timingController.UpdateTiming(_backgroundAudioSource.timeSamples);
            _railScroll.UpdateRailPosition(_timingController.CurrentPulse);

            foreach (var audioChannel in _scriptLoader.Script.AudioChannels)
            {
                if (audioChannel.Notes != null)
                {
                    foreach (var note in audioChannel.Notes)
                    {
                        if (_noteJudgement.ShouldJudgeNote(note, _timingController.CurrentPulse))
                        {
                            _noteJudgement.JudgeNote(note, _timingController.CurrentPulse);
                        }
                    }
                }
            }
        }
    }
}