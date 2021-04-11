﻿using System;

namespace LinearBeats.EditorTests.Time
{
    public static class FloatTests
    {
        public static float RandomFloat => (float) Random.NextDouble() - 0.5f;
        private static readonly Random Random = new Random();

        public const int Iteration = 10000;
        public const int Digits = 6;
        public const float Delta = 1e-6f;
        public const float F0 = default;
    }
}