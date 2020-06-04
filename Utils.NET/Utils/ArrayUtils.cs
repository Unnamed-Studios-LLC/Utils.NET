using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.NET.Utils
{
    public static class ArrayUtils
    {

        /// <summary>
        /// Returns a random array element using Rand.Next
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T Random<T>(this T[] array)
        {
            if (array.Length == 0) return default;
            return array[Rand.Next(array.Length)];
        }
    }
}
