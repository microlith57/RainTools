using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Mod.Backdrops;
using Monocle;
using Celeste.Mod.RainTools.ShadowCasters;
using System.Linq;

namespace Celeste.Mod.RainTools.Backdrops {
    [CustomBackdrop("RainTools/Sunlight")]
    public class Sunlight : LightingStyleground, IHasAngle {
        public const int DOWNRES_FACTOR = 2;
        public static Vector2 RENDER_OFFSET = new Vector2(320, 180) / 2f - new Vector2(320, 180) / (2f * DOWNRES_FACTOR);

        public float Angle { get; set; } = 0f;
        public float Blur1, Blur2;

        private DirectionalLightingRenderer state;

        private RenderTarget2D target;
        // private MTexture clouds;

        private Vector2 offset;

        public Sunlight(BinaryPacker.Element data) {
            target = new(Engine.Instance.GraphicsDevice,
                         320, 180,
                         mipMap: false,
                         SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            UseSpritebatch = true;

            Angle = data.AttrFloat("angleOffsetDegrees", 90) * Calc.DegToRad;
            Color = Calc.HexToColor(data.Attr("lightColor", "ffffff")) * data.AttrFloat("alpha", 1f);
            Blur1 = data.AttrFloat("blur1", 2f);
            Blur2 = data.AttrFloat("blur2", 1f);

            // clouds = GFX.Game["bgs/microlith57/RainTools/clouds"];
        }

        public override void BeforeRenderLighting(Scene scene) {
            var level = scene as Level;

            var light = Calc.Rotate(Vector2.UnitX, Angle);
            if (state == null) {
                var shadows = scene.Tracker.GetEntitiesCopy<ShadowCaster>().ConvertAll((e) => e as ShadowCaster);
                state = new(light, shadows);
            } else {
                state.Light = light;
            }

            state.Generate();
            if (state.verts == null || state.v <= 0)
                return;

            var snapped = new Vector2((float) Math.Round(level.Camera.Position.X / 2) * 2,
                                      (float) Math.Round(level.Camera.Position.Y / 2) * 2);
            var pos = level.Camera.Position;
            offset = snapped - pos;

            var matrix = Matrix.CreateTranslation(RENDER_OFFSET.X - snapped.X, RENDER_OFFSET.Y - snapped.Y, 0f) * Matrix.CreateScale(1f / DOWNRES_FACTOR, 1f / DOWNRES_FACTOR, 0.5f);

            Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.TempA);
            Engine.Graphics.GraphicsDevice.Clear(Color.White);
            Engine.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            state.Draw(matrix);

            DrawCustomShadows(scene, matrix);

            if (Blur1 > 0)
                GaussianBlur.Blur(GameplayBuffers.TempA, GameplayBuffers.TempB, GameplayBuffers.TempA, sampleScale: Blur1);

            // Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            // clouds.Draw(Vector2.Zero, Vector2.Zero, Color.White);
            // Draw.SpriteBatch.End();

            if (Blur2 > 0)
                GaussianBlur.Blur(GameplayBuffers.TempA, GameplayBuffers.TempB, GameplayBuffers.TempA, sampleScale: Blur2);

            Engine.Graphics.GraphicsDevice.SetRenderTarget(target);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            Draw.SpriteBatch.Draw(GameplayBuffers.TempA, new Vector2(320, 180) / 2f, target.Bounds, Color.White, 0f, new Vector2(target.Width, target.Height) / 2f, 1f, SpriteEffects.None, 0f);
            Draw.SpriteBatch.End();
        }

        private void DrawCustomShadows(Scene scene, Matrix matrix) {
            var shadows = (scene as Level).Tracker.GetComponents<CustomShadow>()
                                                  .Cast<CustomShadow>()
                                                  .OrderBy(shadow => shadow.Entity.actualDepth)
                                                  .ToList();
            if (!shadows.Any())
                return;

            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, matrix);

            foreach (var shadow in shadows)
                shadow.OnRenderShadow(state);

            Draw.SpriteBatch.End();
        }

        public override void RenderLighting(Scene scene) {
            if (target == null || target.IsDisposed)
                return;
            var pos = new Vector2(320, 180) / 2;
            var tl = pos - new Vector2(target.Width, target.Height) / 2 + offset - RENDER_OFFSET;

            Draw.SpriteBatch.Draw(target, tl, target.Bounds, Color * FadeAlphaMultiplier, 0f, Vector2.Zero, DOWNRES_FACTOR, SpriteEffects.None, 0f);
        }

    }
}
