using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Celeste.Mod.RainTools.DecalRegistryProperties {
    public static class LightTexture {

        public class LightImage : ShadowCasters.CustomLight {

            private Color Color;
            private List<MTexture> Textures;
            private float frame = 0f;

            private Decal Decal => (Decal) Entity;

            public LightImage(Color color, List<MTexture> textures) : base(() => { }) {
                Active = true;
                Color = color;
                Textures = textures;

                OnRenderLight = RenderLight;
            }

            public override void Update() {
                if (Textures.Count > 0)
                    frame = (frame + Decal.AnimationSpeed * Engine.DeltaTime) % Textures.Count;
            }

            private void RenderLight() {
                if (Textures.Count > 0)
                    Textures[(int) frame].DrawCentered(Decal.Position, Color, Decal.scale);
            }

        }

        public static void HandleLightTextureDecal(Decal decal, XmlAttributeCollection attrs) {
            var color = decal.Color;
            var textures = decal.textures;

            if (attrs["color"]?.Value is string s_col)
                color = Calc.HexToColor(s_col);

            if (attrs["alpha"]?.Value is string s_alpha)
                color *= float.Parse(s_alpha);

            if (attrs["path"]?.Value is string s_path)
                textures = GFX.Game.GetAtlasSubtextures(s_path);

            if (attrs["frames"]?.Value is string s_frames)
                textures = Calc.ReadCSVIntWithTricks(s_frames).Select(i => textures[i]).ToList();

            decal.Add(new LightImage(color, textures));

            if ((attrs["replace"]?.Value ?? "false") == "true")
                decal.Color = Color.Transparent;
        }

        internal static void Load() {
            DecalRegistry.AddPropertyHandler("raintools_light_texture", HandleLightTextureDecal);
        }

    }
}
