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
        public static T Test<T>(this T t)
        {
            GD.Print(t);
            return t;
        }
    }
}
