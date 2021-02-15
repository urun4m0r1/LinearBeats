namespace Utils.Extensions
{
    public static class ArrayExtensions
    {
        public static int[] Reciprocal(this int[] input)
        {
            var output = new int[input.Length];
            for (var i = 0; i < input.Length; ++i) output[i] = 1 / input[i];
            return output;
        }

        public static ulong[] Reciprocal(this ulong[] input)
        {
            var output = new ulong[input.Length];
            for (var i = 0; i < input.Length; ++i) output[i] = 1 / input[i];
            return output;
        }

        public static float[] Reciprocal(this float[] input)
        {
            var output = new float[input.Length];
            for (var i = 0; i < input.Length; ++i) output[i] = 1 / input[i];
            return output;
        }

        public static int Sigma(this int[] input, int start, int length)
        {
            int output = 0;
            for (var i = start; i < start + length; ++i) output = checked(output + input[i]);
            return output;
        }

        public static ulong Sigma(this ulong[] input, int start, int length)
        {
            ulong output = 0;
            for (var i = start; i < start + length; ++i) output = checked(output + input[i]);
            return output;
        }

        public static float Sigma(this float[] input, int start, int length)
        {
            float output = 0f;
            for (var i = start; i < start + length; ++i) output += input[i];
            return output;
        }
    }
}
