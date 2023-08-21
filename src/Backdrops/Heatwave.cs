using Monocle;
using Celeste.Mod.Backdrops;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.RainTools.Backdrops {
    [CustomBackdrop("RainTools/Heatwave")]
    public class Heatwave : Backdrop, IDisplacementStyleground {

        public struct Blob {

            public MTexture Texture;
            public Vector2 Position;
            public Vector2 Velocity;
            public float Scale;
            public float ScaleVelocity;
            public float Rotation;

            public void Set(MTexture texture,
                            Vector2 position,
                            Vector2 minVelocity, Vector2 maxVelocity,
                            float minScaleVelocity, float maxScaleVelocity) {

                Texture = texture;

                Position = position;
                Velocity = Calc.Random.Range(minVelocity, maxVelocity);

                Scale = 0f;
                ScaleVelocity = Calc.Random.Range(minScaleVelocity, maxScaleVelocity);

                Rotation = Calc.Random.NextAngle();

            }

            public void Update(float scaleAcceleration, float dt) {
                Position += Velocity * dt;

                ScaleVelocity += scaleAcceleration;
                Scale = Calc.Clamp(Scale + ScaleVelocity * dt, 0f, 1.8f);
            }

            public readonly void Render(Vector2 camera, float multiplier, float alpha) {
                var scale = Scale * multiplier;

                if (scale <= 0f)
                    return;

                var pos = Position - camera;
                pos.X = Utils.Mod(pos.X, SIZE.X);
                pos.Y = Utils.Mod(pos.Y, SIZE.Y);

                Texture.DrawCentered(pos, Microsoft.Xna.Framework.Color.White * alpha, scale, Rotation);
            }

        }

        private static Vector2 OFFSET = new(64f, 64f);
        private static Vector2 SIZE = new Vector2(320f, 180f) + 2 * OFFSET;

        public bool DisplacementVisible { get; private set; } = false;

        public Blob[] Blobs;
        public int TargetCount;
        public List<MTexture> Textures;

        public Vector2 MinVelocity, MaxVelocity;
        public float MinScaleVelocity, MaxScaleVelocity;

        public float Alpha = 15;

        private bool firstFrame = true;

        public Heatwave(BinaryPacker.Element data) {

            TargetCount = data.AttrInt("blobCount", 100);
            Blobs = new Blob[TargetCount];

            Textures = GFX.Game.GetAtlasSubtextures(data.Attr("texture"));

            MinVelocity = new(data.AttrFloat("minVelX", -1f), data.AttrFloat("minVelY", 1f));
            MaxVelocity = new(data.AttrFloat("maxVelX", -10f), data.AttrFloat("maxVelY", -20f));
            MinScaleVelocity = data.AttrFloat("minScaleVel", 0.05f);
            MaxScaleVelocity = data.AttrFloat("minScaleVel", 0.3f);

            Scroll = new(data.AttrFloat("scrollx", 1f), data.AttrFloat("scrolly", 1f));

            Alpha = data.AttrFloat("alpha", 15f / 255f);

            for (int i = 0; i < Blobs.Length; i++) {
                var texture = Calc.Random.Choose(Textures);
                var position = Calc.Random.Range(Vector2.Zero, SIZE) - OFFSET;

                Blobs[i] = new();
                Blobs[i].Set(texture, position, MinVelocity, MaxVelocity, MinScaleVelocity, MaxScaleVelocity);
            }

        }

        public override void Update(Scene scene) {
            base.Update(scene);

            DisplacementVisible = Visible;
            Visible = false;

            if (FadeAlphaMultiplier <= 0f)
                return;

            List<int> toRefresh = new();

            for (int i = 0; i < Blobs.Length; i++) {
                Blobs[i].Update(-0.001f, firstFrame ? Engine.DeltaTime * 60f : Engine.DeltaTime);

                if (Blobs[i].Scale <= 0)
                    toRefresh.Add(i);
            }

            var numToRefresh = TargetCount - (Blobs.Length - toRefresh.Count);
            if (numToRefresh > 0)
                for (int i = 0; i < numToRefresh; i++) {
                    var texture = Calc.Random.Choose(Textures);
                    var position = Calc.Random.Range(Vector2.Zero, SIZE) - OFFSET;

                    Blobs[toRefresh[i]].Set(texture, position, MinVelocity, MaxVelocity, MinScaleVelocity, MaxScaleVelocity);
                }

            firstFrame = false;
        }

        public void RenderDisplacement(Scene scene) {
            var camera = (scene as Level).Camera.Position;

            if (FadeAlphaMultiplier <= 0f)
                return;

            foreach (var blob in Blobs)
                blob.Render(camera, FadeAlphaMultiplier, Alpha);
        }

    }
}
