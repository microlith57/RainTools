using Celeste.Mod.RainTools.ShadowCasters;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Celeste.Mod.RainTools.DecalRegistryProperties {
    public static class ShadowTexture {

        public class ShadowImage : CustomShadow {

            private float Offset;
            private Color Color;
            private List<MTexture> Textures;
            private float frame = 0f;

            private Decal Decal => (Decal) Entity;

            public ShadowImage(Color color, List<MTexture> textures, float offset = 0f) : base((_) => { }) {
                Active = true;
                Offset = offset;
                Color = color;
                Textures = textures;

                OnRenderShadow = RenderShadow;
            }

            public override void Update() {
                if (Textures.Count > 0)
                    frame = (frame + Decal.AnimationSpeed * Engine.DeltaTime) % Textures.Count;
            }

            private void RenderShadow(DirectionalLightingRenderer state) {
                if (Textures.Count > 0)
                    Textures[(int) frame].DrawCentered(Decal.Position + state.Light * Offset, Color, Decal.scale, Decal.Rotation);
            }

        }

        public static void HandleShadowTextureDecal(Decal decal, XmlAttributeCollection attrs) {
            var color = decal.Color;
            var textures = decal.textures;
            var offset = 0f;

            if (attrs["color"]?.Value is string s_col)
                color = Calc.HexToColor(s_col);

            if (attrs["alpha"]?.Value is string s_alpha)
                color *= float.Parse(s_alpha);

            if (attrs["offset"]?.Value is string s_offset)
                offset = float.Parse(s_offset);

            if (attrs["path"]?.Value is string s_path)
                textures = GFX.Game.GetAtlasSubtextures(s_path);

            if (attrs["frames"]?.Value is string s_frames)
                textures = Calc.ReadCSVIntWithTricks(s_frames).Select(i => textures[i]).ToList();

            decal.Add(new ShadowImage(color, textures, offset));

            if ((attrs["replace"]?.Value ?? "false") == "true")
                decal.Color = Color.Transparent;
        }

        internal static void Load() {
            DecalRegistry.AddPropertyHandler("raintools_shadow_texture", HandleShadowTextureDecal);
        }

    }
}
