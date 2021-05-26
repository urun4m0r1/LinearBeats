using JetBrains.Annotations;
using Sirenix.OdinInspector;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace LinearBeats.Script
{
    public struct LinearBeatsScript
    {
        public ushort VersionCode { get; private set; }
        [CanBeNull] public string VersionName { get; private set; }
        public Metadata Metadata { get; private set; }
        [CanBeNull] public MediaChannel[] VideoChannels { get; private set; }
        [CanBeNull] public MediaChannel[] AudioChannels { get; private set; }
        public Timing Timing { get; private set; }
        public Scrolling Scrolling { get; private set; }
        [CanBeNull] public Divider[] Dividers { get; private set; }
        [CanBeNull] public Trigger[] Effects { get; private set; }
        [CanBeNull] public Trigger[] Videos { get; private set; }
        [CanBeNull] public Note[] Notes { get; private set; }
    }

    public struct Metadata
    {
        public string GameMode { get; private set; }
        public string Category { get; private set; }
        public byte Level { get; private set; }
        public byte JudgeLevel { get; private set; }
        public byte LifeLevel { get; private set; }
        public byte Difficulty { get; private set; }
        [CanBeNull] public string DifficultyName { get; private set; }
        [CanBeNull] public string Genre { get; private set; }
        [CanBeNull] public string Title { get; private set; }
        [CanBeNull] public string TitleSub { get; private set; }
        [CanBeNull] public string Artist { get; private set; }
        [CanBeNull] public string[] ArtistsSub { get; private set; }
        [CanBeNull] public string FileNameCover { get; private set; }
        [CanBeNull] public string FileNameBackground { get; private set; }
        [CanBeNull] public string FileNameBanner { get; private set; }
        [CanBeNull] public string FileNameSplash { get; private set; }
        [CanBeNull] public string FileNamePreview { get; private set; }
        public Second PreviewStart { get; private set; }
        public Second PreviewEnd { get; private set; }
    }

    public struct Timing
    {
        [CanBeNull] public float? StandardBpm { get; private set; }
        [CanBeNull] public float? StandardPpqn { get; private set; }
        [CanBeNull] public BpmEvent[] BpmEvents { get; private set; }
    }

    public struct Scrolling
    {
        [CanBeNull] public ScrollingEvent[] StopEvents { get; private set; }
        [CanBeNull] public ScrollingEvent[] JumpEvents { get; private set; }
        [CanBeNull] public ScrollingEvent[] BackJumpEvents { get; private set; }
        [CanBeNull] public ScrollingEvent[] RewindEvents { get; private set; }
        [CanBeNull] public ScrollingEvent[] SpeedEvents { get; private set; }
        [CanBeNull] public ScrollingEvent[] SpeedBounceEvents { get; private set; }
    }

    [InlineProperty]
    public struct BpmEvent
    {
        public BpmEvent(float ppqn, float bpm, Pulse pulse)
        {
            Ppqn = ppqn;
            Bpm = bpm;
            Pulse = pulse;
        }

        [ShowInInspector, ReadOnly, HorizontalGroup(LabelWidth = 30)]
        public float Ppqn { get; private set; }

        [ShowInInspector, ReadOnly, HorizontalGroup]
        public float Bpm { get; private set; }

        [ShowInInspector, ReadOnly, HorizontalGroup, LabelText("Tick")]
        public Pulse Pulse { get; private set; }
    }

    public struct ScrollingEvent
    {
        public Pulse Pulse { get; private set; }
        public Pulse Duration { get; private set; }
        public float Amount { get; private set; }
        [CanBeNull] public string IgnoreLane { get; private set; }
    }

    public struct Divider
    {
        public Pulse Pulse { get; private set; }
        public Pulse Interval { get; private set; }
        public byte Type { get; private set; }
        [CanBeNull] public string IgnoreScrollEvent { get; private set; }
    }

    public struct MediaChannel
    {
        public ushort Channel { get; private set; }
        [CanBeNull] public string FileName { get; private set; }
        public Second Offset { get; private set; }
        public byte Layer { get; private set; }
    }

    public struct Trigger
    {
        public ushort Channel { get; private set; }
        public byte Mode { get; private set; }
        public Pulse Pulse { get; private set; }
        public Pulse Duration { get; private set; }
    }

    public struct Note
    {
        public Trigger Trigger { get; private set; }
        public Shape Shape { get; private set; }
        [CanBeNull] public string IgnoreScrollEvent { get; private set; }
    }

    public struct Shape
    {
        public Shape(
            byte posRow,
            byte posCol,
            byte sizeRow = default,
            byte sizeCol = default,
            byte dstRow = default,
            byte dstCol = default,
            byte type = default)
        {
            PosRow = posRow;
            PosCol = posCol;
            SizeRow = sizeRow;
            SizeCol = sizeCol;
            DstRow = dstRow;
            DstCol = dstCol;
            Type = type;
        }

        public byte PosRow { get; private set; }
        public byte PosCol { get; private set; }
        public byte SizeRow { get; private set; }
        public byte SizeCol { get; private set; }
        public byte DstRow { get; private set; }
        public byte DstCol { get; private set; }
        public byte Type { get; private set; }
    }
}
