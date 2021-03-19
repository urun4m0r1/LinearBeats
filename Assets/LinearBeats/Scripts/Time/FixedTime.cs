#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct FixedTime : IComparable<FixedTime>, IEquatable<FixedTime>
    {
        public static FixedTime MaxValue = new FixedTime(int.MaxValue, float.MaxValue);
        public static FixedTime MinValue = new FixedTime(int.MinValue, float.MinValue);
        public static FixedTime Zero = new FixedTime(0, 0f);

        public TimingConverter Converter { get; }
        public Pulse Pulse { get; }
        public Second Second { get; }
        public Sample Sample { get; }
        public float Bpm { get; }

        private readonly float _value;

        public FixedTime(Pulse value, TimingConverter converter)
        {
            Converter = converter;

            Pulse = converter.ToPulse(value);
            Second = converter.ToSecond(value);
            Sample = converter.ToSample(value);
            Bpm = converter.GetBpm(value);

            _value = Sample;
        }

        public FixedTime(Second value, TimingConverter converter)
        {
            Converter = converter;

            Pulse = converter.ToPulse(value);
            Second = converter.ToSecond(value);
            Sample = converter.ToSample(value);
            Bpm = converter.GetBpm(value);

            _value = Sample;
        }

        public FixedTime(Sample value, TimingConverter converter)
        {
            Converter = converter;

            Pulse = converter.ToPulse(value);
            Second = converter.ToSecond(value);
            Sample = converter.ToSample(value);
            Bpm = converter.GetBpm(value);

            _value = Sample;
        }

        private FixedTime(int intValue, float floatValue)
        {
            Converter = null;

            Pulse = intValue;
            Second = floatValue;
            Sample = floatValue;
            Bpm = floatValue;

            _value = floatValue;
        }

        public static implicit operator Pulse(FixedTime value) => value.Pulse;
        public static implicit operator Second(FixedTime value) => value.Second;
        public static implicit operator Sample(FixedTime value) => value.Sample;

        int IComparable<FixedTime>.CompareTo(FixedTime value) => _value.CompareTo(value._value);
        bool IEquatable<FixedTime>.Equals(FixedTime value) => (_value == value._value) && (Converter == value.Converter);
        public override bool Equals(object obj) => (obj is FixedTime value) && (_value == value._value);
        public override int GetHashCode() => GetHashCode();
        public override string ToString()
        {
            return $"Bpm: {Bpm:0.##} / Pulse: {Pulse:0.##} / Second: {Second:0.##} / Sample: {Sample:0.##}";
        }

        public static FixedTime operator +(FixedTime value) => value;
        public static FixedTime operator -(FixedTime value)
        {
            return new FixedTime((Sample)(-value._value), value.Converter);
        }

        public static FixedTime operator +(FixedTime a, FixedTime b)
        {
            if (a.Converter == b.Converter) return new FixedTime((Sample)(a._value + b._value), a.Converter);
            else throw new InvalidOperationException();
        }

        public static FixedTime operator -(FixedTime a, FixedTime b)
        {
            if (a.Converter == b.Converter) return new FixedTime((Sample)(a._value - b._value), a.Converter);
            else throw new InvalidOperationException();
        }

        public static FixedTime operator *(FixedTime a, FixedTime b)
        {
            if (a.Converter == b.Converter) return new FixedTime((Sample)(a._value * b._value), a.Converter);
            else throw new InvalidOperationException();
        }

        public static FixedTime operator /(FixedTime a, FixedTime b)
        {
            if (a.Converter == b.Converter) return new FixedTime((Sample)(a._value / b._value), a.Converter);
            else throw new InvalidOperationException();
        }

        public static bool operator ==(FixedTime a, FixedTime b) => a._value == b._value;
        public static bool operator !=(FixedTime a, FixedTime b) => a._value != b._value;

        public static bool operator <(FixedTime a, FixedTime b) => a._value < b._value;
        public static bool operator >(FixedTime a, FixedTime b) => a._value > b._value;

        public static bool operator <=(FixedTime a, FixedTime b) => a._value <= b._value;
        public static bool operator >=(FixedTime a, FixedTime b) => a._value >= b._value;
    }
}
