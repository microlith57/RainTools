
using Microsoft.Xna.Framework;
using System;

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

    }
}