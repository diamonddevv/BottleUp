using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottleUp.asset.script.Util
{
    internal static class BottleUpHelper
    {
        public const string GITHUB_REPOSITORY = "https://github.com/diamonddevv/BottleUp";
        public const string ISSUES_GITHUB_REPOSITORY = "https://github.com/diamonddevv/BottleUp/issues";

        public static T Test<T>(this T t)
        {
            GD.Print(t);
            return t;
        }


        public static bool IsBitSet(this byte b, int index) => (b & (1 << index)) != 0;
    }
}
