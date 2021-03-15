#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct Pulse : IComparable<Pulse>, IEquatable<Pulse>
    {
        private readonly int _value;

        public Pulse(int value) => _value = value;

        public static implicit operator int(Pulse value) => value._value;

        public static implicit operator Pulse(int value) => new Pulse(value);
        public static implicit operator Pulse(string value) => new Pulse(int.Parse(value));

        int IComparable<Pulse>.CompareTo(Pulse value) => _value.CompareTo(value._value);
        bool IEquatable<Pulse>.Equals(Pulse value) => _value == value._value;
        public override bool Equals(object obj) => (obj is Pulse value) && (_value == value._value);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value.ToString();
    }
}
