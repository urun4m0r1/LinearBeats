using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Extensions
{
    public static class CSharpExtensions
    {
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list.IsNullOrEmptyRefType() || list.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array.IsNullOrEmptyRefType() || array.Length == 0;
        }

        public static bool IsNullOrEmptyRefType<T>(this T reference)
        {
            return reference == null || reference.ToString().Equals("null");
        }

        public static Type GetType<T>(this T obj)
        {
            return typeof(T);
        }

        public static bool IsIn<T>(this T obj, params T[] collection)
        {
            if (collection.IsNullOrEmpty())
            {
                throw new ArgumentOutOfRangeException(nameof(collection));
            }

            return collection.Contains(obj);
        }
    }
}
