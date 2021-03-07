#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct Sample : IComparable<Sample>
    {
        private float _value;
        public Sample(float value) => _value = value;
        public static implicit operator float(Sample value) => value._value;
        public static implicit operator Sample(float value) => new Sample { _value = value };
        public static implicit operator Sample(string value) => new Sample { _value = float.Parse(value) };
        int IComparable<Sample>.CompareTo(Sample value) => _value.CompareTo(value);
        public override string ToString() => _value.ToString();
    }
}
