using Monocle;

namespace Celeste.Mod.RainTools {
    public static class Commands {
        [Command("cycle_set", "set a cycle progression to the given value")]
        public static void CycleSet(string cycle, float value = 0) {
            Cycles.SetProgression(cycle, value);
        }

        [Command("cycle_get", "get a cycle progression and angle")]
        public static void CycleGet(string cycle) {
            Engine.Commands.Log($"progression: {Cycles.GetProgression(cycle)}");
            Engine.Commands.Log($"angle: {Cycles.GetAngle(cycle)}");
        }

        [Command("cycle_debug", "show a given cycle's debugging visualisation")]
        public static void CycleSet(string cycle) {
            if (cycle == "")
                Cycles.Debugging = null;
            else
                Cycles.Debugging = cycle;
        }
    }
}
