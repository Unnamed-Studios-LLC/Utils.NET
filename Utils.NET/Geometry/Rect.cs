using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.NET.Geometry
{
    public struct Rect
    {
        public float x;

        public float y;

        public float width;

        public float height;


        public Vec2 BottomLeft => new Vec2(x, y);
        public Vec2 TopRight => new Vec2(x + width, y + height);


        public Rect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Rect(Vec2 position, Vec2 size)
        {
            x = position.x;
            y = position.y;
            width = size.x;
            height = size.y;
        }
    }
}
