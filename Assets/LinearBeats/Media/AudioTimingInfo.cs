using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Time;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LinearBeats.Media
{
    //TODO: 오프셋이 양수일시 노래만 늦게 나오고 스크롤이 멈춰있음
    public sealed class AudioTimingInfo
    {
        [ShowInInspector, ReadOnly, HideLabel] public FixedTime Now => _fixedTimeFactory.Create(Current - _offset);
        [ShowInInspector, ReadOnly, HorizontalGroup(0.2f, LabelWidth = 60)] public bool IsPlaying => _audioSource.isPlaying;
        [ShowInInspector, ReadOnly, ProgressBar(0f, 1f), HorizontalGroup] public float Progress => Current / _length;

        private FixedTime Current => _fixedTimeFactory.Create((Sample) _audioSource.timeSamples);
        private readonly Second _offset;
        private readonly Second _length;

        [NotNull] private readonly AudioSource _audioSource;
        [NotNull] private readonly FixedTime.Factory _fixedTimeFactory;

        public AudioTimingInfo([NotNull] AudioPlayer audioPlayer, [NotNull] FixedTime.Factory fixedTimeFactory)
        {
            _audioSource = audioPlayer.Source;
            _offset = audioPlayer.Offset;
            _length = _audioSource.clip.length;
            _fixedTimeFactory = fixedTimeFactory;
        }
    }
}
