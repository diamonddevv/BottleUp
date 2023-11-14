﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottleUp.asset.script.Util
{
    internal static class BottleUpMath
    {
        public const float DEGTORAD = Mathf.Pi / 180;
        public const float RADTODEG = 180 / Mathf.Pi;

        public const float SQUARED_PIXELS_TO_ARB_METERS = 2e-5f;

        public static float SqrPxToArbMeters(float sqrpx) => sqrpx * SQUARED_PIXELS_TO_ARB_METERS;
        public static float ArbMetersToSqrPx(float arbMeters) => arbMeters / SQUARED_PIXELS_TO_ARB_METERS;


        public static Vector2 RadToVec(float radians) => new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        public static Vector2 DegToVec(float degrees) => RadToVec(degrees * DEGTORAD);

        public static float Lerp(float a, float b, float by) => a + (b - a) * by;
        public static Vector2 LerpVec(Vector2 a, Vector2 b, float by) => a + (b - a) * by;

        public static Vector2 Uniform(float f) => Vector2.One * f;

        public static Vector2 Multiply(this Vector2 vec, float xCoef, float yCoef) => new Vector2(vec.X * xCoef, vec.Y * yCoef);
        public static Vector2 Multiply(this Vector2 vec, float coef) => new Vector2(vec.X * coef, vec.Y * coef);

        public static Vector2I RoundInts(this Vector2 vec) => new Vector2I(Mathf.RoundToInt(vec.X), Mathf.RoundToInt(vec.Y));
    }
}
