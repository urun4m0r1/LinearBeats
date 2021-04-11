using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using LinearBeats.Time;
using NUnit.Framework;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class PulseTests
    {
        private readonly Pulse _v0 = new Pulse(default);

        private static void Iterate([NotNull] Action<float, Pulse> action)
        {
            for (var i = 0; i < FloatTests.Iteration; ++i)
            {
                var randomFloat = FloatTests.RandomFloat;
                action(randomFloat, new Pulse(randomFloat));
            }
        }

        [Test]
        public void Implements_IFloat()
        {
            Assert.AreEqual(FloatTests.F0, _v0.ToFloat());
            Iterate((f, v) => Assert.AreEqual(f, v.ToFloat()));
        }

        [Test]
        public void Should_Cast_To_Float()
        {
            Assert.AreEqual(FloatTests.F0, (float) _v0);
            Iterate((f, v) => Assert.AreEqual(f, (float) v));
        }

        [Test]
        public void Should_Cast_From_Float()
        {
            Assert.AreEqual(_v0, (Pulse) FloatTests.F0);
            Iterate((f, v) => Assert.AreEqual(v, (Pulse) f));
        }

        [Test]
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public void Should_Cast_From_String()
        {
            Assert.AreEqual(_v0, (Pulse) FloatTests.F0.ToString());

            Iterate((f, v) =>
            {
                var rand = (float) Math.Round(f, FloatTests.Digits);
                Assert.AreEqual(new Pulse(rand), (Pulse) rand.ToString());
            });
        }

        [Test]
        public void Implements_IComparable()
        {
            var v0 = new Pulse(default);

            Assert.IsTrue(_v0.CompareTo(v0) == 0);

            Assert.IsTrue(_v0.CompareTo(v0) >= 0);
            Assert.IsTrue(_v0.CompareTo(v0) <= 0);

            Assert.IsTrue((_v0 as IComparable).CompareTo(v0) >= 0);
            Assert.IsTrue((_v0 as IComparable).CompareTo(v0) <= 0);

            Assert.IsTrue(_v0 >= v0);
            Assert.IsTrue(_v0 <= v0);

            Iterate((f, v) =>
            {
                var positive = new Pulse(f - 0.5f);
                var negative = new Pulse(f + 0.5f);

                Assert.IsTrue(positive.CompareTo(negative) <= 0);
                Assert.IsTrue(negative.CompareTo(positive) >= 0);
                Assert.IsTrue(positive.CompareTo(negative + FloatTests.Delta) < 0);
                Assert.IsTrue(negative.CompareTo(positive - FloatTests.Delta) > 0);

                Assert.IsTrue((positive as IComparable).CompareTo(negative) <= 0);
                Assert.IsTrue((positive as IComparable).CompareTo(positive) >= 0);
                Assert.IsTrue((positive as IComparable).CompareTo(negative + FloatTests.Delta) < 0);
                Assert.IsTrue((positive as IComparable).CompareTo(positive - FloatTests.Delta) > 0);

                Assert.IsTrue(positive <= negative);
                Assert.IsTrue(negative >= positive);
                Assert.IsTrue(positive < negative + FloatTests.Delta);
                Assert.IsTrue(negative > positive - FloatTests.Delta);
            });
        }

        [Test]
        public void Implements_IEquatable()
        {
            var v0 = new Pulse(default);

            Assert.IsTrue(_v0.Equals(v0));
            Assert.IsTrue(_v0.Equals(v0 as object));
            Assert.IsTrue(_v0 == v0);

            Iterate((f, v) =>
            {
                var w = new Pulse(f);

                Assert.IsTrue(v.Equals(w));
                Assert.IsTrue(v.Equals(w as object));
                Assert.IsTrue(v == w);

                Assert.IsFalse(v.Equals(_v0));
                Assert.IsFalse(v.Equals(_v0 as object));
                Assert.IsTrue(v != _v0);
            });
        }

        [Test]
        public void Have_Unique_HashCode()
        {
            var hashList = new List<int>();
            Iterate((f, v) => hashList.Add(v.GetHashCode()));
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
            var v1 = new Pulse(1);

            Assert.IsTrue(+v1 + -v1 == _v0);

            Assert.IsTrue(v1 + _v0 == v1);
            Assert.IsTrue(_v0 + v1 == v1);

            Assert.IsTrue(v1 - v1 == _v0);
            Assert.IsTrue(_v0 - v1 == -v1);
            Assert.IsTrue(v1 - _v0 == v1);

            Assert.IsTrue(v1 * v1 == v1);
            Assert.IsTrue(v1 * _v0 == _v0);
            Assert.IsTrue(_v0 * v1 == _v0);

            Assert.IsTrue(v1 / v1 == v1);
            Assert.IsTrue(_v0 / v1 == _v0);

            Iterate((f, v) =>
            {
                Assert.IsTrue(+v + -v == _v0);

                Assert.IsTrue(v + _v0 == v);
                Assert.IsTrue(_v0 + v == v);

                Assert.IsTrue(v - v == _v0);
                Assert.IsTrue(_v0 - v == -v);
                Assert.IsTrue(v - _v0 == v);

                Assert.IsTrue(v * v1 == v);
                Assert.IsTrue(v1 * v == v);
                Assert.IsTrue(v * _v0 == _v0);
                Assert.IsTrue(_v0 * v == _v0);

                Assert.IsTrue(v / v == v1);
                Assert.IsTrue(v / v1 == v);
                Assert.IsTrue(_v0 / v == _v0);
            });
        }

        [Test]
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public void Implements_IFormattable()
        {
            Iterate((f, v) =>
            {
                const string format = "F";
                var culture = CultureInfo.CurrentCulture;

                Assert.AreEqual(f.ToString(), v.ToString());
                Assert.AreEqual(f.ToString(culture), v.ToString(culture));
                Assert.AreEqual(f.ToString(format), v.ToString(format));
                Assert.AreEqual(f.ToString(format, culture), v.ToString(format, culture));
            });
        }
    }
}
