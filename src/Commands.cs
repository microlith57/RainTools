using Monocle;

namespace Celeste.Mod.RainTools {
    public static class Commands {
        [Command("cycle", "set a cycle progression to the given value")]
        public static void TimeSet(string cycle, float value = 0) {
            Cycles.SetProgression(cycle, value);
        }
    }
}
