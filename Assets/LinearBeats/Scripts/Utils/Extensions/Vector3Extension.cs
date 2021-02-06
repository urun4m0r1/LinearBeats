using UnityEngine;

namespace Utils.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ClampMagnitude(this Vector3 v, float min, float max)
        {
            if (max < min)
            {
                throw new System.ArgumentException();
            }

            double sm = v.sqrMagnitude;
            if (sm > (double)max * (double)max) return v.normalized * max;
            else if (sm < (double)min * (double)min) return v.normalized * min;
            return v;
        }
    }
}
