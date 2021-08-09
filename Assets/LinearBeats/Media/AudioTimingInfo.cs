using JetBrains.Annotations;
using LinearBeats.Time;
using Sirenix.OdinInspector;

namespace LinearBeats.Media
{
    public sealed class AudioTimingInfo
    {
        [ShowInInspector, ReadOnly, HideLabel] public FixedTime Current => _fixedTimeFactory.Create(_audioPlayer.Current);

        [ShowInInspector, ReadOnly] [NotNull] private readonly AudioPlayer _audioPlayer;
        [NotNull] private readonly FixedTime.Factory _fixedTimeFactory;

        public AudioTimingInfo([NotNull] AudioPlayer audioPlayer, [NotNull] FixedTime.Factory fixedTimeFactory)
        {
            _audioPlayer = audioPlayer;
            _fixedTimeFactory = fixedTimeFactory;
        }
    }
}
