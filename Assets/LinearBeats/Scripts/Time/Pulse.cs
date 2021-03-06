#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct Pulse : IComparable<Pulse>
    {
        private int _value;
        public Pulse(int value) => _value = value;
        public static implicit operator int(Pulse value) => value._value;
        public static implicit operator Pulse(int value) => new Pulse { _value = value };
        public static implicit operator Pulse(string value) => new Pulse { _value = int.Parse(value) };
        int IComparable<Pulse>.CompareTo(Pulse value) => _value.CompareTo(value);
    }
}
