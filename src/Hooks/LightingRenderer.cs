using Monocle;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.RainTools.Hooks {

    internal static class hook_LightingRenderer {

        internal static void Load() {
            On.Celeste.LightingRenderer.BeforeRender += On_LightingRenderer_BeforeRender;
        }

        internal static void Unload() {
            On.Celeste.LightingRenderer.BeforeRender -= On_LightingRenderer_BeforeRender;
        }

        private static void On_LightingRenderer_BeforeRender(On.Celeste.LightingRenderer.orig_BeforeRender orig, LightingRenderer self, Scene scene) {
            orig(self, scene);

            var level = (Level) scene;

            var bgs = level.Foreground.GetEach<Backdrops.LightingStyleground>()
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
                        StartSpritebatch(ref usingSpritebatch);
                    else
                        EndSpritebatch(ref usingSpritebatch);

                    backdrop.RenderLighting(level);
                }
            }
            EndSpritebatch(ref usingSpritebatch);

            if (lights.Count > 0) {
                StartSpritebatch(ref usingSpritebatch, Matrix.CreateTranslation(new(-level.Camera.Position, 0)));

                foreach (var light in lights)
                    light.OnRenderLight();

                EndSpritebatch(ref usingSpritebatch);
            }
        }

        private static void StartSpritebatch(ref bool usingSpritebatch) {
            StartSpritebatch(ref usingSpritebatch, Matrix.Identity);
        }

        private static void StartSpritebatch(ref bool usingSpritebatch, Matrix matrix) {
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

        private static void EndSpritebatch(ref bool usingSpritebatch) {
            if (!usingSpritebatch)
                return;

            Draw.SpriteBatch.End();
            usingSpritebatch = false;
        }

    }

}
