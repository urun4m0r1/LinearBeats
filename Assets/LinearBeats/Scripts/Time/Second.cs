#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct Second : IComparable<Second>, IEquatable<Second>
    {
        private readonly float _value;

        public Second(float value) => _value = value;

        public static implicit operator float(Second value) => value._value;

        public static implicit operator Second(float value) => new Second(value);
        public static implicit operator Second(string value) => new Second(float.Parse(value));

        int IComparable<Second>.CompareTo(Second value) => value._value.CompareTo(_value);
        bool IEquatable<Second>.Equals(Second value) => value.Equals(this);
        public override bool Equals(object obj) => (obj is Second value) && (value._value == _value);
        public override int GetHashCode() => GetHashCode();
        public override string ToString() => _value.ToString();

        public static Second operator +(Second value) => value;
        public static Second operator -(Second value) => -value._value;

        public static Second operator +(Second a, Second b) => a._value + b._value;
        public static Second operator -(Second a, Second b) => a._value - b._value;
        public static Second operator *(Second a, Second b) => a._value * b._value;
        public static Second operator /(Second a, Second b) => a._value / b._value;

        public static bool operator ==(Second a, Second b) => a._value == b._value;
        public static bool operator !=(Second a, Second b) => a._value != b._value;

        public static bool operator <(Second a, Second b) => a._value < b._value;
        public static bool operator >(Second a, Second b) => a._value > b._value;

        public static bool operator <=(Second a, Second b) => a._value <= b._value;
        public static bool operator >=(Second a, Second b) => a._value >= b._value;
    }
}
