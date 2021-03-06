#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct Sample : IComparable<Sample>
    {
        private int _value;
        public Sample(int value) => _value = value;
        public static implicit operator int(Sample value) => value._value;
        public static implicit operator Sample(int value) => new Sample { _value = value };
        public static implicit operator Sample(string value) => new Sample { _value = int.Parse(value) };
        int IComparable<Sample>.CompareTo(Sample value) => _value.CompareTo(value);
    }
}
