using Monocle;
using Celeste.Mod.Backdrops;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Celeste.Mod.RainTools.Backdrops {
    [CustomBackdrop("RainTools/BetterHeatwave")]
    public class Heatwave : Backdrop, IDisplacementStyleground {

        public class Blob {

            private static Queue<Blob> pool = new();

            public static Blob Get() {
                if (pool.Count > 0)
                    return pool.Dequeue();

                return new();
            }

            public static void Recycle(Blob blob) => pool.Enqueue(blob);

            public MTexture Texture;
            public Vector2 Position;
            public Vector2 Velocity;
            public float Scale;
            public float ScaleVelocity;
            public float Rotation;

            public void Set(MTexture texture,
                            Vector2 minVelocity, Vector2 maxVelocity,
                            float minScaleVelocity, float maxScaleVelocity) {

                Texture = texture;

                Position = Calc.Random.Range(Vector2.Zero, SIZE);
                Velocity = Calc.Random.Range(minVelocity, maxVelocity);

                Scale = 0f;
                ScaleVelocity = Calc.Random.Range(minScaleVelocity, maxScaleVelocity);

                Rotation = Calc.Random.Choose(0f, (float) Math.PI);
            }

            public void Update(float scaleAcceleration, float maxScale) {
                Position += Velocity * Engine.DeltaTime;

                ScaleVelocity += scaleAcceleration;
                Scale = Calc.Clamp(Scale + ScaleVelocity * Engine.DeltaTime, 0f, maxScale);
            }

            public void Render(Vector2 camera, float multiplier, float alpha) {
                var scale = Scale * multiplier;

                if (scale <= 0f)
                    return;

                var pos = Position - camera;
                pos.X = Utils.Mod(pos.X, SIZE.X) - OFFSET.X;
                pos.Y = Utils.Mod(pos.Y, SIZE.Y) - OFFSET.Y;

                Texture.DrawCentered(pos, Color.White * alpha, scale, Rotation);
            }

        }

        private static Vector2 OFFSET = new(64f, 64f);
        private static Vector2 SIZE = new Vector2(320f, 180f) + 2 * OFFSET;

        public bool DisplacementVisible { get; private set; } = false;

        public List<Blob> Blobs;
        public int TargetCount;
        public List<MTexture> Textures;

        public Vector2 MinVelocity, MaxVelocity;

        public float MaxScale;
        public float MinScaleVelocity, MaxScaleVelocity;
        public float ScaleAcceleration;

        public float Alpha;

        private bool firstFrame = true;

        public Heatwave(BinaryPacker.Element data) {

            TargetCount = data.AttrInt("blobCount", 100);
            Blobs = new();

            Textures = GFX.Game.GetAtlasSubtextures(data.Attr("texture"));

            MinVelocity = new(data.AttrFloat("minVelX", -1f), data.AttrFloat("minVelY", -15f));
            MaxVelocity = new(data.AttrFloat("maxVelX", +1f), data.AttrFloat("maxVelY", -20f));
            MinScaleVelocity = data.AttrFloat("minScaleVel", 0.05f);
            MaxScaleVelocity = data.AttrFloat("maxScaleVel", 0.3f);
            ScaleAcceleration = data.AttrFloat("scaleAcceleration", -0.001f);
            MaxScale = data.AttrFloat("maxScale", 1f);

            Alpha = data.AttrFloat("distortAlpha", 0.05f);

        }

        public override void Update(Scene scene) {
            base.Update(scene);

            if (firstFrame) {
                Blobs.Clear();

                for (int i = 0; i < TargetCount; i++) {
                    var texture = Calc.Random.Choose(Textures);

                    var blob = Blob.Get();
                    blob.Set(texture, MinVelocity, MaxVelocity, 0, 0);
                    blob.Scale = Calc.Random.Range(0f, MaxScale);

                    Blobs.Add(blob);
                }

                firstFrame = false;
            }

            float scale = FadeAlphaMultiplier * (Color.A / 255f);

            if ((Blobs.Count == 0 && TargetCount == 0) || scale <= 0f) {
                DisplacementVisible = Visible = false;
                return;
            }

            Blobs.RemoveAll(blob => {
                blob.Update(ScaleAcceleration, MaxScale);

                if (blob.Scale <= 0f) {
                    Blob.Recycle(blob);
                    return true;
                }

                return false;
            });

            var toAdd = TargetCount - Blobs.Count;
            if (toAdd > 0)
                for (int i = 0; i < toAdd; i++) {
                    var texture = Calc.Random.Choose(Textures);

                    var blob = Blob.Get();
                    blob.Set(texture, MinVelocity, MaxVelocity, MinScaleVelocity, MaxScaleVelocity);

                    Blobs.Add(blob);
                }

            DisplacementVisible = Blobs.Count > 0;
        }

        public void RenderDisplacement(Scene scene) {
            var camera = (scene as Level).Camera.Position;
            float scale = FadeAlphaMultiplier * (Color.A / 255f);

            if (scale <= 0f)
                return;

            foreach (var blob in Blobs)
                blob.Render(camera, scale, Alpha);
        }

    }
}
