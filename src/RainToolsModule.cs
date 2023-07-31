using System;

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
            GlobalEntityAttribute.Load();
            LightingStyleground.Load();
            AltColorgrade.Load();
        }

        public override void Unload() {
            GlobalEntityAttribute.Unload();
            LightingStyleground.Unload();
            AltColorgrade.Unload();
        }
    }
}
