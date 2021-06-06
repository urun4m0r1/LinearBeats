using System;
using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Time;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Media
{
    public sealed class AudioPlayer : IMediaPlayer
    {
        //TODO: 오프셋이 양수일시 노래만 늦게 나오고 스크롤이 멈춰있음 -> 스크롤은 오프셋과 관계없이 이동해야 함
        [ShowInInspector, ReadOnly] public Sample Current => _source.timeSamples + (Sample) ((float) _offset * SamplesPerSecond);
        [ShowInInspector, ReadOnly, HorizontalGroup(0.2f, LabelWidth = 60)] public bool IsPlaying => _source.isPlaying;
        [ShowInInspector, ReadOnly, ProgressBar(0f, 1f), HorizontalGroup] public float Progress => _source.time / _audioClip.length;
        [ShowInInspector, ReadOnly] public int SamplesPerSecond => _audioClip.frequency;

        [ShowInInspector, ReadOnly] [NotNull] private readonly AudioSource _source;
        [ShowInInspector, ReadOnly] [NotNull] private readonly AudioClip _audioClip;
        [ShowInInspector, ReadOnly] private readonly Second _offset;

        public AudioPlayer([NotNull] AudioSource source, [CanBeNull] Second? offset = null)
        {
            _source = source;

            _audioClip = source.clip;
            if (!_audioClip) throw new NullReferenceException("Null audioClip reference.");

            _offset = offset ?? default;
        }

        public void Play(Second start = default, [CanBeNull] Second? length = null)
        {
            var startSecond = start - _offset;

            _source.Stop();

            if (startSecond < 0)
            {
                _source.PlayDelayed(-startSecond);
            }
            else
            {
                _source.time = startSecond;
                _source.Play();
            }

            if (length != null) _source.SetScheduledEndTime(AudioSettings.dspTime + (double) length);
        }

        public void Resume()
        {
            _source.UnPause();
        }

        public void Pause()
        {
            _source.Pause();
        }

        public void Stop()
        {
            Play();
            _source.Pause();
        }
    }
}
