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

    }
}
