#pragma warning disable IDE0051

using System.Collections.Generic;
using LinearBeats.Judgement;
using LinearBeats.Script;
using LinearBeats.Visuals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using Utils.Unity;

namespace LinearBeats.Game
{
    public sealed class GameManager : MonoBehaviour
    {
#pragma warning disable IDE0044
        [SerializeField]
        private Transform _notesHolder = null;
        [SerializeField]
        private GameObject _shortNotePrefab = null;
        [SerializeField]
        private Transform _dividerHolder = null;
        [SerializeField]
        private GameObject _dividerPrefab = null;
        [SerializeField]
        private AudioListener _audioListener = null;
        [SerializeField]
        private float _meterPerPulse = 0.01f;
        [SerializeField]
        private AudioMixerGroup[] _audioMixerGroups = null;
#pragma warning restore IDE0044

        private ScriptLoader _scriptLoader = null;
        private TimingController _timingController = null;
        private RailScroll _railScroll = null;
        private Queue<RailBehaviour> _dividerBehaviours = null;
        private Queue<RailBehaviour> _noteBehaviours = null;
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
            _scriptLoader = new ScriptLoader("Songs/Tutorial/");
            _scriptLoader.LoadScript("Tutorial");
        }

        private void InitAudioSources()
        {
            _audioSources = _scriptLoader.InstantiateAudioSource(_audioMixerGroups, _audioListener.transform);
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
            _noteBehaviours = _scriptLoader.InstantiateNotes(_shortNotePrefab, _notesHolder);
            _dividerBehaviours = _scriptLoader.InstantiateDividers(_dividerPrefab, _dividerHolder);

            var railBehaviours = new Queue<RailBehaviour>[]
            {
                _dividerBehaviours,
                _noteBehaviours
            };

            _railScroll = new RailScroll(_meterPerPulse, railBehaviours);
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
                    NoteJudgement.UpdateNoteJudgement(audioChannel.Notes, _timingController.CurrentPulse);
                }
            }
        }
    }
}