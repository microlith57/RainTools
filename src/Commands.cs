using Celeste.Mod.RainTools.Subregion;
using Monocle;

namespace Celeste.Mod.RainTools {
    public static class Commands {

        [Command("cycleset", "set a cycle progression to the given value")]
        public static void CycleSet(string cycle, float value = 0) {
            Cycles.SetProgression(cycle, value);
        }

        [Command("cycleget", "get a cycle progression and angle")]
        public static void CycleGet(string cycle) {
            Engine.Commands.Log($"progression: {Cycles.GetProgression(cycle)}");
            Engine.Commands.Log($"angle: {Cycles.GetAngle(cycle)}");
        }

        [Command("cycledbg", "show a given cycle's debugging visualisation")]
        public static void CycleSet(string cycle) {
            if (cycle == "")
                Cycles.Debugging = null;
            else
                Cycles.Debugging = cycle;
        }

        [Command("subregiontext", "displays a subregion text element")]
        public static void SubregionText(string cycleTag, string dialogKey, float duration = 10f, float easeTime = .25f, float delay = 0f) {
            Engine.Scene.Add(new TextElement(cycleTag, dialogKey, duration, easeTime, delay));
        }

        [Command("currentSubregion", "writes the current subregion ID")]
        public static void CurrentSubregion() {
            Engine.Commands.Log($"current subregion ID: {RainToolsModule.Session.CurrentSubregionID}");
        }
    }
}
