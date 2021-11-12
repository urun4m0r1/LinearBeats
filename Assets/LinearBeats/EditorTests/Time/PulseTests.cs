﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using LinearBeats.Script;
using NUnit.Framework;
using static LinearBeats.EditorTests.FloatTests;

namespace LinearBeats.EditorTests.Time
{
    [TestFixture]
    public class PulseTests
    {
        public static readonly Pulse V0 = new Pulse(default);

        private static void Iterate([NotNull] Action<int, Pulse> action)
        {
            for (var i = 0; i < Iteration; ++i)
            {
                var randomInt = RandomInt;
                action(randomInt, new Pulse(randomInt));
            }
        }

        [Test]
        public void Should_Cast_To_Int()
        {
            Assert.AreEqual(F0, (int) V0);

            Iterate((i, v) => Assert.AreEqual(i, (int) v));
        }

        [Test]
        public void Should_Cast_From_Int()
        {
            Assert.AreEqual(V0, (Pulse) F0);

            Iterate((i, v) => Assert.AreEqual(v, (Pulse) i));
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
        public void Implements_IFormattable()
        {
            Iterate((i, v) =>
            {
                const string format = "F";
                var culture = CultureInfo.CurrentCulture;

                Assert.AreEqual(i.ToString(), v.ToString());
                Assert.AreEqual(i.ToString(culture), v.ToString(culture));
                Assert.AreEqual(i.ToString(format), v.ToString(format));
                Assert.AreEqual(i.ToString(format, culture), v.ToString(format, culture));
            });
        }

        [Test]
        public void Implements_IEquatable()
        {
            var v0 = new Pulse(default);
            AssertEquatable(v0, V0);

            Iterate((i, v) =>
            {
                var w = new Pulse(i);
                AssertEquatable(v, w);

                if (v == V0) return;

                Assert.IsFalse(v.Equals(V0));
                Assert.IsFalse(v.Equals(V0 as object));
                Assert.IsTrue(v != V0);
            });

            static void AssertEquatable(Pulse v, Pulse w)
            {
                Assert.IsTrue(v.Equals(w));
                Assert.IsTrue(v.Equals(w as object));
                Assert.IsTrue(v == w);
            }
        }

        [Test]
        public void Implements_IComparable()
        {
            var v0 = new Pulse(default);
            AssertEquatable(v0, V0);

            Iterate((i, v) =>
            {
                var vx = new Pulse(i); // [-5_000 ~ 5_000]
                AssertEquatable(v, vx);

                var vn = new Pulse(i - 5_000); // [-10_000, 0]
                var vp = new Pulse(i + 5_000); // [0, 10_000]
                AssertRightIsBigger(vn, vp);
                AssertRightIsBigger(v, vp);
                AssertRightIsBigger(vn, v);
            });

            static void AssertEquatable(Pulse left, Pulse right)
            {
                Assert.IsTrue(left.CompareTo(right) == 0);

                Assert.IsTrue(left.CompareTo(right) >= 0);
                Assert.IsTrue(left.CompareTo(right) <= 0);

                Assert.IsTrue((left as IComparable).CompareTo(right) >= 0);
                Assert.IsTrue((left as IComparable).CompareTo(right) <= 0);

                Assert.IsTrue(left >= right);
                Assert.IsTrue(left <= right);
            }

            static void AssertRightIsBigger(Pulse l, Pulse r)
            {
                Assert.IsTrue(r.CompareTo(l) >= 0);
                Assert.IsTrue(r.CompareTo(l - 1) > 0);
                Assert.IsTrue((r as IComparable).CompareTo(l) >= 0);
                Assert.IsTrue((r as IComparable).CompareTo(l - 1) > 0);
                Assert.IsTrue(r >= l);
                Assert.IsTrue(r > l - 1);

                Assert.IsTrue(l.CompareTo(r) <= 0);
                Assert.IsTrue(l.CompareTo(r + 1) < 0);
                Assert.IsTrue((l as IComparable).CompareTo(r) <= 0);
                Assert.IsTrue((l as IComparable).CompareTo(r + 1) < 0);
                Assert.IsTrue(l <= r);
                Assert.IsTrue(l < r + 1);
            }
        }

        [Test]
        public void Calculable()
        {
            var v1 = new Pulse(1);
            AssertCalculation(V0);
            AssertCalculation(v1);

            Assert.IsTrue(v1 * v1 == v1); // 양수 * 양수
            Assert.IsTrue(v1 * -v1 == -v1); // 양수 * 음수
            Assert.IsTrue(-v1 * v1 == -v1); // 음수 * 양수
            Assert.IsTrue(-v1 * -v1 == v1); // 음수 * 음수

            Iterate((_, v) => AssertCalculation(v));

            static void AssertCalculation(Pulse v)
            {
                var v1 = new Pulse(1);

                Assert.IsTrue(+v == V0 + v); // 양수
                Assert.IsTrue(-v == V0 - v); // 음수

                Assert.IsTrue(v + -v == V0); // 역원
                Assert.IsTrue(v + V0 == v); // 항등원
                Assert.IsTrue(v + v1 == v1 + v); // 교환법칙

                Assert.IsTrue(v - v == V0); // 역원
                Assert.IsTrue(v - V0 == v); // 항등원
                if (v != v1) Assert.IsTrue(v - v1 != v1 - v); // 교환법칙

                Assert.IsTrue(v * V0 == V0); // 영원
                Assert.IsTrue(v * v1 == v); // 항등원
                Assert.IsTrue(v * v1 == v1 * v); // 교환법칙

                if (v != V0) Assert.IsTrue(V0 / v == V0); // 영원
                Assert.IsTrue(v / v1 == v); // 항등원
            }
        }
    }
}
