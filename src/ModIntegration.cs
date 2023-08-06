using System;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Linq;
using Celeste.Mod.Helpers;

namespace Celeste.Mod.RainTools.ModIntegration {
    internal static class CommunalHelper {
        public static bool Loaded = false;

        private static Type t_Cloudscape;
        private static MethodInfo m_Cloudscape_ConfigureColors;

        public static void Load() {
            t_Cloudscape = FakeAssembly.GetFakeEntryAssembly().GetType("Celeste.Mod.CommunalHelper.Backdrops.Cloudscape");

            if (t_Cloudscape == null)
                return;

            m_Cloudscape_ConfigureColors = t_Cloudscape.GetMethod("ConfigureColors");

            Loaded = true;
        }

        public static void ConfigureCloudscapes(Level level, string tag, Color bg, Color[] a, Color[] b, float fac) {
            if (!Loaded)
                throw new Exception();

            var bgs = level.Background.GetEach<Backdrop>(tag).Where(bg => bg.GetType() == t_Cloudscape);
            var fgs = level.Foreground.GetEach<Backdrop>(tag).Where(bg => bg.GetType() == t_Cloudscape);

            foreach (var cloudscape in bgs.Union(fgs)) {
                m_Cloudscape_ConfigureColors.Invoke(cloudscape, new object[] { bg, a, b, fac });
            }
        }
    }
}
