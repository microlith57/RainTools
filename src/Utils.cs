
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

        // from https://github.com/CommunalHelper/CommunalHelper/blob/c78ae86324cd380d0731cb4a30ec9f29b151ec4f/src/Utils/Util.cs#L38
        // used here under the MIT license
        internal static Color ColorArrayLerp(float lerp, params Color[] colors) {
            float m = Mod(lerp, colors.Length);
            int fromIndex = (int) Math.Floor(m);
            int toIndex = Mod(fromIndex + 1, colors.Length);
            float clampedLerp = m - fromIndex;

            return Color.Lerp(colors[fromIndex], colors[toIndex], clampedLerp);
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