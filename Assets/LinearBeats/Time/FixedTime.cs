using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using LinearBeats.Script;

namespace LinearBeats.Time
{
    [Serializable]
    public readonly struct FixedTime : IComparable, IFormattable, IComparable<FixedTime>, IEquatable<FixedTime>
    {
        [NotNull] private readonly ITimingConverter _converter;
        public float Bpm { get; }
        public Pulse Pulse { get; }
        public Sample Sample { get; }
        public Second Second { get; }

        private FixedTime([NotNull] ITimingConverter converter) : this() => _converter = converter;

        private FixedTime([NotNull] ITimingConverter converter, Second value) : this(converter)
        {
            Second = value;

            Sample = converter.ToSample(value);
            var timingIndex = _converter.GetTimingIndex(Sample);
            Pulse = converter.ToPulse(Sample, timingIndex);
            Bpm = _converter.GetBpm(timingIndex);
        }

        private FixedTime([NotNull] ITimingConverter converter, Pulse value) : this(converter)
        {
            Pulse = value;

            var timingIndex = _converter.GetTimingIndex(value);
            Sample = converter.ToSample(Pulse, timingIndex);
            Second = converter.ToSecond(Sample);
            Bpm = _converter.GetBpm(timingIndex);
        }

        private FixedTime([NotNull] ITimingConverter converter, Sample value) : this(converter)
        {
            Sample = value;

            Second = converter.ToSecond(Sample);
            var timingIndex = _converter.GetTimingIndex(value);
            Pulse = converter.ToPulse(Sample, timingIndex);
            Bpm = _converter.GetBpm(timingIndex);
        }

        [Serializable]
        public sealed class Factory
        {
            [NotNull] private readonly ITimingConverter _converter;

            public Factory([NotNull] ITimingConverter converter) => _converter = converter;

            public FixedTime Create(Pulse value) => new FixedTime(_converter, value);
            public FixedTime Create(Sample value) => new FixedTime(_converter, value);
            public FixedTime Create(Second value) => new FixedTime(_converter, value);
        }

        public static implicit operator Pulse(FixedTime right) => right.Pulse;
        public static implicit operator Second(FixedTime right) => right.Second;
        public static implicit operator Sample(FixedTime right) => right.Sample;

        int IComparable.CompareTo([CanBeNull] object obj) =>
            obj is FixedTime right
                ? CompareTo(right)
                : throw new InvalidOperationException("Cannot compare different types");

        public int CompareTo(FixedTime right)
        {
            if (!ConverterEquals(this, right))
                throw new InvalidOperationException("TimingConverter mismatch");

            return Sample.CompareTo(right.Sample);
        }

        public override bool Equals(object obj) => obj is FixedTime right && Equals(right);
        public bool Equals(FixedTime right) => Sample.Equals(right.Sample) && ConverterEquals(this, right);
        public override int GetHashCode() => Sample.GetHashCode();

        public static bool ConverterEquals(FixedTime left, FixedTime right)
        {
            var a = left._converter;
            var b = right._converter;

            if (ReferenceEquals(a, b)) return true;

            return a.GetType() == b.GetType() && a.Equals(b);
        }

        [NotNull]
        private static ITimingConverter ChooseConverter(FixedTime left, FixedTime right)
        {
            if (!ConverterEquals(left, right))
                throw new InvalidOperationException("TimingConverter mismatch");

            return left._converter;
        }

        [NotNull]
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public override string ToString() => GetString(v => v.ToString());

        [NotNull]
        public string ToString(string format) => GetString(v => v.ToString(format));

        [NotNull]
        public string ToString(IFormatProvider formatProvider) => GetString(v => v.ToString(formatProvider));

        public string ToString(string format, IFormatProvider formatProvider) =>
            GetString(v => v.ToString(format, formatProvider));

        [NotNull]
        private string GetString([NotNull] Func<float, string> format) =>
            $"Pulse: {format(Pulse)} / Second: {format(Second)} / Sample: {format(Sample)} / Bpm: {format(Bpm)}";

        public static FixedTime operator +(FixedTime right) => right;

        public static FixedTime operator -(FixedTime right) =>
            new FixedTime(right._converter, -right.Sample);

        public static FixedTime operator +(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left.Sample + right.Sample);

        public static FixedTime operator -(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left.Sample - right.Sample);

        public static bool operator ==(FixedTime left, FixedTime right) => left.Equals(right);
        public static bool operator !=(FixedTime left, FixedTime right) => !left.Equals(right);

        public static bool operator <(FixedTime left, FixedTime right) => left.CompareTo(right) < 0;
        public static bool operator >(FixedTime left, FixedTime right) => left.CompareTo(right) > 0;

        public static bool operator <=(FixedTime left, FixedTime right) => left.CompareTo(right) <= 0;
        public static bool operator >=(FixedTime left, FixedTime right) => left.CompareTo(right) >= 0;
    }
}
