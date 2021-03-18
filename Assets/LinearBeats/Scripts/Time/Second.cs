#pragma warning disable IDE0090

using System;
using UnityEngine;

namespace LinearBeats.Time
{
    public struct Second : IComparable<Second>, IEquatable<Second>
    {
        [SerializeField]
        private readonly float _value;

        public Second(float value) => _value = value;

        public static implicit operator float(Second value) => value._value;

        public static implicit operator Second(float value) => new Second(value);
        public static implicit operator Second(string value) => new Second(float.Parse(value));

        int IComparable<Second>.CompareTo(Second value) => _value.CompareTo(value._value);
        bool IEquatable<Second>.Equals(Second value) => _value == value._value;
        public override bool Equals(object obj) => (obj is Second value) && (_value == value._value);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value.ToString();
    }
}
