using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Linq;

namespace Celeste.Mod.RainTools {
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
        }

        public virtual void BeforeRenderLighting(Scene scene) { }
        public virtual void RenderLighting(Scene scene) { }

        internal static void Load() {
            On.Celeste.LightingRenderer.BeforeRender += LightingRenderer_BeforeRender;
        }

        internal static void Unload() {
            On.Celeste.LightingRenderer.BeforeRender -= LightingRenderer_BeforeRender;
        }

        private static void LightingRenderer_BeforeRender(On.Celeste.LightingRenderer.orig_BeforeRender orig, LightingRenderer self, Scene scene) {
            orig(self, scene);

            var level = (Level) scene;
            var bgs = level.Foreground.GetEach<LightingStyleground>().ToList();
            if (bgs.Count == 0)
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
                        StartSpritebatch(ref usingSpritebatch);
                    else
                        EndSpritebatch(ref usingSpritebatch);

                    backdrop.RenderLighting(level);
                }
            }
            if (usingSpritebatch)
                EndSpritebatch(ref usingSpritebatch);

        }

        private static void StartSpritebatch(ref bool usingSpritebatch) {
            if (usingSpritebatch)
                return;

            Draw.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Additive,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise
            );
            usingSpritebatch = true;
        }

        private static void EndSpritebatch(ref bool usingSpritebatch) {
            if (!usingSpritebatch)
                return;

            Draw.SpriteBatch.End();
            usingSpritebatch = false;
        }
    }
}
