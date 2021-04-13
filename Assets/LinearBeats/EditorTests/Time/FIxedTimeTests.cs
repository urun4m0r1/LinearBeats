using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using LinearBeats.Time;
using NUnit.Framework;
using UnityEngine;
using static LinearBeats.EditorTests.FloatTests;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public class FixedTimeTests
    {
        [NotNull] private static readonly BpmEvent[] BpmEvents = {new BpmEvent {Ppqn = 100, Pulse = 0, Bpm = 120}};
        [NotNull] private static readonly ITimingConverter Converter = new TimingConverter(BpmEvents, 120, 1000);
        [NotNull] public static readonly FixedTime.Factory Factory = new FixedTime.Factory(Converter);

        private static readonly Pulse Pulse0 = new Pulse(default);
        private static readonly Sample Sample0 = new Sample(default);
        private static readonly Second Second0 = new Second(default);

        private static readonly FixedTime V0 = Factory.Create(Pulse0);

        private static Sample ToSample(Pulse v) => Converter.ToSample(v, Converter.GetTimingIndex(v));
        private static Pulse ToPulse(Sample v) => Converter.ToPulse(v, Converter.GetTimingIndex(v));
        private static Second ToSecond(Sample v) => Converter.ToSecond(v);
        private static Sample ToSample(Second v) => Converter.ToSample(v);

        private static Second ToSecond(Pulse v)
        {
            var sample = Converter.ToSample(v, Converter.GetTimingIndex(v));
            return Converter.ToSecond(sample);
        }

        private static Pulse ToPulse(Second v)
        {
            var sample = Converter.ToSample(v);
            return Converter.ToPulse(sample, Converter.GetTimingIndex(sample));
        }

        private static void Iterate([NotNull] Action<float, FixedTime> action)
        {
            for (var i = 0; i < Iteration; ++i)
            {
                var randomFloat = RandomFloat;
                action(randomFloat, Factory.Create(new Pulse(randomFloat)));
            }
        }

        private static void Iterate([NotNull] Action<Pulse, Sample, Second> action)
        {
            for (var i = 0; i < Iteration; ++i)
                action(new Pulse(RandomFloat), new Sample(RandomFloat), new Second(RandomFloat));
        }

        [Test]
        public void Should_Cast_To_Pulse()
        {
            Assert.AreEqual(Pulse0, (Pulse) Factory.Create(Pulse0));
            Assert.AreEqual(Pulse0, (Pulse) Factory.Create(Sample0));
            Assert.AreEqual(Pulse0, (Pulse) Factory.Create(Second0));

            Iterate((pulse, sample, second) =>
            {
                Assert.AreEqual(pulse, (Pulse) Factory.Create(pulse));
                Assert.AreEqual(ToPulse(sample), (Pulse) Factory.Create(sample));
                Assert.AreEqual(ToPulse(second), (Pulse) Factory.Create(second));
            });
        }

        [Test]
        public void Should_Cast_To_Sample()
        {
            Assert.AreEqual(Sample0, (Sample) Factory.Create(Pulse0));
            Assert.AreEqual(Sample0, (Sample) Factory.Create(Sample0));
            Assert.AreEqual(Sample0, (Sample) Factory.Create(Second0));

            Iterate((pulse, sample, second) =>
            {
                Assert.AreEqual(ToSample(pulse), (Sample) Factory.Create(pulse));
                Assert.AreEqual(sample, (Sample) Factory.Create(sample));
                Assert.AreEqual(ToSample(second), (Sample) Factory.Create(second));
            });
        }

        [Test]
        public void Should_Cast_To_Second()
        {
            Assert.AreEqual(Second0, (Second) Factory.Create(Pulse0));
            Assert.AreEqual(Second0, (Second) Factory.Create(Sample0));
            Assert.AreEqual(Second0, (Second) Factory.Create(Second0));

            Iterate((pulse, sample, second) =>
            {
                Assert.AreEqual(ToSecond(pulse), (Second) Factory.Create(pulse));
                Assert.AreEqual(ToSecond(sample), (Second) Factory.Create(sample));
                Assert.AreEqual(second, (Second) Factory.Create(second));
            });
        }

        [Test]
        public void Have_Unique_HashCode()
        {
            var hashList = new List<int>();

            Iterate((_, v) => hashList.Add(v.GetHashCode()));
            hashList.Sort();

            var differentHashes = 0;
            var lastHash = hashList.First();
            foreach (var hash in hashList.Where(hash => hash != lastHash))
            {
                differentHashes++;
                lastHash = hash;
            }

            Assert.IsTrue(differentHashes >= hashList.Count / 2);
        }

        [Test]
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public void Implements_IFormattable()
        {
            var v = Factory.Create(new Pulse(RandomFloat));

            const string format = "F";
            var culture = CultureInfo.CurrentCulture;

            Debug.Log(v.ToString());
            Debug.Log(v.ToString(culture));
            Debug.Log(v.ToString(format));
            Debug.Log(v.ToString(format, culture));
        }

        [Test]
        public void Implements_IEquatable()
        {
            var v0 = Factory.Create(Pulse0);
            AssertEquatable(v0, V0);

            Iterate((f, v) =>
            {
                var w = Factory.Create(new Pulse(f));
                AssertEquatable(v, w);

                if (v == V0) return;

                Assert.IsFalse(v.Equals(V0));
                Assert.IsFalse(v.Equals(V0 as object));
                Assert.IsTrue(v != V0);
            });

            static void AssertEquatable(FixedTime v, FixedTime w)
            {
                Assert.IsTrue(v.Equals(w));
                Assert.IsTrue(v.Equals(w as object));
                Assert.IsTrue(v == w);
            }
        }

        [Test]
        public void Implements_IComparable()
        {
            var v0 = Factory.Create(Pulse0);
            AssertEquatable(v0, V0);

            Iterate((f, v) =>
            {
                var vx = Factory.Create(new Pulse(f)); // [-0.5 ~ 0.5]
                AssertEquatable(v, vx);

                var vn = Factory.Create(new Pulse(f - 0.5f)); // [-1, 0]
                var vp = Factory.Create(new Pulse(f + 0.5f)); // [0, 1]
                AssertRightIsBigger(vn, vp);
                AssertRightIsBigger(v, vp);
                AssertRightIsBigger(vn, v);
            });

            static void AssertEquatable(FixedTime left, FixedTime right)
            {
                Assert.IsTrue(left.CompareTo(right) == 0);

                Assert.IsTrue(left.CompareTo(right) >= 0);
                Assert.IsTrue(left.CompareTo(right) <= 0);

                Assert.IsTrue((left as IComparable).CompareTo(right) >= 0);
                Assert.IsTrue((left as IComparable).CompareTo(right) <= 0);

                Assert.IsTrue(left >= right);
                Assert.IsTrue(left <= right);
            }

            static void AssertRightIsBigger(FixedTime l, FixedTime r)
            {
                var delta = Factory.Create(new Pulse(Delta));

                Assert.IsTrue(r.CompareTo(l) >= 0);
                Assert.IsTrue(r.CompareTo(l - delta) > 0);
                Assert.IsTrue((r as IComparable).CompareTo(l) >= 0);
                Assert.IsTrue((r as IComparable).CompareTo(l - delta) > 0);
                Assert.IsTrue(r >= l);
                Assert.IsTrue(r > l - delta);

                Assert.IsTrue(l.CompareTo(r) <= 0);
                Assert.IsTrue(l.CompareTo(r + delta) < 0);
                Assert.IsTrue((l as IComparable).CompareTo(r) <= 0);
                Assert.IsTrue((l as IComparable).CompareTo(r + delta) < 0);
                Assert.IsTrue(l <= r);
                Assert.IsTrue(l < r + delta);
            }
        }

        [Test]
        public void Calculable()
        {
            var v1 = Factory.Create(new Pulse(1f));
            AssertCalculation(V0);
            AssertCalculation(v1);

            Iterate((_, v) => AssertCalculation(v));

            static void AssertCalculation(FixedTime v)
            {
                var v1 = Factory.Create(new Pulse(1f));

                Assert.IsTrue(+v == V0 + v); // 양수
                Assert.IsTrue(-v == V0 - v); // 음수

                Assert.IsTrue(v + -v == V0); // 역원
                Assert.IsTrue(v + V0 == v); // 항등원
                Assert.IsTrue(v + v1 == v1 + v); // 교환법칙

                Assert.IsTrue(v - v == V0); // 역원
                Assert.IsTrue(v - V0 == v); // 항등원
                if (v != v1) Assert.IsTrue(v - v1 != v1 - v); // 교환법칙
            }
        }

        [Test]
        public void Converter_Comparable()
        {
            var v0 = Factory.Create(Pulse0);

            Assert.IsTrue(FixedTime.ConverterEquals(v0, V0));

            Iterate((f, v) =>
            {
                var vx = Factory.Create(new Pulse(f));

                Assert.IsTrue(FixedTime.ConverterEquals(v, vx));
                Assert.IsTrue(FixedTime.ConverterEquals(v, V0));
                Assert.IsTrue(FixedTime.ConverterEquals(V0, vx));
            });
        }

        [Test]
        public void Should_Get_NormalizedPulse()
        {
            var v0 = Factory.Create(Pulse0);
            Assert.AreEqual(v0.NormalizedPulse, V0.NormalizedPulse);

            Iterate((f, v) =>
            {
                var vx = Factory.Create(new Pulse(f));

                Assert.AreEqual(v.NormalizedPulse, vx.NormalizedPulse);
            });
        }

        [Test]
        public void Should_Get_Bpm()
        {
            var v0 = Factory.Create(Pulse0);
            Assert.AreEqual(v0.Bpm, V0.Bpm);

            Iterate((f, v) =>
            {
                var vx = Factory.Create(new Pulse(f));

                Assert.AreEqual(v.Bpm, vx.Bpm);
            });
        }
    }
}
