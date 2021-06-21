using JetBrains.Annotations;
using LinearBeats.Keyboard;
using Sirenix.Serialization;

namespace LinearBeats.Script
{
    public struct LinearBeatsScript
    {
        [OdinSerialize] public ushort VersionCode { get; private set; }

        [OdinSerialize] public Metadata Metadata { get; private set; }
        [OdinSerialize] public PreviewInfo PreviewInfo { get; private set; }
        [OdinSerialize] public Timing Timing { get; private set; }
        [OdinSerialize] public Scrolling Scrolling { get; private set; }

        [OdinSerialize] [CanBeNull] public MediaChannel[] VideoChannels { get; private set; }
        [OdinSerialize] [CanBeNull] public MediaChannel[] AudioChannels { get; private set; }

        [OdinSerialize] [CanBeNull] public Divider[] Dividers { get; private set; }
        [OdinSerialize] [CanBeNull] public Trigger[] Effects { get; private set; }
        [OdinSerialize] [CanBeNull] public Trigger[] Videos { get; private set; }
        [OdinSerialize] [CanBeNull] public Note[] Notes { get; private set; }

        public LinearBeatsScript(
            ushort versionCode,
            Metadata metadata,
            PreviewInfo previewInfo,
            Timing timing,
            Scrolling scrolling,
            [CanBeNull] MediaChannel[] audioChannels = null,
            [CanBeNull] MediaChannel[] videoChannels = null,
            [CanBeNull] Divider[] dividers = null,
            [CanBeNull] Trigger[] effects = null,
            [CanBeNull] Trigger[] videos = null,
            [CanBeNull] Note[] notes = null)
        {
            VersionCode = versionCode;
            Metadata = metadata;
            PreviewInfo = previewInfo;
            Timing = timing;
            Scrolling = scrolling;
            AudioChannels = audioChannels;
            VideoChannels = videoChannels;
            Dividers = dividers;
            Effects = effects;
            Videos = videos;
            Notes = notes;
        }
    }

    public struct Metadata
    {
        [OdinSerialize] [YamlRequired, NotNull] public string Title { get; private set; }

        [OdinSerialize] public byte GameMode { get; private set; }
        [OdinSerialize] public byte Level { get; private set; }
        [OdinSerialize] public byte Difficulty { get; private set; }

        [OdinSerialize] [CanBeNull] public byte? JudgeLevel { get; private set; }
        [OdinSerialize] [CanBeNull] public byte? LifeLevel { get; private set; }

        [OdinSerialize] [CanBeNull] public string DifficultyName { get; private set; }
        [OdinSerialize] [CanBeNull] public string Category { get; private set; }
        [OdinSerialize] [CanBeNull] public string Genre { get; private set; }

        [OdinSerialize] [CanBeNull] public string TitleSub { get; private set; }
        [OdinSerialize] [CanBeNull] public string Artist { get; private set; }
        [OdinSerialize] [CanBeNull] public string[] ArtistsSub { get; private set; }

        public Metadata(
            [NotNull] string title,
            byte gameMode,
            byte level,
            byte difficulty,
            [CanBeNull] byte? judgeLevel = null,
            [CanBeNull] byte? lifeLevel = null,
            [CanBeNull] string difficultyName = null,
            [CanBeNull] string category = null,
            [CanBeNull] string genre = null,
            [CanBeNull] string titleSub = null,
            [CanBeNull] string artist = null,
            [CanBeNull] string[] artistsSub = null)
        {
            Title = title;
            GameMode = gameMode;
            Level = level;
            Difficulty = difficulty;
            JudgeLevel = judgeLevel;
            LifeLevel = lifeLevel;
            DifficultyName = difficultyName;
            Category = category;
            Genre = genre;
            TitleSub = titleSub;
            Artist = artist;
            ArtistsSub = artistsSub;
        }
    }

    public struct PreviewInfo
    {
        [OdinSerialize] [CanBeNull] public string FileNameCover { get; private set; }
        [OdinSerialize] [CanBeNull] public string FileNameBackground { get; private set; }
        [OdinSerialize] [CanBeNull] public string FileNameBanner { get; private set; }
        [OdinSerialize] [CanBeNull] public string FileNameSplash { get; private set; }

        [OdinSerialize] [CanBeNull] public string FileNamePreview { get; private set; }

        [OdinSerialize] [CanBeNull] public Second? PreviewStart { get; private set; }
        [OdinSerialize] [CanBeNull] public Second? PreviewEnd { get; private set; }

        public PreviewInfo(
            [CanBeNull] string fileNameCover = null,
            [CanBeNull] string fileNameBackground = null,
            [CanBeNull] string fileNameBanner = null,
            [CanBeNull] string fileNameSplash = null,
            [CanBeNull] string fileNamePreview = null,
            [CanBeNull] Second? previewStart = null,
            [CanBeNull] Second? previewEnd = null)
        {
            FileNameCover = fileNameCover;
            FileNameBackground = fileNameBackground;
            FileNameBanner = fileNameBanner;
            FileNameSplash = fileNameSplash;
            FileNamePreview = fileNamePreview;
            PreviewStart = previewStart;
            PreviewEnd = previewEnd;
        }
    }

    public struct Timing
    {
        [OdinSerialize] public float StandardBpm { get; private set; }
        [OdinSerialize] public int StandardPpqn { get; private set; }
        [OdinSerialize] [CanBeNull] public BpmEvent[] BpmEvents { get; private set; }
        public Timing(float standardBpm, int standardPpqn, [CanBeNull] BpmEvent[] bpmEvents = null)
        {
            StandardBpm = standardBpm;
            StandardPpqn = standardPpqn;
            BpmEvents = bpmEvents;
        }
    }

    public struct Scrolling
    {
        [OdinSerialize] [CanBeNull] public ScrollingEvent[] StopEvents { get; private set; }
        [OdinSerialize] [CanBeNull] public ScrollingEvent[] JumpEvents { get; private set; }
        [OdinSerialize] [CanBeNull] public ScrollingEvent[] BackJumpEvents { get; private set; }
        [OdinSerialize] [CanBeNull] public ScrollingEvent[] RewindEvents { get; private set; }
        [OdinSerialize] [CanBeNull] public ScrollingEvent[] SpeedEvents { get; private set; }
        [OdinSerialize] [CanBeNull] public ScrollingEvent[] SpeedBounceEvents { get; private set; }

        public Scrolling(
            [CanBeNull] ScrollingEvent[] stopEvents = null,
            [CanBeNull] ScrollingEvent[] jumpEvents = null,
            [CanBeNull] ScrollingEvent[] backJumpEvents = null,
            [CanBeNull] ScrollingEvent[] rewindEvents = null,
            [CanBeNull] ScrollingEvent[] speedEvents = null,
            [CanBeNull] ScrollingEvent[] speedBounceEvents = null)
        {
            StopEvents = stopEvents;
            JumpEvents = jumpEvents;
            BackJumpEvents = backJumpEvents;
            RewindEvents = rewindEvents;
            SpeedEvents = speedEvents;
            SpeedBounceEvents = speedBounceEvents;
        }
    }

    public struct ScrollingEvent
    {
        [OdinSerialize] public Pulse Pulse { get; private set; }
        [OdinSerialize] public Pulse Duration { get; private set; }
        [OdinSerialize] [CanBeNull] public float? Amount { get; private set; }
        [OdinSerialize] [CanBeNull] public string IgnoreLane { get; private set; }

        public ScrollingEvent(
            Pulse pulse,
            Pulse duration,
            [CanBeNull] float? amount = null,
            [CanBeNull] string ignoreLane = null)
        {
            Pulse = pulse;
            Duration = duration;
            Amount = amount;
            IgnoreLane = ignoreLane;
        }
    }

    public struct BpmEvent
    {
        [OdinSerialize] public Pulse Pulse { get; private set; }
        [OdinSerialize] [CanBeNull] public float? Bpm { get; private set; }
        [OdinSerialize] [CanBeNull] public int? Ppqn { get; private set; }

        public BpmEvent(
            Pulse pulse,
            [CanBeNull] float? bpm = null,
            [CanBeNull] int? ppqn = null)
        {
            Pulse = pulse;
            Bpm = bpm;
            Ppqn = ppqn;
        }
    }

    public struct MediaChannel
    {
        [OdinSerialize] [YamlRequired, NotNull] public string FileName { get; private set; }
        [OdinSerialize] public ushort Channel { get; private set; }
        [OdinSerialize] [CanBeNull] public Second? Offset { get; private set; }
        [OdinSerialize] [CanBeNull] public byte? Layer { get; private set; }

        public MediaChannel(
            [NotNull] string fileName,
            ushort channel,
            [CanBeNull] Second? offset = null,
            [CanBeNull] byte? layer = null) : this()
        {
            FileName = fileName;
            Channel = channel;
            Offset = offset;
            Layer = layer;
        }
    }

    public struct Divider
    {
        [OdinSerialize] public Pulse Pulse { get; private set; }
        [OdinSerialize] public Pulse? Interval { get; private set; }
        [OdinSerialize] public byte? Type { get; private set; }
        [OdinSerialize] [CanBeNull] public string IgnoreScrollEvent { get; private set; }

        public Divider(
            Pulse pulse,
            Pulse? interval = null,
            byte? type = null,
            [CanBeNull] string ignoreScrollEvent = null)
        {
            Pulse = pulse;
            Interval = interval;
            Type = type;
            IgnoreScrollEvent = ignoreScrollEvent;
        }
    }

    public struct Trigger
    {
        [OdinSerialize] public ushort Channel { get; private set; }
        [OdinSerialize] public Pulse Pulse { get; private set; }
        [OdinSerialize] [CanBeNull] public Pulse? Duration { get; private set; }
        [OdinSerialize] [CanBeNull] public byte? Mode { get; private set; }

        public Trigger(
            ushort channel,
            Pulse pulse,
            Pulse? duration = null,
            byte? mode = null)
        {
            Channel = channel;
            Pulse = pulse;
            Duration = duration;
            Mode = mode;
        }
    }

    public struct Note
    {
        [OdinSerialize] public Trigger Trigger { get; private set; }
        [OdinSerialize] public Shape Shape { get; private set; }
        [OdinSerialize] [CanBeNull] public string IgnoreScrollEvent { get; private set; }

        public Note(
            Trigger trigger,
            Shape shape,
            [CanBeNull] string ignoreScrollEvent = null)
        {
            Trigger = trigger;
            Shape = shape;
            IgnoreScrollEvent = ignoreScrollEvent;
        }
    }

    public struct Shape
    {
        [OdinSerialize] public KeyType Pos { get; private set; }
        [OdinSerialize] [CanBeNull] public KeyType? Dst { get; private set; }
        [OdinSerialize] [CanBeNull] public byte? Size { get; private set; }
        [OdinSerialize] [CanBeNull] public byte? Type { get; private set; }

        public Shape(
            KeyType pos,
            KeyType? dst = null,
            byte? size = null,
            byte? type = null)
        {
            Pos = pos;
            Dst = dst;
            Size = size;
            Type = type;
        }
    }
}
