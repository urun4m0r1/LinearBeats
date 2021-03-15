#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct FixedTime : IComparable<FixedTime>, IEquatable<FixedTime>
    {
        public static TimingConverter Converter;
        public static FixedTime MaxValue = (Sample)float.MaxValue;
        public static FixedTime MinValue = (Sample)float.MinValue;
        public static FixedTime Zero = (Sample)0f;

        public Pulse Pulse { get; }
        public Second Second { get; }
        public Sample Sample { get; }
        public float Bpm { get; }

        private readonly float _value;

        public FixedTime(Pulse value)
        {
            Pulse = value;
            Second = Converter.ToSecond(value);
            Sample = Converter.ToSample(value);
            Bpm = Converter.GetBpm(value);

            _value = Sample;
        }

        public FixedTime(Second value)
        {
            Pulse = Converter.ToPulse(value);
            Second = value;
            Sample = Converter.ToSample(value);
            Bpm = Converter.GetBpm(value);

            _value = Sample;
        }

        public FixedTime(Sample value)
        {
            Pulse = Converter.ToPulse(value);
            Second = Converter.ToSecond(value);
            Sample = value;
            Bpm = Converter.GetBpm(value);

            _value = Sample;
        }

        public static implicit operator Pulse(FixedTime value) => value.Pulse;
        public static implicit operator Second(FixedTime value) => value.Second;
        public static implicit operator Sample(FixedTime value) => value.Sample;

        public static implicit operator FixedTime(Pulse value) => new FixedTime(value);
        public static implicit operator FixedTime(Second value) => new FixedTime(value);
        public static implicit operator FixedTime(Sample value) => new FixedTime(value);

        int IComparable<FixedTime>.CompareTo(FixedTime value) => _value.CompareTo(value._value);
        bool IEquatable<FixedTime>.Equals(FixedTime value) => _value == value._value;
        public override bool Equals(object obj) => (obj is FixedTime value) && (_value == value._value);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString()
        {
            return $"Bpm: {Bpm:0.##} / Pulse: {Pulse:0.##} / Second: {Second:0.##} / Sample: {Sample:0.##}";
        }

        public static FixedTime operator +(FixedTime value) => value;
        public static FixedTime operator -(FixedTime value) => (Sample)(-value._value);

        public static FixedTime operator +(FixedTime a, FixedTime b) => (Sample)(a._value + b._value);
        public static FixedTime operator -(FixedTime a, FixedTime b) => (Sample)(a._value - b._value);
        public static FixedTime operator *(FixedTime a, FixedTime b) => (Sample)(a._value * b._value);
        public static FixedTime operator /(FixedTime a, FixedTime b) => (Sample)(a._value / b._value);

        public static bool operator ==(FixedTime a, FixedTime b) => a._value == b._value;
        public static bool operator !=(FixedTime a, FixedTime b) => a._value != b._value;

        public static bool operator <(FixedTime a, FixedTime b) => a._value < b._value;
        public static bool operator >(FixedTime a, FixedTime b) => a._value > b._value;

        public static bool operator <=(FixedTime a, FixedTime b) => a._value <= b._value;
        public static bool operator >=(FixedTime a, FixedTime b) => a._value >= b._value;
    }
}
