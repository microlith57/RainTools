using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Mod.Backdrops;
using Monocle;

namespace Celeste.Mod.RainTools {
    [CustomBackdrop("RainTools/Sunlight")]
    public class Sunlight : LightingStyleground {
        public const int DOWNRES_FACTOR = 2;

        public float Angle;
        public float Blur1, Blur2;

        private DirectionalLightingRenderer state;

        private RenderTarget2D target;
        // private MTexture clouds;

        public Sunlight(BinaryPacker.Element data) {
            target = new(Engine.Instance.GraphicsDevice,
                         320 / DOWNRES_FACTOR, 180 / DOWNRES_FACTOR,
                         mipMap: false,
                         SurfaceFormat.Color, DepthFormat.Depth16);

            UseSpritebatch = true;

            Angle = data.AttrFloat("angle", (float) Math.PI / 2f);
            Color = Calc.HexToColor(data.Attr("lightColor", "ffffff")) * data.AttrFloat("alpha", 1f);
            Blur1 = data.AttrFloat("blur1", 2f);
            Blur2 = data.AttrFloat("blur2", 1f);

            // clouds = GFX.Game["bgs/microlith57/RainTools/clouds"];
        }

        public override void BeforeRenderLighting(Scene scene) {
            var light = Calc.Rotate(Vector2.UnitX, Angle);
            if (state == null) {
                var shadows = scene.Tracker.GetEntitiesCopy<ShadowCaster>().ConvertAll((e) => e as ShadowCaster);
                state = new(light, shadows);
            } else {
                state.Light = light;
            }

            state.Generate();

            var cam_pos = (scene as Level).Camera.Position;
            cam_pos.X = (float) Math.Round(cam_pos.X);
            cam_pos.Y = (float) Math.Round(cam_pos.Y);

            var offset = (new Vector2(320, 180) - new Vector2(target.Width, target.Height)) / 2f;
            offset += new Vector2(cam_pos.X % 2, cam_pos.Y % 2);

            var mat = Matrix.CreateTranslation(new(-cam_pos, 0f))
                    * Matrix.CreateScale(new Vector3(Vector2.One / DOWNRES_FACTOR, 1f))
                    * Matrix.CreateTranslation(new(offset, 0f));

            Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.TempA);
            Engine.Graphics.GraphicsDevice.Clear(Color.White);
            state.Draw(mat);

            if (Blur1 > 0)
                GaussianBlur.Blur(GameplayBuffers.TempA, GameplayBuffers.TempB, GameplayBuffers.TempA, sampleScale: Blur1);

            // Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            // clouds.Draw(Vector2.Zero, Vector2.Zero, Color.White);
            // Draw.SpriteBatch.End();

            if (Blur2 > 0)
                GaussianBlur.Blur(GameplayBuffers.TempA, GameplayBuffers.TempB, GameplayBuffers.TempA, sampleScale: Blur2);

            Engine.Graphics.GraphicsDevice.SetRenderTarget(target);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            Draw.SpriteBatch.Draw(GameplayBuffers.TempA, -offset, Color.White);
            Draw.SpriteBatch.End();
        }

        public override void RenderLighting(Scene scene) {
            if (target == null || target.IsDisposed)
                return;
            Draw.SpriteBatch.Draw(target, Vector2.Zero, target.Bounds, Color * FadeAlphaMultiplier, 0f, Vector2.Zero, DOWNRES_FACTOR, SpriteEffects.None, 0f);
        }
    }
}
