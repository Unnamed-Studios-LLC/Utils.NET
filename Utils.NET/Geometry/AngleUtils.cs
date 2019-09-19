﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.NET.Geometry
{
    public static class AngleUtils
    {
        public const float PI = 3.1415926535897f;

        public const float PI_2 = 6.28318530718f;

        public const float Rad2Deg = 57.2957795131f;

        public const float Deg2Rad = 0.01745329251f;

        /// <summary>
        /// Normalizes given radians between 0 - 2PI
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static float NormalizeRadians(float radians)
        {
            if (radians >= 0)
            {
                return radians % PI_2;
            }
            else
            {
                return PI_2 + (radians % PI_2);
            }
        }

        /// <summary>
        /// Normalizes given degrees between 0 - 360
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float NormalizeDegrees(float degrees)
        {
            if (degrees >= 0)
            {
                return degrees % 360;
            }
            else
            {
                return 360 + (degrees % 360);
            }
        }
    }
}