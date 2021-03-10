#pragma warning disable IDE0090

namespace LinearBeats.Time
{
    public struct FixedTime
    {
        public static TimingConverter Converter; //Singleton에서 받아오기
        public Pulse Pulse { get; }
        public Second Second { get; }
        public Sample Sample { get; }
        public float Bpm { get; }

        public FixedTime(Pulse value)
        {
            Pulse = value;
            Second = Converter.ToSecond(value);
            Sample = Converter.ToSample(value);
            Bpm = Converter.GetBpm(value);
        }

        public FixedTime(Second value)
        {
            Pulse = Converter.ToPulse(value);
            Second = value;
            Sample = Converter.ToSample(value);
            Bpm = Converter.GetBpm(value);
        }

        public FixedTime(Sample value)
        {
            Pulse = Converter.ToPulse(value);
            Second = Converter.ToSecond(value);
            Sample = value;
            Bpm = Converter.GetBpm(value);
        }

        public static implicit operator Pulse(FixedTime value) => value.Pulse;
        public static implicit operator Second(FixedTime value) => value.Second;
        public static implicit operator Sample(FixedTime value) => value.Sample;
        public static implicit operator FixedTime(Pulse value) => new FixedTime(value);
        public static implicit operator FixedTime(Second value) => new FixedTime(value);
        public static implicit operator FixedTime(Sample value) => new FixedTime(value);
    }
}
