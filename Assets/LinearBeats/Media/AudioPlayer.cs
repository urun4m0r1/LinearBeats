using System;
using JetBrains.Annotations;
using LinearBeats.Script;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Media
{
    public sealed class AudioPlayer : IMediaPlayer
    {
        [ShowInInspector, ReadOnly] public int SamplesPerSecond => _source.clip.frequency;
        [ShowInInspector, ReadOnly, HorizontalGroup(0.2f, LabelWidth = 60)] public bool IsPlaying { get; private set; }
        [ShowInInspector, ReadOnly, ProgressBar(0f, 1f), HorizontalGroup] public float Progress => Current / _clipLength;
        [ShowInInspector, ReadOnly] public Second Current => CurrentPlayback - _offset;

        //TODO: 시작 전 4박, 종료 후 4박 추가
        [ShowInInspector, ReadOnly] private readonly Second _offset;
        [ShowInInspector, ReadOnly] private readonly float _clipLength;

        [ShowInInspector, ReadOnly] [NotNull] private readonly AudioSource _source;

        [ShowInInspector, ReadOnly]
        private Second CurrentPlayback
        {
            get
            {
                if (!IsPlaying) return _playbackTime;

                return _playbackTime + (Second) AudioSettings.dspTime - _lastStartTime;
            }
        }

        [ShowInInspector, ReadOnly] private Second _playbackTime;
        [ShowInInspector, ReadOnly] private Second _lastStartTime;

        private AudioPlayer([NotNull] AudioSource source)
        {
            _source = source;

            if (!source.clip) throw new NullReferenceException("Null audioClip reference.");
        }

        public AudioPlayer([NotNull] AudioSource source, [CanBeNull] Second? offset = null) : this(source)
        {
            _offset = offset ?? default;
            _clipLength = source.clip.length - offset ?? default;
        }

        public void Play(Second? playbackPosition = null, Second? playLength = null)
        {
            PlaySource((playbackPosition ?? default) + _offset, playLength);
        }

        public void Resume()
        {
            PlaySource(_playbackTime);
        }

        private void PlaySource(Second playbackTime, [CanBeNull] Second? playLength = null)
        {
            _lastStartTime = (Second) AudioSettings.dspTime;

            if (playbackTime < 0)
            {
                _source.time = 0;
                _source.PlayDelayed(-playbackTime);
            }
            else
            {
                _source.time = playbackTime;
                _source.Play();
            }

            if (playLength != null) _source.SetScheduledEndTime(AudioSettings.dspTime + (double) playLength);

            IsPlaying = true;
        }

        public void Pause()
        {
            _source.Pause();

            _playbackTime = CurrentPlayback;

            IsPlaying = false;
        }

        public void Stop()
        {
            _source.Stop();

            _playbackTime = _offset;
            _source.time = Mathf.Clamp(_offset, 0f, float.MaxValue);

            IsPlaying = false;
        }
    }
}
