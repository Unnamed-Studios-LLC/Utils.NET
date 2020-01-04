using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.NET.Collections
{
    public struct Range
    {
        /// <summary>
        /// The minimum value of this range
        /// </summary>
        public float min;

        /// <summary>
        /// The maximum value of this range
        /// </summary>
        public float max;

        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public class RangeMap<T>
    {
        private class RangePair
        {
            public Range range;
            public T value;

            public RangePair(Range range, T value)
            {
                this.range = range;
                this.value = value;
            }

            public bool InRange(float value)
            {
                return value >= range.min && value < range.max;
            }
        }

        private List<RangePair> rangePairs = new List<RangePair>();

        public T this[float i]
        {
            get => Get(i);
        }

        public T Get(float index)
        {
            foreach (var pair in rangePairs)
            {
                if (pair.InRange(index))
                    return pair.value;
            }
            return default;
        }

        public void Add(Range range, T value)
        {
            rangePairs.Add(new RangePair(range, value));
        }
    }
}
