using System;
using System.Threading;
using Utils.NET.Geometry;

namespace Utils.NET.Utils
{
    public static class Rand
    {
        private static ThreadLocal<Random> randoms = new ThreadLocal<Random>();

        private static Random random
        {
            get
            {
                if (randoms.IsValueCreated) return randoms.Value;
                Random rnd = new Random((int)DateTime.Now.Ticks);
                randoms.Value = rnd;
                return rnd;
            }
        }

        #region Integer

        /// <summary>
        /// Returns a random number within a range
        /// </summary>
        /// <returns>The next.</returns>
        /// <param name="min">The lowest number (inclusive)</param>
        /// <param name="max">The maximum output (exclusive)</param>
        public static int Range(int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// Returns a random number with a maximum given value
        /// </summary>
        /// <returns>The next.</returns>
        /// <param name="max">The maximum output (exclusive)</param>
        public static int Next(int max)
        {
            return Range(0, max);
        }

        /// <summary>
        /// Returns a random int32 value
        /// </summary>
        /// <returns>The value.</returns>
        public static int IntValue()
        {
            return Range(int.MinValue, int.MaxValue);
        }

        #endregion

        #region Float

        /// <summary>
        /// Returns a random float value at or between 0 and 2 PI
        /// </summary>
        /// <returns>The value.</returns>
        public static float AngleValue()
        {
            return (float)random.NextDouble() * AngleUtils.PI_2;
        }

        /// <summary>
        /// Returns a random float value at or between 0 and 1
        /// </summary>
        /// <returns>The value.</returns>
        public static float FloatValue()
        {
            return (float)random.NextDouble();
        }

        /// <summary>
        /// Returns a random float value between two given numbers
        /// </summary>
        /// <returns>The range.</returns>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Max.</param>
        public static float Range(float min, float max)
        {
            return min + (max - min) * FloatValue();
        }

        #endregion

        #region Misc

        public static byte[] Bytes(int length)
        {
            var bytes = new byte[length];
            random.NextBytes(bytes);
            return bytes;
        }

        public static string Base64(int byteLength)
        {
            var bytes = Bytes(byteLength);
            return Convert.ToBase64String(bytes);
        }

        #endregion
    }
}
