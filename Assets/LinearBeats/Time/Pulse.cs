using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace LinearBeats.Time
{
    public readonly struct Pulse : IComparable, IFormattable, IComparable<Pulse>, IEquatable<Pulse>
    {
        private readonly float _value;

        public Pulse(float value) => _value = value;

        public static implicit operator float(Pulse right) => right._value;
        public static implicit operator Pulse(float right) => new Pulse(right);

        public static implicit operator Pulse([NotNull] string right) => new Pulse(float.Parse(right));

        int IComparable.CompareTo([CanBeNull] object obj)
        {
            if (obj is Pulse right) return CompareTo(right);

            throw new InvalidOperationException();
        }

        public int CompareTo(Pulse right) => _value.CompareTo(right._value);

        public override bool Equals(object obj) => obj is Pulse right && Equals(right);
        public bool Equals(Pulse right) => _value.Equals(right._value);

        public override int GetHashCode() => _value.GetHashCode();

        [NotNull]
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public override string ToString() => _value.ToString();

        [NotNull]
        public string ToString(string format) => _value.ToString(format);

        [NotNull]
        public string ToString(IFormatProvider formatProvider) => _value.ToString(formatProvider);

        public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);

        public static Pulse operator +(Pulse right) => right;
        public static Pulse operator -(Pulse right) => -right._value;

        public static Pulse operator +(Pulse left, Pulse right) => left._value + right._value;
        public static Pulse operator -(Pulse left, Pulse right) => left._value - right._value;
        public static Pulse operator *(Pulse left, Pulse right) => left._value * right._value;
        public static Pulse operator /(Pulse left, Pulse right) => left._value / right._value;

        public static bool operator ==(Pulse left, Pulse right) => left.Equals(right);
        public static bool operator !=(Pulse left, Pulse right) => !left.Equals(right);

        public static bool operator <(Pulse left, Pulse right) => left.CompareTo(right) < 0;
        public static bool operator >(Pulse left, Pulse right) => left.CompareTo(right) > 0;

        public static bool operator <=(Pulse left, Pulse right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Pulse left, Pulse right) => left.CompareTo(right) >= 0;
    }
}
