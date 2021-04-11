using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace LinearBeats.Time
{
    public readonly struct FixedTime : IComparable, IFormattable, IComparable<FixedTime>, IEquatable<FixedTime>
    {
        public float Bpm { get; }
        public float NormalizedPulse { get; }

        [NotNull] private readonly ITimingConverter _converter;
        private readonly Pulse _pulse;
        private readonly Sample _sample;
        private readonly Second _second;

        private FixedTime([NotNull] ITimingConverter converter) : this() => _converter = converter;

        public FixedTime([NotNull] ITimingConverter converter, Second value) : this(converter)
        {
            _second = value;

            _sample = converter.ToSample(value);
            var timingIndex = _converter.GetTimingIndex(_sample);
            _pulse = converter.ToPulse(_sample, timingIndex);
            Bpm = _converter.GetBpm(timingIndex);
            NormalizedPulse = _converter.Normalize(_pulse, timingIndex);
        }

        public FixedTime([NotNull] ITimingConverter converter, Pulse value) : this(converter)
        {
            _pulse = value;

            var timingIndex = _converter.GetTimingIndex(value);
            _sample = converter.ToSample(_pulse, timingIndex);
            _second = converter.ToSecond(_sample);
            Bpm = _converter.GetBpm(timingIndex);
            NormalizedPulse = _converter.Normalize(_pulse, timingIndex);
        }

        public FixedTime([NotNull] ITimingConverter converter, Sample value) : this(converter)
        {
            _sample = value;

            _second = converter.ToSecond(_sample);
            var timingIndex = _converter.GetTimingIndex(value);
            _pulse = converter.ToPulse(_sample, timingIndex);
            Bpm = _converter.GetBpm(timingIndex);
            NormalizedPulse = _converter.Normalize(_pulse, timingIndex);
        }

        public static implicit operator Pulse(FixedTime right) => right._pulse;
        public static implicit operator Second(FixedTime right) => right._second;
        public static implicit operator Sample(FixedTime right) => right._sample;

        int IComparable.CompareTo([CanBeNull] object obj) =>
            obj is FixedTime right ? CompareTo(right) : throw new InvalidOperationException();

        public int CompareTo(FixedTime right)
        {
            if (!ConverterEquals(this, right)) throw new InvalidOperationException();

            return _sample.CompareTo(right._sample);
        }

        public override bool Equals(object obj) => obj is FixedTime right && Equals(right);
        public bool Equals(FixedTime right) => _sample.Equals(right._sample) && ConverterEquals(this, right);
        public override int GetHashCode() => _sample.GetHashCode();

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
            if (!ConverterEquals(left, right)) throw new InvalidOperationException();

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
            $"Pulse: {format(_pulse)} / Second: {format(_second)} / Sample: {format(_sample)} / Bpm: {format(Bpm)}";

        public static FixedTime operator +(FixedTime right) => right;

        public static FixedTime operator -(FixedTime right) =>
            new FixedTime(right._converter, -right._sample);

        public static FixedTime operator +(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left._sample + right._sample);

        public static FixedTime operator -(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left._sample - right._sample);

        public static bool operator ==(FixedTime left, FixedTime right) => left.Equals(right);
        public static bool operator !=(FixedTime left, FixedTime right) => !left.Equals(right);

        public static bool operator <(FixedTime left, FixedTime right) => left.CompareTo(right) < 0;
        public static bool operator >(FixedTime left, FixedTime right) => left.CompareTo(right) > 0;

        public static bool operator <=(FixedTime left, FixedTime right) => left.CompareTo(right) <= 0;
        public static bool operator >=(FixedTime left, FixedTime right) => left.CompareTo(right) >= 0;
    }
}
