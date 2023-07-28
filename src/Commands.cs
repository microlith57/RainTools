using Monocle;

namespace Celeste.Mod.RainTools {
    public static class Commands {
        [Command("time_set", "set the time")]
        public static void TimeSet(float time = 0) {
            RainToolsModule.Session.Time = time;
        }
    }
}
