using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace LinearBeats.Scrolling
{
    [InlineProperty] public readonly struct Position : IComparable, IFormattable, IComparable<Position>, IEquatable<Position>
    {
        [ShowInInspector, ReadOnly, HideLabel] private readonly float _value;

        public Position(float value) => _value = value;

        public static implicit operator float(Position right) => right._value;
        public static implicit operator Position(float right) => new Position(right);

        int IComparable.CompareTo([CanBeNull] object obj) =>
            obj is Position right ? CompareTo(right) : throw new InvalidOperationException("Cannot compare different types");

        public int CompareTo(Position right) => _value.CompareTo(right._value);

        public override bool Equals(object obj) => obj is Position right && Equals(right);
        public bool Equals(Position right) => _value.Equals(right._value);

        public override int GetHashCode() => _value.GetHashCode();

        [NotNull] public override string ToString() => _value.ToString();
        [NotNull] public string ToString(string format) => _value.ToString(format);
        [NotNull] public string ToString(IFormatProvider formatProvider) => _value.ToString(formatProvider);
        public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);

        public static Position operator +(Position right) => right;
        public static Position operator -(Position right) => -right._value;

        public static Position operator +(Position left, Position right) => left._value + right._value;
        public static Position operator -(Position left, Position right) => left._value - right._value;
        public static Position operator *(Position left, Position right) => left._value * right._value;
        public static Position operator /(Position left, Position right) => left._value / right._value;

        public static bool operator ==(Position left, Position right) => left.Equals(right);
        public static bool operator !=(Position left, Position right) => !left.Equals(right);

        public static bool operator <(Position left, Position right) => left.CompareTo(right) < 0;
        public static bool operator >(Position left, Position right) => left.CompareTo(right) > 0;

        public static bool operator <=(Position left, Position right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Position left, Position right) => left.CompareTo(right) >= 0;
    }
}
