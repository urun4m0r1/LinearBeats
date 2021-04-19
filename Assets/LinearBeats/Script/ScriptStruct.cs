using LinearBeats.Time;

namespace LinearBeats.Script
{
    public struct LinearBeatsScript
    {
        public ushort VersionCode;
        public string VersionName;
        public Metadata Metadata;
        public MediaChannel[] VideoChannels;
        public MediaChannel[] AudioChannels;
        public Timing Timing;
        public Divider[] Dividers;
        public Trigger[] Effects;
        public Trigger[] Videos;
        public Note[] Notes;
    }

    public struct Metadata
    {
        public string GameMode;
        public string Category;
        public byte Level;
        public byte JudgeLevel;
        public byte LifeLevel;
        public byte Difficulty;
        public string DifficultyName;
        public string Genre;
        public string Title;
        public string TitleSub;
        public string Artist;
        public string[] ArtistsSub;
        public string FileNameCover;
        public string FileNameBackground;
        public string FileNameBanner;
        public string FileNameSplash;
        public string FileNamePreview;
        public Second PreviewStart;
        public Second PreviewEnd;
    }

    public struct Timing
    {
        public float? StandardBpm;
        public float? StandardPpqn;
        public BpmEvent[] BpmEvents;
        public TimingEvent[] StopEvents;
        public TimingEvent[] RewindEvents;
        public TimingEvent[] JumpEvents;
        public SpeedEvent[] SpeedEvents;
    }

    public struct BpmEvent
    {
        public float Ppqn;
        public Pulse Pulse;
        public float Bpm;
    }

    public struct TimingEvent
    {
        public Pulse Pulse;
        public Pulse Duration;
        public string IgnoreLane;
    }

    public struct SpeedEvent
    {
        public Pulse Pulse;
        public Pulse Duration;
        public float Multiplier;
        public string IgnoreLane;
    }

    public struct Divider
    {
        public Pulse Pulse;
        public Pulse Interval;
        public byte Type;
        public string IgnoreTimingEvent;
    }

    public struct MediaChannel
    {
        public ushort Channel;
        public string FileName;
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
        public string IgnoreTimingEvent;
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
