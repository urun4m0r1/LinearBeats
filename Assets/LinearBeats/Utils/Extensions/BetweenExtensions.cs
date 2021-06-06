using System;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace LinearBeats.Utils.Extensions
{
    /// <summary>
    /// An extension class for the between operation
    /// name pattern IsBetweenXX where X = ( I -> Inclusive, E -> Exclusive )
    /// <a href="https://stackoverflow.com/a/13470099/37055"></a>
    /// </summary>
    public static class BetweenExtensions
    {
        /// <summary>
        /// Between check <![CDATA[min <= value <= max]]>
        /// </summary>
        public static bool IsBetweenII<T>(this T value, T min, T max) where T : struct, IComparable<T> =>
            min.CompareTo(value) <= 0 && value.CompareTo(max) <= 0;

        /// <summary>
        /// Between check <![CDATA[min < value <= max]]>
        /// </summary>
        public static bool IsBetweenEI<T>(this T value, T min, T max) where T : struct, IComparable<T> =>
            min.CompareTo(value) < 0 && value.CompareTo(max) <= 0;

        /// <summary>
        /// between check <![CDATA[min <= value < max]]>
        /// </summary>
        public static bool IsBetweenIE<T>(this T value, T min, T max) where T : struct, IComparable<T> =>
            min.CompareTo(value) <= 0 && value.CompareTo(max) < 0;

        /// <summary>
        /// between check <![CDATA[min < value < max]]>
        /// </summary>
        public static bool IsBetweenEE<T>(this T value, T min, T max) where T : struct, IComparable<T> =>
            min.CompareTo(value) < 0 && value.CompareTo(max) < 0;
    }
}
