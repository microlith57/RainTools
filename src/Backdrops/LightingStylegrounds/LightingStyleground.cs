using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Linq;

namespace Celeste.Mod.RainTools.Backdrops {
    public abstract class LightingStyleground : Backdrop {

        public bool LightVisible;

        public LightingStyleground() : base() {
            LightVisible = true;
            Visible = false;
        }

        public override void Update(Scene scene) {
            base.Update(scene);

            LightVisible = Visible;
            Visible = false;

            if (Color.A == 0 || (Color.R == 0 && Color.G == 0 && Color.B == 0))
                LightVisible = false;
        }

        public virtual void BeforeRenderLighting(Scene scene) { }
        public virtual void RenderLighting(Scene scene) { }

        #region hook

        internal static void Load() {
            On.Celeste.LightingRenderer.BeforeRender += LightingRenderer_BeforeRender;
        }

        internal static void Unload() {
            On.Celeste.LightingRenderer.BeforeRender -= LightingRenderer_BeforeRender;
        }

        private static void LightingRenderer_BeforeRender(On.Celeste.LightingRenderer.orig_BeforeRender orig, LightingRenderer self, Scene scene) {
            orig(self, scene);

            var level = (Level) scene;

            var bgs = level.Foreground.GetEach<LightingStyleground>()
                                      .ToList();

            var lights = level.Tracker.GetComponents<ShadowCasters.CustomLight>()
                                      .Cast<ShadowCasters.CustomLight>()
                                      .ToList();

            if (bgs.Count == 0 && lights.Count == 0)
                return;

            foreach (var backdrop in bgs) {
                if (backdrop.LightVisible) {
                    backdrop.BeforeRenderLighting(level);
                }
            }

            Engine.Graphics.GraphicsDevice.SetRenderTarget(GameplayBuffers.Light);

            var usingSpritebatch = false;

            foreach (var backdrop in bgs) {
                if (backdrop.LightVisible) {
                    if (backdrop.UseSpritebatch)
                        startSpritebatch(ref usingSpritebatch);
                    else
                        endSpritebatch(ref usingSpritebatch);

                    backdrop.RenderLighting(level);
                }
            }
            endSpritebatch(ref usingSpritebatch);

            if (lights.Count > 0) {
                startSpritebatch(ref usingSpritebatch, Matrix.CreateTranslation(new(-level.Camera.Position, 0)));

                foreach (var light in lights)
                    light.OnRenderLight();

                endSpritebatch(ref usingSpritebatch);
            }
        }

        private static void startSpritebatch(ref bool usingSpritebatch) {
            startSpritebatch(ref usingSpritebatch, Matrix.Identity);
        }

        private static void startSpritebatch(ref bool usingSpritebatch, Matrix matrix) {
            if (usingSpritebatch)
                return;

            Draw.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Additive,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null, matrix
            );
            usingSpritebatch = true;
        }

        private static void endSpritebatch(ref bool usingSpritebatch) {
            if (!usingSpritebatch)
                return;

            Draw.SpriteBatch.End();
            usingSpritebatch = false;
        }

        #endregion

    }
}
