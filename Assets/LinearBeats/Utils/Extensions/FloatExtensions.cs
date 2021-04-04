using UnityEngine;

namespace Utils.Extensions
{
    public static class FloatExtensions
    {
        public static int RoundToInt(this float value)
        {
            return checked((int)Mathf.Round(value));
        }
    }
}
