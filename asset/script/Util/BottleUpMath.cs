using Godot;
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

        public static Vector2 RadToVec(float radians) => new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        public static Vector2 DegToVec(float degrees) => RadToVec(degrees * DEGTORAD);

        public static float Lerp(float a, float b, float by) => a + (b - a) * by;
        public static Vector2 LerpVec(Vector2 a, Vector2 b, float by) => a + (b - a) * by;

        public static Vector2 Uniform(float f) => new Vector2(f, f);
    }
}
