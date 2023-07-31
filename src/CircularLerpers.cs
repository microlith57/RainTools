using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools {
    public abstract class CircularLerper<T, T_out> {
        public List<KeyValuePair<float, T>> Stops;
        public bool Any => Stops.Count > 0;

        public CircularLerper() {
            Stops = new();
        }

        public CircularLerper(IEnumerable<KeyValuePair<float, T>> stops) : this() {
            foreach (var kvp in stops)
                Add(kvp);
        }

        public void Add(KeyValuePair<float, T> kvp) {
            Stops.AddSorted(kvp, new KVPComparer<float, T>());
        }

        public virtual void Add(float angleRadians, T stop) {
            Add(new(Calc.WrapAngle(angleRadians), stop));
        }

        public abstract T_out Convert(T val);
        public abstract T_out Lerp(T a, T b, float fac);

        public T_out GetOrDefault(float angleRadians) {
            if (Stops.Count == 0) {
                return default(T_out);
            } else if (Stops.Count == 1) {
                return Convert(Stops.First().Value);
            } else {
                int next_index = Stops.BinarySearch(new(angleRadians, default), new KVPComparer<float, T>());
                if (next_index < 0)
                    next_index = ~next_index;
                if (next_index >= Stops.Count)
                    next_index = 0;

                int prev_index = next_index - 1;
                if (prev_index < 0)
                    prev_index = Stops.Count - 1;

                var a = Stops[prev_index];
                var b = Stops[next_index];

                float dist_a = Calc.AbsAngleDiff(a.Key, angleRadians);
                float dist_b = Calc.AbsAngleDiff(b.Key, angleRadians);
                float fac = Calc.Clamp((dist_a) / (dist_a + dist_b), 0f, 1f);

                return Lerp(a.Value, b.Value, fac);
            }
        }

        public T_out Get(float angleRadians) {
            if (!Any)
                throw new ArgumentOutOfRangeException();

            return GetOrDefault(angleRadians);
        }
    }

    public abstract class SimpleCircularLerper<T> : CircularLerper<T, T> {
        public override T Convert(T val) {
            return val;
        }
    }

    public class CircularFloatLerper : SimpleCircularLerper<float> {
        public override float Lerp(float a, float b, float fac) {
            return MathHelper.Lerp(a, b, fac);
        }
    }

    public class CircularColorLerper : SimpleCircularLerper<Color> {
        public override Color Lerp(Color a, Color b, float fac) {
            return Color.Lerp(a, b, fac);
        }
    }

    public class CircularColorgradeLerper : CircularLerper<string, CircularColorgradeLerper.Blend> {
        public struct Blend {
            public string a, b;
            public float fac;

            public Blend(string a, string b, float fac) {
                this.a = a;
                this.b = b;
                this.fac = fac;
            }

            public Blend(string colorgrade) : this(colorgrade, colorgrade, 0f) { }
        }

        public override Blend Convert(string val) {
            return new(val);
        }

        public override Blend Lerp(string a, string b, float fac) {
            return new(a, b, fac);
        }
    }

    public class CircularColorGradientLerper : SimpleCircularLerper<Color[]> {
        public int Length { get; private set; }

        public override void Add(float angleRadians, Color[] stop) {
            if (Stops.Count == 0)
                Length = stop.Length;

            if (stop.Length != Length)
                throw new ArgumentException("gradient stop is of incorrect length for this gradient");

            base.Add(angleRadians, stop);
        }

        public override Color[] Lerp(Color[] a, Color[] b, float fac) {
            var result = new Color[Length];
            for (int i = 0; i < Length; i++) {
                result[i] = Color.Lerp(a[i], b[i], fac);
            }
            return result;
        }
    }
}
