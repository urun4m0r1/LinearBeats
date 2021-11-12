using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace LinearBeats.Script
{
    [InlineProperty] public readonly struct Pulse : IComparable, IFormattable, IComparable<Pulse>, IEquatable<Pulse>
    {
        [ShowInInspector, ReadOnly, HideLabel] private readonly int _value;

        public Pulse(int value) => _value = value;

        public static implicit operator int(Pulse right) => right._value;
        public static implicit operator Pulse(int right) => new Pulse(right);

        public static implicit operator Pulse([NotNull] string right)
        {
            if (int.TryParse(right, out var value)) return value;

            throw new InvalidScriptException($"Unable to parse int from \"{right}\"");
        }

        int IComparable.CompareTo([NotNull] object obj) =>
            obj is Pulse right ? CompareTo(right) : throw new InvalidOperationException("Cannot compare different types");

        public int CompareTo(Pulse right) => _value.CompareTo(right._value);

        public override bool Equals(object obj) => obj is Pulse right && Equals(right);
        public bool Equals(Pulse right) => _value.Equals(right._value);

        public override int GetHashCode() => _value.GetHashCode();

        [NotNull] public override string ToString() => _value.ToString();
        [NotNull] public string ToString(string format) => _value.ToString(format);
        [NotNull] public string ToString(IFormatProvider formatProvider) => _value.ToString(formatProvider);
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
