using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools {
    public abstract class CircularInterpolator<T, T_out> {
        public List<float> StopAngles;
        public List<T> StopValues;
        public List<Ease.Easer> StopEasers;

        public int Count => StopAngles.Count;
        public bool Any => StopAngles.Count > 0;

        public CircularInterpolator() {
            StopAngles = new();
            StopValues = new();
            StopEasers = new();
        }

        public virtual void Add(float angleRadians, T stopValue, Ease.Easer ease) {
            if (Count == 0 || angleRadians > StopAngles.Last()) {
                StopAngles.Add(angleRadians);
                StopValues.Add(stopValue);
                StopEasers.Add(ease);
                return;
            }
            if (angleRadians < StopAngles.First()) {
                StopAngles.Insert(0, angleRadians);
                StopValues.Insert(0, stopValue);
                StopEasers.Insert(0, ease);
                return;
            }

            int index = StopAngles.BinarySearch(angleRadians);
            if (index < 0)
                index = ~index;

            StopAngles.Insert(index, angleRadians);
            StopValues.Insert(index, stopValue);
            StopEasers.Insert(index, ease);
        }

        public void Add(float angleRadians, T stop, string easerName = "") {
            var ease = easerName == "" ? Ease.Linear : FrostHelper.API.API.GetEaser(easerName, Ease.Linear);

            Add(angleRadians, stop, ease);
        }

        public abstract T_out Convert(T val);
        public abstract T_out Lerp(T a, T b, float fac);

        public T_out GetOrDefault(float angleRadians) {
            if (Count == 0) {
                return default(T_out);
            } else if (Count == 1) {
                return Convert(StopValues.First());
            } else {
                int next_index = StopAngles.BinarySearch(angleRadians);
                if (next_index < 0)
                    next_index = ~next_index;
                if (next_index >= Count)
                    next_index = 0;

                int prev_index = next_index - 1;
                if (prev_index < 0)
                    prev_index = Count - 1;

                var ease = StopEasers[prev_index];

                float dist_a = Calc.AbsAngleDiff(StopAngles[prev_index], angleRadians);
                float dist_b = Calc.AbsAngleDiff(StopAngles[next_index], angleRadians);
                float fac = ease(dist_a / (dist_a + dist_b));

                return Lerp(StopValues[prev_index], StopValues[next_index], fac);
            }
        }

        public T_out Get(float angleRadians) {
            if (!Any)
                throw new ArgumentOutOfRangeException();

            return GetOrDefault(angleRadians);
        }
    }

    public abstract class SimpleCircularInterpolator<T> : CircularInterpolator<T, T> {
        public override T Convert(T val) {
            return val;
        }
    }

    public class CircularFloatInterpolator : SimpleCircularInterpolator<float> {
        public override float Lerp(float a, float b, float fac) {
            return MathHelper.Lerp(a, b, fac);
        }
    }

    public class CircularColorInterpolator : SimpleCircularInterpolator<Color> {
        public override Color Lerp(Color a, Color b, float fac) {
            return Color.Lerp(a, b, fac);
        }
    }

    public class CircularColorgradeInterpolator : CircularInterpolator<string, CircularColorgradeInterpolator.Blend> {
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

    public class CircularColorGradientInterpolator : SimpleCircularInterpolator<Color[]> {
        public int Length { get; private set; }

        public override void Add(float angleRadians, Color[] stop, Ease.Easer easer) {
            if (Count == 0)
                Length = stop.Length;

            if (stop.Length != Length)
                throw new ArgumentException("gradient stop is of incorrect length for this gradient");

            base.Add(angleRadians, stop, easer);
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
