using JetBrains.Annotations;
using Sirenix.OdinInspector;

// ReSharper disable UnassignedField.Global

namespace LinearBeats.Script
{
    public struct LinearBeatsScript
    {
        public ushort VersionCode;
        [CanBeNull] public string VersionName;
        public Metadata Metadata;
        [CanBeNull] public MediaChannel[] VideoChannels;
        [CanBeNull] public MediaChannel[] AudioChannels;
        public Timing Timing;
        public Scrolling Scrolling;
        [CanBeNull] public Divider[] Dividers;
        [CanBeNull] public Trigger[] Effects;
        [CanBeNull] public Trigger[] Videos;
        [CanBeNull] public Note[] Notes;
    }

    public struct Metadata
    {
        public string GameMode;
        public string Category;
        public byte Level;
        public byte JudgeLevel;
        public byte LifeLevel;
        public byte Difficulty;
        [CanBeNull] public string DifficultyName;
        [CanBeNull] public string Genre;
        [CanBeNull] public string Title;
        [CanBeNull] public string TitleSub;
        [CanBeNull] public string Artist;
        [CanBeNull] public string[] ArtistsSub;
        [CanBeNull] public string FileNameCover;
        [CanBeNull] public string FileNameBackground;
        [CanBeNull] public string FileNameBanner;
        [CanBeNull] public string FileNameSplash;
        [CanBeNull] public string FileNamePreview;
        public Second PreviewStart;
        public Second PreviewEnd;
    }

    public struct Timing
    {
        [CanBeNull] public float? StandardBpm;
        [CanBeNull] public float? StandardPpqn;
        [CanBeNull] public BpmEvent[] BpmEvents;
    }

    public struct Scrolling
    {
        [CanBeNull] public ScrollingEvent[] StopEvents;
        [CanBeNull] public ScrollingEvent[] JumpEvents;
        [CanBeNull] public ScrollingEvent[] BackJumpEvents;
        [CanBeNull] public ScrollingEvent[] RewindEvents;
        [CanBeNull] public ScrollingEvent[] SpeedEvents;
        [CanBeNull] public ScrollingEvent[] SpeedBounceEvents;
    }

    [InlineProperty]
    public struct BpmEvent
    {
        [ShowInInspector, ReadOnly, HorizontalGroup(LabelWidth = 30)] public float Ppqn;
        [ShowInInspector, ReadOnly, HorizontalGroup] public float Bpm;
        [ShowInInspector, ReadOnly, HorizontalGroup, LabelText("Tick")] public Pulse Pulse;
    }

    public struct ScrollingEvent
    {
        public Pulse Pulse;
        public Pulse Duration;
        public float Amount;
        [CanBeNull] public string IgnoreLane;
    }

    public struct Divider
    {
        public Pulse Pulse;
        public Pulse Interval;
        public byte Type;
        [CanBeNull] public string IgnoreScrollEvent;
    }

    public struct MediaChannel
    {
        public ushort Channel;
        [CanBeNull] public string FileName;
        public Second Offset;
        public byte Layer;
    }

    public struct Trigger
    {
        public ushort Channel;
        public byte Mode;
        public Pulse Pulse;
        public Pulse Duration;
    }

    public struct Note
    {
        public Trigger Trigger;
        public Shape Shape;
        [CanBeNull] public string IgnoreScrollEvent;
    }

    public struct Shape
    {
        public byte Type;
        public byte PosRow;
        public byte PosCol;
        public byte SizeRow;
        public byte SizeCol;
        public byte DstRow;
        public byte DstCol;
    }
}
