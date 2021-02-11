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
        private LinearBeatsScript _script;
        private Queue<RailBehaviour> _dividerBehaviours = null;
        private Queue<NoteBehaviour> _noteBehaviours = null;
        private AudioSource[] _audioSources = null;

        void Start()
        {
            _scriptLoader = new ScriptLoader("Songs/Tutorial/");
            _scriptLoader.LoadScript("Tutorial");

            _audioSources = _scriptLoader.InstantiateAudioSource(_audioMixerGroups, _audioListener.transform);
            _noteBehaviours = _scriptLoader.InstantiateNotes(_shortNotePrefab, _notesHolder);
            _dividerBehaviours = _scriptLoader.InstantiateDividers(_dividerPrefab, _dividerHolder);

            _script = _scriptLoader.Script;
            _timingController = new TimingController(
                _script.Timings,
                AudioUtils.AudioFrequencies(_audioSources),
                _script.Metadata.PulsesPerQuarterNote);
        }


        [DisableInEditorMode]
        [Button("PlayAllAudioSource")]
        public void PlayAllAudioSource()
        {
            _timingController.ResetTiming();

            foreach (var audioSource in _audioSources)
            {
                audioSource.Play();
            }
        }

        void Update()
        {
            _timingController.UpdateCurrentPulse(_audioSources[0].timeSamples);
            RailScroll.UpdateRailPosition(_noteBehaviours, _timingController.CurrentPulse, _meterPerPulse);
            RailScroll.UpdateRailPosition(_dividerBehaviours, _timingController.CurrentPulse, _meterPerPulse);

            foreach (var audioChannel in _scriptLoader.Script.AudioChannels)
            {
                if (audioChannel.Notes == null) continue;
                NoteJudgement.UpdateNoteJudgement(audioChannel.Notes, _timingController.CurrentPulse);
            }
        }
    }
}