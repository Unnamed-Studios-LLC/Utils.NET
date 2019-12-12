using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.NET.Utils
{
    public static class LinqEx
    {
        public static T Min<T>(this IEnumerable<T> collection, Func<T, T, T> comparison)
        {
            T result = default(T);
            foreach (var value in collection)
            {
                if (result == null)
                {
                    result = value;
                    continue;
                }

                result = comparison(result, value);
            }
            return result;
        }

        public static T Closest<T>(this IEnumerable<T> collection, Func<T, float> distance)
        {
            T result = default(T);
            float closestDistance = float.MaxValue;
            foreach (var value in collection)
            {
                var dis = distance(value);
                if (dis < closestDistance)
                {
                    result = value;
                    closestDistance = dis;
                }
            }
            return result;
        }
    }
}
