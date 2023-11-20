using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottleUp.asset.script.Util
{
    public static class BottleUpHelper
    {
        public const string GITHUB_REPOSITORY = "https://github.com/diamonddevv/BottleUp";
        public const string ISSUES_GITHUB_REPOSITORY = "https://github.com/diamonddevv/BottleUp/issues";


        public static T Test<T>(this T t)
        {
            GD.Print(t);
            return t;
        }

        public static string FormatTime(double secs)
        {
            double mins = Math.Floor(secs / 60);
            double secsAfterMins = Math.Floor(secs % 60);
            return $"{mins}:{secsAfterMins}";
        }

        public static bool IsBitSet(this byte b, int index) => (b & (1 << index)) != 0;

        public struct Rating
        {
            public float Percentage;
            public int StarCount;

            public float GetStarCount() => Percentage / StarCount;
        }
    }
}
