using System;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    public static class Cycles {
        public static float Time => GetProgression("time");

        public static bool Has(string tag) {
            return RainToolsModule.Session.CycleProgressions.ContainsKey(tag);
        }

        public static float GetProgression(string tag) {
            if (RainToolsModule.Session.CycleProgressions.TryGetValue(tag, out float progression)) {
                return progression;
            }
            return 0f;
        }

        public static float GetAngle(string tag, float offset = -(float) Math.PI / 2f, float multipler = 1f) {
            return (float) (GetProgression(tag) * multipler * 2f * Math.PI) + offset;
        }

        public static void SetProgression(string tag, float value) {
            RainToolsModule.Session.CycleProgressions[tag] = value;
        }

        public static void AddProgression(string tag, float value) {
            if (RainToolsModule.Session.CycleProgressions.TryGetValue(tag, out float prev)) {
                RainToolsModule.Session.CycleProgressions[tag] = prev + value;
            } else {
                RainToolsModule.Session.CycleProgressions[tag] = value;
            }
        }
    }
}
