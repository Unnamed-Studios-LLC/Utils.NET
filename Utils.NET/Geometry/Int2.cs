using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.NET.Geometry
{
    public struct Int2
    {
        public int x;

        public int y;

        public float Length => (float)Math.Sqrt(x * x + y * y);
        public float SqrLength => x * x + y * y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Int2 Add(Int2 vec) => new Int2(x + vec.x, y + vec.y);
        public Int2 Subtract(Int2 vec) => new Int2(x - vec.x, y - vec.y);
        public Int2 Multiply(Int2 vec) => new Int2(x * vec.x, y * vec.y);
        public Int2 Divide(Int2 vec) => new Int2(x / vec.x, y / vec.y);

        public Int2 Add(int value) => new Int2(x + value, y + value);
        public Int2 Subtract(int value) => new Int2(x - value, y - value);
        public Int2 Multiply(int value) => new Int2(x * value, y * value);
        public Int2 Divide(int value) => new Int2(x / value, y / value);

        public float AngleTo(Int2 vec) => (float)Math.Atan2(vec.y - y, vec.x - x);
        public float DistanceTo(Int2 vec) => vec.Subtract(this).Length;

        public static Int2 operator +(Int2 a, Int2 b) => a.Add(b);
        public static Int2 operator -(Int2 a, Int2 b) => a.Subtract(b);
        public static Int2 operator *(Int2 a, Int2 b) => a.Multiply(b);
        public static Int2 operator /(Int2 a, Int2 b) => a.Divide(b);

        public static Int2 operator +(Int2 a, int b) => a.Add(b);
        public static Int2 operator -(Int2 a, int b) => a.Subtract(b);
        public static Int2 operator *(Int2 a, int b) => a.Multiply(b);
        public static Int2 operator /(Int2 a, int b) => a.Divide(b);

        public static implicit operator Int2(Vec2 vec) => new Int2((int)vec.x, (int)vec.y);
        public static implicit operator Int2(int value) => new Int2(value, value);
    }
}
