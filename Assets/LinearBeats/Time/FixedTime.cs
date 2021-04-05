using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;

namespace LinearBeats.Time
{
    public readonly struct FixedTime : IComparable, IComparable<FixedTime>, IEquatable<FixedTime>
    {
        public Pulse Pulse
        {
            get
            {
                if (_second != null) return TimingConverter.ToPulse((Second) _second);
                if (_sample != null) return TimingConverter.ToPulse((Sample) _sample);

                return _pulse ?? default;
            }
        }

        public Second Second
        {
            get
            {
                if (_sample != null) return TimingConverter.ToSecond((Sample) _sample);
                if (_pulse != null) return TimingConverter.ToSecond((Pulse) _pulse);

                return _second ?? default;
            }
        }

        public Sample Sample
        {
            get
            {
                if (_second != null) return TimingConverter.ToSample((Second) _second);
                if (_pulse != null) return TimingConverter.ToSample((Pulse) _pulse);

                return _sample ?? default;
            }
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public float Bpm
        {
            get
            {
                if (_pulse != null) return TimingConverter.GetBpm((Pulse) _pulse);
                if (_second != null) return TimingConverter.GetBpm((Second) _second);
                if (_sample != null) return TimingConverter.GetBpm((Sample) _sample);

                return default;
            }
        }

        public float Position => _positionConverter?.ToPosition(TimingConverter.Normalize(Pulse)) ?? default;


        private TimingConverter TimingConverter => _positionConverter.TimingConverter;
        private readonly PositionConverter _positionConverter;

        private readonly Pulse? _pulse;
        private readonly Second? _second;
        private readonly Sample? _sample;

        private float Value => Second;

        private FixedTime([CanBeNull] PositionConverter positionConverter)
            : this() => _positionConverter = positionConverter;

        public FixedTime([NotNull] PositionConverter positionConverter, Pulse value)
            : this(positionConverter) => _pulse = value;

        public FixedTime([NotNull] PositionConverter positionConverter, Second value)
            : this(positionConverter) => _second = value;

        public FixedTime([NotNull] PositionConverter positionConverter, Sample value)
            : this(positionConverter) => _sample = value;

        private FixedTime([NotNull] PositionConverter positionConverter, float value)
            : this(positionConverter) => _second = value;

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
            var a = left._positionConverter;
            var b = right._positionConverter;

            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
            if (ReferenceEquals(a, b)) return true;

            return a.GetType() == b.GetType() && a.Equals(b);
        }

        private static PositionConverter ChooseConverter(FixedTime left, FixedTime right)
        {
            if (!ConverterEquals(left, right)) throw new InvalidOperationException();

            return left._positionConverter;
        }

        [NotNull]
        public override string ToString()
        {
            const string format = "0.##";
            var provider = CultureInfo.InvariantCulture;

            var pulse = Pulse.ToString(format, provider);
            var second = Second.ToString(format, provider);
            var sample = Sample.ToString(format, provider);
            var bpm = Bpm.ToString(format, provider);
            var position = Position.ToString(format, provider);

            return $"Pulse: {pulse} / Second: {second} / Sample: {sample} / Bpm: {bpm} / Position: {position}";
        }

        public static FixedTime operator +(FixedTime right) => right;

        public static FixedTime operator -(FixedTime right) =>
            new FixedTime(right._positionConverter, -right.Value);

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
