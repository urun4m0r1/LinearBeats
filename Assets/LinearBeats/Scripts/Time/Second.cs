#pragma warning disable IDE0090

using System;

namespace LinearBeats.Time
{
    public struct Second : IComparable<Second>
    {
        private readonly float _value;
        public Second(float value) => _value = value;
        public static implicit operator float(Second value) => value._value;
        public static implicit operator Second(float value) => new Second(value);
        public static implicit operator Second(string value) => new Second(float.Parse(value));
        int IComparable<Second>.CompareTo(Second value) => _value.CompareTo(value);
        public override string ToString() => _value.ToString();
    }
}
