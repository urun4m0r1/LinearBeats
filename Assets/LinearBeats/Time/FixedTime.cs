using System;
using System.Globalization;
using JetBrains.Annotations;

namespace LinearBeats.Time
{
    public readonly struct FixedTime : IComparable, IComparable<FixedTime>, IEquatable<FixedTime>
    {
        public float Position { get; }
        public float Bpm { get; }

        private readonly IPositionConverter _converter;
        private readonly Pulse _pulse;
        private readonly Sample _sample;
        private readonly Second _second;

        private FixedTime([NotNull] IPositionConverter converter) : this() =>
            _converter = converter ?? throw new ArgumentNullException();

        public FixedTime([NotNull] IPositionConverter converter, Second value) : this(converter)
        {
            _second = value;

            _sample = converter.ToSample(value);
            var timingIndex = _converter.GetTimingIndex(_sample);
            _pulse = converter.ToPulse(_sample, timingIndex);

            Position = _converter.ToPosition(_converter.Normalize(_pulse, timingIndex));
            Bpm = _converter.GetBpm(timingIndex);
        }

        public FixedTime([NotNull] IPositionConverter converter, Pulse value) : this(converter)
        {
            _pulse = value;

            var timingIndex = _converter.GetTimingIndex(value);
            _sample = converter.ToSample(_pulse, timingIndex);
            _second = converter.ToSecond(_sample);

            Position = _converter.ToPosition(_converter.Normalize(_pulse, timingIndex));
            Bpm = _converter.GetBpm(timingIndex);
        }

        public FixedTime([NotNull] IPositionConverter converter, Sample value) : this(converter)
        {
            _sample = value;

            _second = converter.ToSecond(_sample);
            var timingIndex = _converter.GetTimingIndex(value);
            _pulse = converter.ToPulse(_sample, timingIndex);

            Position = _converter.ToPosition(_converter.Normalize(_pulse, timingIndex));
            Bpm = _converter.GetBpm(timingIndex);
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
        private static IPositionConverter ChooseConverter(FixedTime left, FixedTime right)
        {
            if (!ConverterEquals(left, right)) throw new InvalidOperationException();

            return left._converter;
        }

        [NotNull]
        public override string ToString()
        {
            const string format = "0.##";
            var provider = CultureInfo.InvariantCulture;

            var pulse = _pulse.ToString(format, provider);
            var second = _second.ToString(format, provider);
            var sample = _sample.ToString(format, provider);
            var bpm = Bpm.ToString(format, provider);
            var position = Position.ToString(format, provider);

            return $"Pulse: {pulse} / Second: {second} / Sample: {sample} / Bpm: {bpm} / Position: {position}";
        }

        public static FixedTime operator +(FixedTime right) => right;

        public static FixedTime operator -(FixedTime right) =>
            new FixedTime(right._converter, -right._sample);

        public static FixedTime operator +(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left._sample + right._sample);

        public static FixedTime operator -(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left._sample - right._sample);

        public static FixedTime operator *(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left._sample * right._sample);

        public static FixedTime operator /(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left._sample / right._sample);

        public static bool operator ==(FixedTime left, FixedTime right) => left.Equals(right);
        public static bool operator !=(FixedTime left, FixedTime right) => !left.Equals(right);

        public static bool operator <(FixedTime left, FixedTime right) => left.CompareTo(right) < 0;
        public static bool operator >(FixedTime left, FixedTime right) => left.CompareTo(right) > 0;

        public static bool operator <=(FixedTime left, FixedTime right) => left.CompareTo(right) <= 0;
        public static bool operator >=(FixedTime left, FixedTime right) => left.CompareTo(right) >= 0;
    }
}
