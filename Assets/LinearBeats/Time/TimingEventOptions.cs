namespace LinearBeats.Time
{
    public readonly struct TimingEventOptions
    {
        public bool IgnoreJump { get; }
        public bool IgnoreStop { get; }
        public bool IgnoreRewind { get; }

        public TimingEventOptions(
            bool ignoreJump = false,
            bool ignoreStop = false,
            bool ignoreRewind = false)
        {
            IgnoreJump = ignoreJump;
            IgnoreStop = ignoreStop;
            IgnoreRewind = ignoreRewind;
        }
    }
}
