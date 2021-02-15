using LinearBeats.Visuals;
using UnityEngine;

namespace LinearBeats.Script
{
    public struct LinearBeatsScript
    {
        public ushort VersionCode;
        public string VersionName;
        public Metadata Metadata;
        public Timing[] Timings;
        public Divider[] Dividers;
        public Effect[] Effects;
        public VideoChannel[] VideoChannels;
        public AudioChannel[] AudioChannels;
        public Note[] Notes;
    }

    public struct Metadata
    {
        public string GameMode;
        public string Category;
        public byte Level;
        public byte LevelJudge;
        public byte LevelLife;
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
        public ulong PulsePreviewStart;
        public ulong PulsePreviewEnd;
        public ushort PulsesPerQuarterNote;
    }

    public struct Timing
    {
        public ulong Pulse;
        public float Bpm;
        public ulong PulseStopDuration;
        public ulong PulseReverseDuration;
    }

    public struct Divider
    {
        public ulong Pulse;
        public ulong PulseInterval;
        public byte DividerType;
    }

    public struct Effect
    {
        public ulong Pulse;
        public ulong PulseDuration;
        public byte Trigger;
        public byte EffectMode;
    }

    public struct VideoChannel
    {
        public string FileName;
        public ushort PulseOffset;
        public byte Layer;
        public Video[] Videos;
    }

    public struct Video
    {
        public ulong Pulse;
        public byte Trigger;
        public byte VideoMode;
    }

    public struct AudioChannel
    {
        public string FileName;
        public ushort PulseOffset;
        public byte Layer;
    }

    public struct Note
    {
        public ulong Pulse;
        public ulong PulseDuration;
        public byte PositionRow;
        public byte PositionCol;
        public byte SizeRow;
        public byte SizeCol;
        public byte DstRow;
        public byte DstCol;
        public ushort Channel;
        public ushort Mode;
    }
}
