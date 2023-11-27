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

        public static T Test<T, V>(this T t, Func<T, V> what)
        {
            GD.Print(what(t));
            return t;
        }

        public static string FormatTime(double secs)
        {
            double mins = Math.Floor(secs / 60);
            double secsAfterMins = Math.Floor(secs % 60);

            string s = "";
            if (secsAfterMins < 10) s = $"0{secsAfterMins}";
            else s = $"{secsAfterMins}";

            return $"{mins}:{s}";
        }

        public static double GetFramerate() => Engine.GetFramesPerSecond();

        public static bool IsBitSet(this byte b, int index) => (b & (1 << index)) != 0;

        public struct Rating
        {
            public double Percentage;
            public int StarCount;

            public double GetStarCount() => Percentage / StarCount;
        }
    }
}
