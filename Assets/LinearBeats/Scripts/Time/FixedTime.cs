#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct FixedTime : IComparable<FixedTime>, IEquatable<FixedTime>
    {
        public TimingConverter Converter { get; }
        public Pulse Pulse { get; }
        public Second Second { get; }
        public Sample Sample { get; }
        public float Bpm { get; }

        private float Value => Second;

        private FixedTime(TimingConverter converter) : this()
        {
            Converter = converter ?? throw new ArgumentNullException();
        }

        public FixedTime(TimingConverter converter, Pulse value) : this(converter)
        {
            Pulse = converter.ToPulse(value);
            Second = converter.ToSecond(value);
            Sample = converter.ToSample(value);
            Bpm = converter.GetBpm(value);
        }

        public FixedTime(TimingConverter converter, Second value) : this(converter)
        {
            Pulse = converter.ToPulse(value);
            Second = converter.ToSecond(value);
            Sample = converter.ToSample(value);
            Bpm = converter.GetBpm(value);
        }

        public FixedTime(TimingConverter converter, Sample value) : this(converter)
        {
            Pulse = converter.ToPulse(value);
            Second = converter.ToSecond(value);
            Sample = converter.ToSample(value);
            Bpm = converter.GetBpm(value);
        }

        private FixedTime(TimingConverter converter, float value) : this(converter, (Second)value) { }

        public static implicit operator Pulse(FixedTime value) => value.Pulse;
        public static implicit operator Second(FixedTime value) => value.Second;
        public static implicit operator Sample(FixedTime value) => value.Sample;

        int IComparable<FixedTime>.CompareTo(FixedTime value)
        {
            if (Converter != value.Converter) throw new InvalidOperationException();
            else return Value.CompareTo(value.Value);
        }

        bool IEquatable<FixedTime>.Equals(FixedTime value) => Equals(value);
        public override bool Equals(object obj)
        {
            return (obj is FixedTime value) && (Value == value.Value) && (Converter == value.Converter);
        }

        public override int GetHashCode() => GetHashCode();

        public override string ToString()
        {
            return $"Bpm: {Bpm:0.##} / Pulse: {Pulse:0.##} / Second: {Second:0.##} / Sample: {Sample:0.##}";
        }

        public static FixedTime operator +(FixedTime value) => value;
        public static FixedTime operator -(FixedTime value) => new FixedTime(value.Converter, -value.Value);

        private static FixedTime ValidateEquality(FixedTime a, FixedTime b, float result)
        {
            if (a.Converter != b.Converter) throw new InvalidOperationException();
            else return new FixedTime(a.Converter, result);
        }

        private static bool ValidateEquality(FixedTime a, FixedTime b, bool result)
        {
            if (a.Converter != b.Converter) throw new InvalidOperationException();
            else return result;
        }

        public static FixedTime operator +(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value + b.Value);
        public static FixedTime operator -(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value - b.Value);
        public static FixedTime operator *(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value * b.Value);
        public static FixedTime operator /(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value / b.Value);

        public static bool operator ==(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value == b.Value);
        public static bool operator !=(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value != b.Value);

        public static bool operator <(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value < b.Value);
        public static bool operator >(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value > b.Value);

        public static bool operator <=(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value <= b.Value);
        public static bool operator >=(FixedTime a, FixedTime b) => ValidateEquality(a, b, a.Value >= b.Value);
    }
}
