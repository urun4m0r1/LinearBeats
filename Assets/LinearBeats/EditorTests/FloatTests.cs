using UnityEngine;
using Random = System.Random;

namespace LinearBeats.EditorTests
{
    public static class FloatTests
    {
        public static float RandomFloat => ((float) Random.NextDouble() - 0.5f) * 10_000; // [-5_000 ~ 5_000]
        public static int RandomInt => Mathf.RoundToInt(((float) Random.NextDouble() - 0.5f) * 10_000); // [-5_000 ~ 5_000]
        private static readonly Random Random = new Random();

        public const int Iteration = 10_000;
        public const int Digits = 2;
        public const float Delta = 1e-2f;
        public const float F0 = default;
    }
}
