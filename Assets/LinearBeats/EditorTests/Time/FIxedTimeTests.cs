using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using LinearBeats.Time;
using NUnit.Framework;
using UnityEngine;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class FixedTimeTests : TimingConvertingTests
    {
        private static readonly Pulse Pulse0 = new Pulse(default);
        private static readonly Sample Sample0 = new Sample(default);
        private static readonly Second Second0 = new Second(default);

        private static readonly FixedTime FixedPulse0 = Factory.Create(Pulse0);
        private static readonly FixedTime FixedSample0 = Factory.Create(Sample0);
        private static readonly FixedTime FixedSecond0 = Factory.Create(Second0);


        private static void Iterate([NotNull] Action<float> action)
        {
            for (var i = 0; i < FloatTests.Iteration; ++i) action(FloatTests.RandomFloat);
        }

        [Test]
        public void Should_Get_NormalizedPulse()
        {
            var v0 = Factory.Create(Pulse0);
            Assert.AreEqual(FixedPulse0.NormalizedPulse, v0.NormalizedPulse);
            Iterate(f =>
            {
                var v = Factory.Create((Pulse) f);
                Assert.AreEqual(v.NormalizedPulse, Factory.Create(new Pulse(f)).NormalizedPulse);
            });
        }

        [Test]
        public void Should_Get_Bpm()
        {
            var v0 = Factory.Create(Pulse0);
            Assert.AreEqual(FixedPulse0.Bpm, v0.Bpm);
            Iterate(f =>
            {
                var v = Factory.Create((Pulse) f);
                Assert.AreEqual(v.Bpm, Factory.Create(new Pulse(f)).Bpm);
            });
        }

        [Test]
        public void Should_Cast_To_Pulse()
        {
            Assert.AreEqual(Pulse0, (Pulse) FixedPulse0);
            Assert.AreEqual(Pulse0, (Pulse) FixedSample0);
            Assert.AreEqual(Pulse0, (Pulse) FixedSecond0);
            Iterate(f =>
            {
                Assert.AreEqual(new Pulse(f), (Pulse) Factory.Create((Pulse) f));
                Assert.AreEqual(ToPulse(new Sample(f)), (Pulse) Factory.Create((Sample) f));
                Assert.AreEqual(ToPulse(new Second(f)), (Pulse) Factory.Create((Second) f));
            });
        }

        [Test]
        public void Should_Cast_To_Sample()
        {
            Assert.AreEqual(Sample0, (Sample) FixedPulse0);
            Assert.AreEqual(Sample0, (Sample) FixedSample0);
            Assert.AreEqual(Sample0, (Sample) FixedSecond0);
            Iterate(f =>
            {
                Assert.AreEqual(ToSample(new Pulse(f)), (Sample) Factory.Create((Pulse) f));
                Assert.AreEqual(new Sample(f), (Sample) Factory.Create((Sample) f));
                Assert.AreEqual(ToSample(new Second(f)), (Sample) Factory.Create((Second) f));
            });
        }

        [Test]
        public void Should_Cast_To_Second()
        {
            Assert.AreEqual(Second0, (Second) FixedPulse0);
            Assert.AreEqual(Second0, (Second) FixedSample0);
            Assert.AreEqual(Second0, (Second) FixedSecond0);
            Iterate(f =>
            {
                Assert.AreEqual(ToSecond(new Pulse(f)), (Second) Factory.Create((Pulse) f));
                Assert.AreEqual(ToSecond(new Sample(f)), (Second) Factory.Create((Sample) f));
                Assert.AreEqual(new Second(f), (Second) Factory.Create((Second) f));
            });
        }

        [Test]
        public void Implements_IComparable()
        {
            var v0 = Factory.Create(Pulse0);

            Assert.IsTrue(FixedPulse0.CompareTo(v0) == 0);

            Assert.IsTrue(FixedPulse0.CompareTo(v0) >= 0);
            Assert.IsTrue(FixedPulse0.CompareTo(v0) <= 0);

            Assert.IsTrue((FixedPulse0 as IComparable).CompareTo(v0) >= 0);
            Assert.IsTrue((FixedPulse0 as IComparable).CompareTo(v0) <= 0);

            Assert.IsTrue(FixedPulse0 >= v0);
            Assert.IsTrue(FixedPulse0 <= v0);

            Iterate(f =>
            {
                var delta = Factory.Create((Pulse) FloatTests.Delta);
                var positive = Factory.Create((Pulse) (f - 0.5));
                var negative = Factory.Create((Pulse) (f + 0.5));

                Assert.IsTrue(positive.CompareTo(negative) <= 0);
                Assert.IsTrue(negative.CompareTo(positive) >= 0);
                Assert.IsTrue(positive.CompareTo(negative + delta) < 0);
                Assert.IsTrue(negative.CompareTo(positive - delta) > 0);

                Assert.IsTrue((positive as IComparable).CompareTo(negative) <= 0);
                Assert.IsTrue((positive as IComparable).CompareTo(positive) >= 0);
                Assert.IsTrue((positive as IComparable).CompareTo(negative + delta) < 0);
                Assert.IsTrue((positive as IComparable).CompareTo(positive - delta) > 0);

                Assert.IsTrue(positive <= negative);
                Assert.IsTrue(negative >= positive);
                Assert.IsTrue(positive < negative + delta);
                Assert.IsTrue(negative > positive - delta);
            });
        }

        [Test]
        public void Implements_IEquatable()
        {
            var v0 = Factory.Create(Pulse0);

            Assert.IsTrue(FixedPulse0.Equals(v0));
            Assert.IsTrue(FixedPulse0.Equals(v0 as object));
            Assert.IsTrue(FixedPulse0 == v0);

            Iterate(f =>
            {
                var v = Factory.Create((Pulse) f);
                var p = new Pulse(f);

                Assert.IsTrue(v.Equals(Factory.Create(p)));
                Assert.IsTrue(v.Equals(Factory.Create(p) as object));
                Assert.IsTrue(v == Factory.Create(p));

                Assert.IsFalse(v.Equals(FixedPulse0));
                Assert.IsFalse(v.Equals(FixedPulse0 as object));
                Assert.IsTrue(v != FixedPulse0);
            });
        }

        [Test]
        public void Converter_Comparable()
        {
            var v0 = Factory.Create(Pulse0);

            Assert.IsTrue(FixedTime.ConverterEquals(FixedPulse0, v0));

            Iterate(f =>
            {
                var v = Factory.Create((Pulse) f);

                Assert.IsTrue(FixedTime.ConverterEquals(v, Factory.Create(new Pulse(f))));
                Assert.IsTrue(FixedTime.ConverterEquals(v, FixedPulse0));
            });
        }

        [Test]
        public void Have_Unique_HashCode()
        {
            var hashList = new List<int>();
            Iterate(f => hashList.Add(Factory.Create((Pulse) f).GetHashCode()));
            hashList.Sort();

            var differentHashes = 0;
            var lastHash = hashList[0];

            for (var i = 1; i < hashList.Count; ++i)
            {
                if (hashList[i] == lastHash) continue;

                differentHashes++;
                lastHash = hashList[i];
            }

            Assert.IsTrue(differentHashes >= hashList.Count / 2);
        }

        [Test]
        public void Calculable()
        {
            var v1 = Factory.Create(new Pulse(1));

            Assert.IsTrue(+v1 + -v1 == FixedPulse0);

            Assert.IsTrue(v1 + FixedPulse0 == v1);
            Assert.IsTrue(FixedPulse0 + v1 == v1);

            Assert.IsTrue(v1 - v1 == FixedPulse0);
            Assert.IsTrue(FixedPulse0 - v1 == -v1);
            Assert.IsTrue(v1 - FixedPulse0 == v1);

            Iterate(f =>
            {
                var v = Factory.Create(new Pulse(f));

                Assert.IsTrue(+v + -v == FixedPulse0);

                Assert.IsTrue(v + FixedPulse0 == v);
                Assert.IsTrue(FixedPulse0 + v == v);

                Assert.IsTrue(v - v == FixedPulse0);
                Assert.IsTrue(FixedPulse0 - v == -v);
                Assert.IsTrue(v - FixedPulse0 == v);
            });
        }

        [Test]
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public void Implements_IFormattable()
        {
            var v = Factory.Create(new Pulse(FloatTests.RandomFloat));

            const string format = "F";
            var culture = CultureInfo.CurrentCulture;

            Debug.Log(v.ToString());
            Debug.Log(v.ToString(culture));
            Debug.Log(v.ToString(format));
            Debug.Log(v.ToString(format, culture));
        }
    }
}
