using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using LinearBeats.Utils;

namespace LinearBeats.Time
{
    public readonly struct Second : IComparable, IFormattable, IComparable<Second>, IEquatable<Second>, IFloat
    {
        private readonly float _value;

        public Second(float value) => _value = value;

        public float ToFloat() => _value;
        public static implicit operator float(Second right) => right._value;
        public static implicit operator Second(float right) => new Second(right);

        public static implicit operator Second([NotNull] string right) => new Second(float.Parse(right));

        int IComparable.CompareTo([CanBeNull] object obj) =>
            obj is Second right
                ? CompareTo(right)
                : throw new InvalidOperationException("Cannot compare different types");

        public int CompareTo(Second right) => _value.CompareTo(right._value);

        public override bool Equals(object obj) => obj is Second right && Equals(right);
        public bool Equals(Second right) => _value.Equals(right._value);

        public override int GetHashCode() => _value.GetHashCode();

        [NotNull]
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public override string ToString() => _value.ToString();

        [NotNull]
        public string ToString(string format) => _value.ToString(format);

        [NotNull]
        public string ToString(IFormatProvider formatProvider) => _value.ToString(formatProvider);

        public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);

        public static Second operator +(Second right) => right;
        public static Second operator -(Second right) => -right._value;

        public static Second operator +(Second left, Second right) => left._value + right._value;
        public static Second operator -(Second left, Second right) => left._value - right._value;
        public static Second operator *(Second left, Second right) => left._value * right._value;
        public static Second operator /(Second left, Second right) => left._value / right._value;

        public static bool operator ==(Second left, Second right) => left.Equals(right);
        public static bool operator !=(Second left, Second right) => !left.Equals(right);

        public static bool operator <(Second left, Second right) => left.CompareTo(right) < 0;
        public static bool operator >(Second left, Second right) => left.CompareTo(right) > 0;

        public static bool operator <=(Second left, Second right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Second left, Second right) => left.CompareTo(right) >= 0;
    }
}
