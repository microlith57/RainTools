using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.RainTools {
    public class RainToolsModule : EverestModule {
        public static RainToolsModule Instance { get; private set; }

        public override Type SettingsType => typeof(RainToolsModuleSettings);
        public static RainToolsModuleSettings Settings => (RainToolsModuleSettings) Instance._Settings;

        public override Type SessionType => typeof(RainToolsModuleSession);
        public static RainToolsModuleSession Session => (RainToolsModuleSession) Instance._Session;

        public RainToolsModule() {
            Instance = this;
#if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(RainToolsModule), LogLevel.Verbose);
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(RainToolsModule), LogLevel.Info);
#endif
        }

        public override void Load() {
            // TODO: apply any hooks that should always be active
        }

        public override void Unload() {
            // TODO: unapply any hooks applied in Load()
        }
    }
}