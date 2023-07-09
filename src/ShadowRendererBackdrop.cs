using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Mod.Backdrops;
using Monocle;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [CustomBackdrop("RainTools/ShadowRenderer")]
    public class ShadowRendererBackdrop : Backdrop {
        public const int DOWNRES_FACTOR = 2;

        private List<ShadowCaster> shadows;
        private ShadowRenderer.State state;

        private RenderTarget2D target;

        public ShadowRendererBackdrop(BinaryPacker.Element data) {
            target = new(Engine.Instance.GraphicsDevice,
                         320 / DOWNRES_FACTOR, 180 / DOWNRES_FACTOR,
                         mipMap: false,
                         SurfaceFormat.Color, DepthFormat.Depth16);
            UseSpritebatch = false;
        }

        public override void BeforeRender(Scene scene) {
            base.BeforeRender(scene);

            if (shadows == null || state == null) {
                shadows = scene.Tracker.GetEntitiesCopy<ShadowCaster>().ConvertAll((e) => e as ShadowCaster);
                shadows.Sort((a, b) => a.Y < b.Y ? -1 : 1);
                int count = shadows.Sum((e) => e.MaxTriCount * 3);

                var bounds = (scene as Level).Bounds;
                var pos = bounds.Center.ToVector2();
                var radius = (float) Math.Sqrt(Math.Pow(bounds.Width / 2, 2) + Math.Pow(bounds.Height / 2, 2));

                var light = -Vector2.UnitY;

                state = new(count, light, pos, radius);
            }

            state.v = 0;
            state.Light = Calc.Rotate(state.Light, 0.2f * Engine.DeltaTime);
            foreach (var shadow in shadows) {
                shadow.UpdateVerts(state);
            }

            var cam_pos = (scene as Level).Camera.Position;
            cam_pos.X = (float) Math.Round(cam_pos.X);
            cam_pos.Y = (float) Math.Round(cam_pos.Y);

            var offset = (new Vector2(320, 180) - new Vector2(target.Width, target.Height)) / 2f;

            var mat = Matrix.CreateTranslation(new(-cam_pos, 0f))
                    * Matrix.CreateScale(new Vector3(Vector2.One / DOWNRES_FACTOR, 1f))
                    * Matrix.CreateTranslation(new(offset, 0f));

            Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.TempA);
            Engine.Graphics.GraphicsDevice.Clear(Color.White);
            GFX.DrawVertices(mat, state.verts, state.v);

            GaussianBlur.Blur(GameplayBuffers.TempA, GameplayBuffers.TempB, GameplayBuffers.TempA, sampleScale: 2f);
            GaussianBlur.Blur(GameplayBuffers.TempA, GameplayBuffers.TempB, GameplayBuffers.TempA, sampleScale: 1f);

            Engine.Graphics.GraphicsDevice.SetRenderTarget(target);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            Draw.SpriteBatch.Draw(GameplayBuffers.TempA, -offset, Color.White);
            Draw.SpriteBatch.End();
        }

        public override void Render(Scene scene) {
            base.Render(scene);

            if (target != null && !target.IsDisposed) {
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
                Draw.SpriteBatch.Draw(target, Vector2.Zero, target.Bounds, Color.White * 0.8f, 0f, Vector2.Zero, DOWNRES_FACTOR, SpriteEffects.None, 0f);
                Draw.SpriteBatch.End();
            }
        }
    }
}
