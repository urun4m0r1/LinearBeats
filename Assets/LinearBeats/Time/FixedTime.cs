using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using LinearBeats.Visuals;

namespace LinearBeats.Time
{
    public readonly struct FixedTime : IComparable, IComparable<FixedTime>, IEquatable<FixedTime>
    {
        // ReSharper disable MemberCanBePrivate.Global

        internal PositionConverter PositionConverter { get; }
        internal Pulse Pulse { get; }
        internal Second Second { get; }
        internal Sample Sample { get; }
        internal float Position { get; }
        internal float Bpm { get; }

        // ReSharper restore MemberCanBePrivate.Global

        private TimingConverter TimingConverter => PositionConverter.TimingConverter;
        private float Value => Second;

        private FixedTime([NotNull] PositionConverter positionConverter) : this() =>
            PositionConverter = positionConverter ?? throw new ArgumentNullException();

        internal FixedTime([NotNull] PositionConverter positionConverter, Pulse value)
            : this(positionConverter)
        {
            Pulse = TimingConverter.ToPulse(value);
            Second = TimingConverter.ToSecond(value);
            Sample = TimingConverter.ToSample(value);
            Bpm = TimingConverter.GetBpm(value);

            var normalizedPulse = TimingConverter.Normalize(Pulse);
            Position = PositionConverter.ToPosition(normalizedPulse);
        }

        internal FixedTime([NotNull] PositionConverter positionConverter, Second value)
            : this(positionConverter)
        {
            Pulse = TimingConverter.ToPulse(value);
            Second = TimingConverter.ToSecond(value);
            Sample = TimingConverter.ToSample(value);
            Bpm = TimingConverter.GetBpm(value);

            var normalizedPulse = TimingConverter.Normalize(Pulse);
            Position = PositionConverter.ToPosition(normalizedPulse);
        }

        internal FixedTime([NotNull] PositionConverter positionConverter, Sample value)
            : this(positionConverter)
        {
            Pulse = TimingConverter.ToPulse(value);
            Second = TimingConverter.ToSecond(value);
            Sample = TimingConverter.ToSample(value);
            Bpm = TimingConverter.GetBpm(value);

            var normalizedPulse = TimingConverter.Normalize(Pulse);
            Position = PositionConverter.ToPosition(normalizedPulse);
        }

        private FixedTime([NotNull] PositionConverter positionConverter, float value)
            : this(positionConverter, (Second) value)
        {
        }

        public static implicit operator Pulse(FixedTime right) => right.Pulse;
        public static implicit operator Second(FixedTime right) => right.Second;
        public static implicit operator Sample(FixedTime right) => right.Sample;

        int IComparable.CompareTo([CanBeNull] object obj) =>
            obj is FixedTime right ? CompareTo(right) : throw new InvalidOperationException();

        public int CompareTo(FixedTime right)
        {
            if (!ConverterEquals(this, right)) throw new InvalidOperationException();

            return Value.CompareTo(right.Value);
        }

        public override bool Equals(object obj) => obj is FixedTime right && Equals(right);
        public bool Equals(FixedTime right) => Value.Equals(right.Value) && ConverterEquals(this, right);
        public override int GetHashCode() => Value.GetHashCode();

        private static bool ConverterEquals(FixedTime left, FixedTime right)
        {
            var a = left.PositionConverter;
            var b = right.PositionConverter;

            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
            if (ReferenceEquals(a, b)) return true;

            return a.GetType() == b.GetType() && a.Equals(b);
        }

        private static PositionConverter ChooseConverter(FixedTime left, FixedTime right)
        {
            if (!ConverterEquals(left, right)) throw new InvalidOperationException();

            return left.PositionConverter;
        }

        [NotNull]
        public override string ToString()
        {
            const string format = "0.##";
            var provider = CultureInfo.InvariantCulture;

            var bpm = Bpm.ToString(format, provider);
            var pulse = Pulse.ToString(format, provider);
            var second = Second.ToString(format, provider);
            var sample = Sample.ToString(format, provider);

            return $"Bpm: {bpm} / Pulse: {pulse} / Second: {second} / Sample: {sample}";
        }

        public static FixedTime operator +(FixedTime right) => right;

        public static FixedTime operator -(FixedTime right) =>
            new FixedTime(right.PositionConverter, -right.Value);

        public static FixedTime operator +(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left.Value + right.Value);

        public static FixedTime operator -(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left.Value - right.Value);

        public static FixedTime operator *(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left.Value * right.Value);

        public static FixedTime operator /(FixedTime left, FixedTime right) =>
            new FixedTime(ChooseConverter(left, right), left.Value / right.Value);

        public static bool operator ==(FixedTime left, FixedTime right) => left.Equals(right);
        public static bool operator !=(FixedTime left, FixedTime right) => !left.Equals(right);

        public static bool operator <(FixedTime left, FixedTime right) => left.CompareTo(right) < 0;
        public static bool operator >(FixedTime left, FixedTime right) => left.CompareTo(right) > 0;

        public static bool operator <=(FixedTime left, FixedTime right) => left.CompareTo(right) <= 0;
        public static bool operator >=(FixedTime left, FixedTime right) => left.CompareTo(right) >= 0;
    }
}
