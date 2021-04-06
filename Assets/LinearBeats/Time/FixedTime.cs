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
                if (_second != null) return _converter?.ToPulse((Second) _second) ?? default;
                if (_sample != null) return _converter?.ToPulse((Sample) _sample) ?? default;

                return _pulse ?? default;
            }
        }

        public Second Second
        {
            get
            {
                if (_sample != null) return _converter?.ToSecond((Sample) _sample) ?? default;
                if (_pulse != null) return _converter?.ToSecond((Pulse) _pulse) ?? default;

                return _second ?? default;
            }
        }

        public Sample Sample
        {
            get
            {
                if (_second != null) return _converter?.ToSample((Second) _second) ?? default;
                if (_pulse != null) return _converter?.ToSample((Pulse) _pulse) ?? default;

                return _sample ?? default;
            }
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public float Bpm
        {
            get
            {
                if (_pulse != null) return _converter?.GetBpm((Pulse) _pulse) ?? default;
                if (_second != null) return _converter?.GetBpm((Second) _second) ?? default;
                if (_sample != null) return _converter?.GetBpm((Sample) _sample) ?? default;

                return default;
            }
        }

        public float Position => _converter?.ToPosition((Pulse) _converter?.Normalize(Pulse)) ?? default;

        private readonly IPositionConverter _converter;
        private readonly Pulse? _pulse;
        private readonly Second? _second;
        private readonly Sample? _sample;
        private float Value => Second;

        private FixedTime([CanBeNull] IPositionConverter converter) : this() => _converter = converter;
        public FixedTime(IPositionConverter converter, Pulse value) : this(converter) => _pulse = value;
        public FixedTime(IPositionConverter converter, Second value) : this(converter) => _second = value;
        public FixedTime(IPositionConverter converter, Sample value) : this(converter) => _sample = value;
        private FixedTime(IPositionConverter converter, float value) : this(converter) => _second = value;

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

        public static bool ConverterEquals(FixedTime left, FixedTime right)
        {
            var a = left._converter;
            var b = right._converter;

            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
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

            var pulse = Pulse.ToString(format, provider);
            var second = Second.ToString(format, provider);
            var sample = Sample.ToString(format, provider);
            var bpm = Bpm.ToString(format, provider);
            var position = Position.ToString(format, provider);

            return $"Pulse: {pulse} / Second: {second} / Sample: {sample} / Bpm: {bpm} / Position: {position}";
        }

        public static FixedTime operator +(FixedTime right) => right;

        public static FixedTime operator -(FixedTime right) =>
            new FixedTime(right._converter, -right.Value);

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
