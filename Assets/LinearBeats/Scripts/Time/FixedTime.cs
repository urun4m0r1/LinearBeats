#pragma warning disable IDE0090

using System;
using LinearBeats.Visuals;

namespace LinearBeats.Time
{
    public struct FixedTime : IComparable<FixedTime>, IEquatable<FixedTime>
    {
        public TimingConverter Converter { get; }
        public PositionConverter PositionConverter { get; }
        public float Position { get; }
        public Pulse Pulse { get; }
        public Second Second { get; }
        public Sample Sample { get; }
        public float Bpm { get; }

        private float Value => Second;

        private FixedTime(PositionConverter positionConverter)
            : this()
        {
            PositionConverter = positionConverter ?? throw new ArgumentNullException();
            Converter = positionConverter?.Converter ?? throw new ArgumentNullException();
        }

        public FixedTime(PositionConverter positionConverter, Pulse value)
            : this(positionConverter)
        {
            Pulse = Converter.ToPulse(value);
            Second = Converter.ToSecond(value);
            Sample = Converter.ToSample(value);
            Bpm = Converter.GetBpm(value);

            var normalizedPulse = Converter.Normalize(Pulse);
            Position = PositionConverter.ToPosition(normalizedPulse);
        }

        public FixedTime(PositionConverter positionConverter, Second value)
            : this(positionConverter)
        {
            Pulse = Converter.ToPulse(value);
            Second = Converter.ToSecond(value);
            Sample = Converter.ToSample(value);
            Bpm = Converter.GetBpm(value);

            var normalizedPulse = Converter.Normalize(Pulse);
            Position = PositionConverter.ToPosition(normalizedPulse);
        }

        public FixedTime(PositionConverter positionConverter, Sample value)
            : this(positionConverter)
        {
            Pulse = Converter.ToPulse(value);
            Second = Converter.ToSecond(value);
            Sample = Converter.ToSample(value);
            Bpm = Converter.GetBpm(value);

            var normalizedPulse = Converter.Normalize(Pulse);
            Position = PositionConverter.ToPosition(normalizedPulse);
        }

        private FixedTime(PositionConverter positionConverter, float value)
            : this(positionConverter, (Second)value) { }

        public static implicit operator Pulse(FixedTime value) => value.Pulse;
        public static implicit operator Second(FixedTime value) => value.Second;
        public static implicit operator Sample(FixedTime value) => value.Sample;

        int IComparable<FixedTime>.CompareTo(FixedTime value)
        {
            ValidateEquality(this, value);
            return Value.CompareTo(value.Value);
        }

        bool IEquatable<FixedTime>.Equals(FixedTime value) => Equals(value);
        public override bool Equals(object obj)
        {
            return (obj is FixedTime value) && (Value == value.Value) && (PositionConverter == value.PositionConverter);
        }

        public override int GetHashCode() => GetHashCode();

        public override string ToString()
        {
            return $"Bpm: {Bpm:0.##} / Pulse: {Pulse:0.##} / Second: {Second:0.##} / Sample: {Sample:0.##}";
        }

        public static FixedTime operator +(FixedTime value) => value;
        public static FixedTime operator -(FixedTime value) => new FixedTime(value.PositionConverter, -value.Value);

        private static FixedTime ValidateEquality(FixedTime a, FixedTime b, float result)
        {
            ValidateEquality(a, b);
            return new FixedTime(a.PositionConverter, result);
        }

        private static bool ValidateEquality(FixedTime a, FixedTime b, bool result)
        {
            ValidateEquality(a, b);
            return result;
        }

        private static void ValidateEquality(FixedTime a, FixedTime b)
        {
            if (a.PositionConverter != b.PositionConverter) throw new InvalidOperationException();
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
