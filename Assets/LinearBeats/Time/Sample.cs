using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using LinearBeats.Utils;

namespace LinearBeats.Time
{
    public readonly struct Sample : IComparable, IFormattable, IComparable<Sample>, IEquatable<Sample>, IFloat
    {
        private readonly float _value;

        public Sample(float value) => _value = value;

        public float ToFloat() => _value;
        public static implicit operator float(Sample right) => right._value;
        public static implicit operator Sample(float right) => new Sample(right);

        public static implicit operator Sample([NotNull] string right) => new Sample(float.Parse(right));

        int IComparable.CompareTo([CanBeNull] object obj) =>
            obj is Sample right
                ? CompareTo(right)
                : throw new InvalidOperationException("Cannot compare different types");

        public int CompareTo(Sample right) => _value.CompareTo(right._value);

        public override bool Equals(object obj) => obj is Sample right && Equals(right);
        public bool Equals(Sample right) => _value.Equals(right._value);

        public override int GetHashCode() => _value.GetHashCode();

        [NotNull]
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public override string ToString() => _value.ToString();

        [NotNull]
        public string ToString(string format) => _value.ToString(format);

        [NotNull]
        public string ToString(IFormatProvider formatProvider) => _value.ToString(formatProvider);

        public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);

        public static Sample operator +(Sample right) => right;
        public static Sample operator -(Sample right) => -right._value;

        public static Sample operator +(Sample left, Sample right) => left._value + right._value;
        public static Sample operator -(Sample left, Sample right) => left._value - right._value;
        public static Sample operator *(Sample left, Sample right) => left._value * right._value;
        public static Sample operator /(Sample left, Sample right) => left._value / right._value;

        public static bool operator ==(Sample left, Sample right) => left.Equals(right);
        public static bool operator !=(Sample left, Sample right) => !left.Equals(right);

        public static bool operator <(Sample left, Sample right) => left.CompareTo(right) < 0;
        public static bool operator >(Sample left, Sample right) => left.CompareTo(right) > 0;

        public static bool operator <=(Sample left, Sample right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Sample left, Sample right) => left.CompareTo(right) >= 0;
    }
}
