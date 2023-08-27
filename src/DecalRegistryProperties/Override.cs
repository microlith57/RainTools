using Monocle;
using System.Xml;

namespace Celeste.Mod.RainTools.DecalRegistryProperties {
    public static class Override {

        public static void HandleOverrideDecal(Decal decal, XmlAttributeCollection attrs) {
            if (attrs["color"]?.Value is string s_col)
                decal.Color = Calc.HexToColorWithAlpha(s_col);

            if (attrs["rotation"]?.Value is string s_rotation)
                decal.Rotation = float.Parse(s_rotation);

            if (attrs["speed"]?.Value is string s_speed)
                decal.AnimationSpeed = float.Parse(s_speed);
        }

        internal static void Load() {
            DecalRegistry.AddPropertyHandler("raintools_override", HandleOverrideDecal);
        }

    }
}
