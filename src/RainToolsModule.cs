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
            Logger.Log(LogLevel.Verbose, nameof(RainToolsModule), "Loading RainTools with verbose logging");
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(RainToolsModule), LogLevel.Info);
#endif
        }

        public override void Load() {
            ModIntegration.CommunalHelper.Load();

            DecalRegistryProperties.BloomTexture.Load();
            DecalRegistryProperties.ShadowTexture.Load();
            DecalRegistryProperties.LightTexture.Load();

            DecalRegistryProperties.RotationSpeed.Load();
            DecalRegistryProperties.PutImageHere.Load();
            DecalRegistryProperties.AnotherImage.Load();
            DecalRegistryProperties.Override.Load();

            Hooks.hook_Level.Load();
            Hooks.hook_DisplacementRenderer.Load();
            Hooks.hook_LightingRenderer.Load();

            GlobalEntityAttribute.Load();
        }

        public override void Unload() {
            Hooks.hook_Level.Unload();
            Hooks.hook_DisplacementRenderer.Unload();
            Hooks.hook_LightingRenderer.Unload();

            GlobalEntityAttribute.Unload();
        }

    }
}
