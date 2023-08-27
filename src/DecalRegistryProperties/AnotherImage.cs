using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Celeste.Mod.RainTools.DecalRegistryProperties {
    public static class AnotherImage {

        private class AnotherDecalImage : Component {

            public Decal Decal => (Decal) base.Entity;

            public List<MTexture> Textures;
            public Vector2 Offset = Vector2.Zero;
            public Color Color = Color.White;
            public float Scale = 1f;
            public float Rotation = 0f;
            public float AnimationSpeed = 12f;

            public float frame = 0f;

            public AnotherDecalImage(List<MTexture> textures) : base(true, true) {
                Textures = textures;
            }

            public override void Update() {
                frame = (frame + AnimationSpeed * Engine.DeltaTime) % (float) Textures.Count;
            }

            public override void Render() {
                Textures[(int) frame].DrawCentered(Decal.Position + Offset, Color, Scale, Rotation);
            }

        }

        public static void HandleAnotherImage(Decal decal, XmlAttributeCollection attrs) {
            var textures = decal.textures;
            var offset = Vector2.Zero;
            var color = decal.Color;
            var scale = 1f;
            var rotation = 0f;
            var speed = 12f;

            if (attrs["path"]?.Value is string s_path)
                textures = GFX.Game.GetAtlasSubtextures(s_path);

            if (attrs["frames"]?.Value is string s_frames)
                textures = Calc.ReadCSVIntWithTricks(s_frames).Select(i => textures[i]).ToList();

            if (attrs["offsetx"]?.Value is string s_x)
                offset.X = float.Parse(s_x);

            if (attrs["offsety"]?.Value is string s_y)
                offset.Y = float.Parse(s_y);

            if (attrs["color"]?.Value is string s_col)
                color = Calc.HexToColor(s_col);

            if (attrs["alpha"]?.Value is string s_alpha)
                color *= float.Parse(s_alpha);

            if (attrs["scale"]?.Value is string s_scale)
                scale = float.Parse(s_scale);

            if (attrs["rotation"]?.Value is string s_rotation)
                rotation = float.Parse(s_rotation);

            if (attrs["speed"]?.Value is string s_speed)
                speed = float.Parse(s_speed);

            decal.Add(new AnotherDecalImage(textures) {
                Offset = offset,
                Color = color,
                Scale = scale,
                Rotation = rotation,
                AnimationSpeed = speed
            });
        }

        internal static void Load() {
            DecalRegistry.AddPropertyHandler("raintools_another_image", HandleAnotherImage);
        }

    }
}
