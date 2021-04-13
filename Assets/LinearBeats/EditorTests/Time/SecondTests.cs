using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Time;
using NUnit.Framework;
using static LinearBeats.EditorTests.FloatTests;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public class SecondTests
    {
        private static readonly Second V0 = new Second(default);

        private static void Iterate([NotNull] Action<float, Second> action)
        {
            for (var i = 0; i < Iteration; ++i)
            {
                var randomFloat = RandomFloat;
                action(randomFloat, new Second(randomFloat));
            }
        }

        [Test]
        public void Implements_IFloat()
        {
            Assert.AreEqual(F0, V0.ToFloat());

            Iterate((f, v) => Assert.AreEqual(f, v.ToFloat()));
        }

        [Test]
        public void Should_Cast_To_Float()
        {
            Assert.AreEqual(F0, (float) V0);

            Iterate((f, v) => Assert.AreEqual(f, (float) v));
        }

        [Test]
        public void Should_Cast_From_Float()
        {
            Assert.AreEqual(V0, (Second) F0);

            Iterate((f, v) => Assert.AreEqual(v, (Second) f));
        }

        [Test]
        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public void Should_Cast_From_String()
        {
            Assert.AreEqual(V0, (Second) F0.ToString());

            Iterate((rf, _) =>
            {
                var f = (float) Math.Round(rf, Digits);
                var v = new Second(f);

                Assert.AreEqual(v, (Second) f.ToString());
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

        [Test]
        public void Implements_IEquatable()
        {
            var v0 = new Second(default);
            AssertEquatable(v0, V0);

            Iterate((f, v) =>
            {
                var w = new Second(f);
                AssertEquatable(v, w);

                if (v == V0) return;

                Assert.IsFalse(v.Equals(V0));
                Assert.IsFalse(v.Equals(V0 as object));
                Assert.IsTrue(v != V0);
            });

            static void AssertEquatable(Second v, Second w)
            {
                Assert.IsTrue(v.Equals(w));
                Assert.IsTrue(v.Equals(w as object));
                Assert.IsTrue(v == w);
            }
        }

        [Test]
        public void Implements_IComparable()
        {
            var v0 = new Second(default);
            AssertEquatable(v0, V0);

            Iterate((f, v) =>
            {
                var vx = new Second(f); // [-0.5 ~ 0.5]
                AssertEquatable(v, vx);

                var vn = new Second(f - 0.5f); // [-1, 0]
                var vp = new Second(f + 0.5f); // [0, 1]
                AssertRightIsBigger(vn, vp);
                AssertRightIsBigger(v, vp);
                AssertRightIsBigger(vn, v);
            });

            static void AssertEquatable(Second left, Second right)
            {
                Assert.IsTrue(left.CompareTo(right) == 0);

                Assert.IsTrue(left.CompareTo(right) >= 0);
                Assert.IsTrue(left.CompareTo(right) <= 0);

                Assert.IsTrue((left as IComparable).CompareTo(right) >= 0);
                Assert.IsTrue((left as IComparable).CompareTo(right) <= 0);

                Assert.IsTrue(left >= right);
                Assert.IsTrue(left <= right);
            }

            static void AssertRightIsBigger(Second l, Second r)
            {
                Assert.IsTrue(r.CompareTo(l) >= 0);
                Assert.IsTrue(r.CompareTo(l - Delta) > 0);
                Assert.IsTrue((r as IComparable).CompareTo(l) >= 0);
                Assert.IsTrue((r as IComparable).CompareTo(l - Delta) > 0);
                Assert.IsTrue(r >= l);
                Assert.IsTrue(r > l - Delta);

                Assert.IsTrue(l.CompareTo(r) <= 0);
                Assert.IsTrue(l.CompareTo(r + Delta) < 0);
                Assert.IsTrue((l as IComparable).CompareTo(r) <= 0);
                Assert.IsTrue((l as IComparable).CompareTo(r + Delta) < 0);
                Assert.IsTrue(l <= r);
                Assert.IsTrue(l < r + Delta);
            }
        }

        [Test]
        public void Calculable()
        {
            var v1 = new Second(1f);
            AssertCalculation(V0);
            AssertCalculation(v1);

            Assert.IsTrue(v1 * v1 == v1); // 양수 * 양수
            Assert.IsTrue(v1 * -v1 == -v1); // 양수 * 음수
            Assert.IsTrue(-v1 * v1 == -v1); // 음수 * 양수
            Assert.IsTrue(-v1 * -v1 == v1); // 음수 * 음수

            Iterate((_, v) => AssertCalculation(v));

            static void AssertCalculation(Second v)
            {
                var v1 = new Second(1f);

                Assert.IsTrue(+v == V0 + v); // 양수
                Assert.IsTrue(-v == V0 - v); // 음수

                Assert.IsTrue(v + -v == V0); // 역원
                Assert.IsTrue(v + V0 == v); // 항등원
                Assert.IsTrue(v + v1 == v1 + v); // 교환법칙

                Assert.IsTrue(v - v == V0); // 역원
                Assert.IsTrue(v - V0 == v); // 항등원
                if (v != v1) Assert.IsTrue(v - v1 != v1 - v); // 교환법칙

                Assert.IsTrue(v * V0 == V0); // 영원
                if (v != V0) Assert.IsTrue(Similar(v * (v1 / v), v1)); // 역원
                Assert.IsTrue(v * v1 == v); // 항등원
                Assert.IsTrue(v * v1 == v1 * v); // 교환법칙

                if (v != V0) Assert.IsTrue(V0 / v == V0); // 영원
                if (v != V0) Assert.IsTrue(Similar(v / (v / v1), v1)); // 역원
                Assert.IsTrue(v / v1 == v); // 항등원
                if (v != V0 && v != v1) Assert.IsTrue(v / v1 != v1 / v); // 교환법칙
            }

            static bool Similar(Second left, Second right) => Math.Abs(left - right) < Delta;
        }
    }
}
