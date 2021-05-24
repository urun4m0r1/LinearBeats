using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using LinearBeats.Utils;
using Sirenix.OdinInspector;

namespace LinearBeats.Script
{
    [Serializable]
    public readonly struct Pulse : IComparable, IFormattable, IComparable<Pulse>, IEquatable<Pulse>, IFloat
    {
        [ShowInInspector] private readonly float _value;

        public Pulse(float value) => _value = value;

        public float ToFloat() => _value;
        public static implicit operator float(Pulse right) => right._value;
        public static implicit operator Pulse(float right) => new Pulse(right);

        public static implicit operator Pulse([NotNull] string right)
        {
            if (float.TryParse(right, out var value)) return value;

            throw new InvalidScriptException($"Unable to parse float from \"{right}\"");
        }

        int IComparable.CompareTo([CanBeNull] object obj) =>
            obj is Pulse right
                ? CompareTo(right)
                : throw new InvalidOperationException("Cannot compare different types");

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
