
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools {
    internal static class Utils {

        internal static int GCF(int a, int b) {
            while (b != 0) {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        internal static int LCM(int a, int b) {
            return a / GCF(a, b) * b;
        }

        internal static Color ColorArrayLerp(float lerp, params Color[] colors) {
            int fromIndex = (int) Math.Floor(lerp);
            int toIndex = (int) Math.Ceiling(lerp);
            float fac = lerp - fromIndex;

            return Color.Lerp(colors[fromIndex], colors[toIndex], fac);
        }

        internal static float Mod(float x, float m) {
            return ((x % m) + m) % m;
        }

        internal static int Mod(int x, int m) {
            return ((x % m) + m) % m;
        }

        internal static Vector2 Proj(this Vector2 vec, Vector2 onto) {
            return Vector2.Dot(vec, onto) / Vector2.Dot(onto, onto) * onto;
        }

        private static Dictionary<float, Color> BlackbodyColors = new() {
            {500, new(0, 0, 0)},
            {550, new(53, 34, 1)},
            {630, new(84, 40, 3)},
            {680, new(104, 17, 0)},
            {740, new(134, 22, 0)},
            {780, new(160, 0, 0)},
            {810, new(193, 27, 27)},
            {850, new(212, 65, 21)},
            {900, new(233, 88, 44)},
            {950, new(233, 126, 28)},
            {1000, new(255, 170, 15)},
            {1100, new(251, 192, 52)},
            {1200, new(255, 207, 97)},
            {1300, new(255, 230, 173)},
            {2000, new(255, 255, 255)},
        };
        private static float MinBlackbodyTemp = BlackbodyColors.Keys.Min();
        private static float MaxBlackbodyTemp = BlackbodyColors.Keys.Max();

        public static Color Blackbody(float celsius) {
            if (celsius <= MinBlackbodyTemp)
                return BlackbodyColors[MinBlackbodyTemp];
            if (celsius >= MaxBlackbodyTemp)
                return BlackbodyColors[MaxBlackbodyTemp];

            var below = BlackbodyColors.Keys.Where(t => t <= celsius).Max();
            var above = BlackbodyColors.Keys.Where(t => t > celsius).Min();
            var fac = Calc.ClampedMap(celsius, below, above, 0, 1);

            return Color.Lerp(BlackbodyColors[below], BlackbodyColors[above], fac);
        }

    }
}