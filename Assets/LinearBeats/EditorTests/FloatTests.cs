using System;

namespace LinearBeats.EditorTests
{
    public static class FloatTests
    {
        public static float RandomFloat => (float) Random.NextDouble() - 0.5f; // [-0.5 ~ 0.5]
        private static readonly Random Random = new Random();

        public const int Iteration = 10000;
        public const int Digits = 6;
        public const float Delta = 1e-6f;
        public const float F0 = default;
    }
}
