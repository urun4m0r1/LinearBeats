#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct Sample : IComparable<Sample>, IEquatable<Sample>
    {
        private readonly float _value;

        public Sample(float value) => _value = value;

        public static implicit operator float(Sample value) => value._value;

        public static implicit operator Sample(float value) => new Sample(value);
        public static implicit operator Sample(string value) => new Sample(float.Parse(value));

        int IComparable<Sample>.CompareTo(Sample value) => _value.CompareTo(value._value);
        bool IEquatable<Sample>.Equals(Sample value) => _value == value._value;
        public override bool Equals(object obj) => (obj is Sample value) && (_value == value._value);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value.ToString();
    }
}
