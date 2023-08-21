using Monocle;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.RainTools.Hooks {

    internal static class hook_Level {

        internal static void Load() {
            IL.Celeste.Level.Render += IL_Level_Render;
        }
        internal static void Unload() {
            IL.Celeste.Level.Render -= IL_Level_Render;
        }

        private static void IL_Level_Render(ILContext context) {
            var cursor = new ILCursor(context);

            cursor.GotoNext(MoveType.Before,
                            i => i.MatchLdnull(),
                            i => i.MatchCallOrCallvirt<GraphicsDevice>("SetRenderTarget"));
            cursor.GotoPrev(MoveType.AfterLabel,
                            i => i.MatchCallOrCallvirt<Engine>("get_Instance"),
                            i => i.MatchCallOrCallvirt<Game>("get_GraphicsDevice"));

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate(RenderAltColorgrades);
        }

        private static void RenderAltColorgrades(Level level) {
            if (!ColorGrade.Enabled)
                return;

            var buffer = GameplayBuffers.Level;
            var temp = GameplayBuffers.TempA;

            var bgs = level.Foreground.GetEach<Backdrops.AltColorgrade>()
                                      .Cast<Backdrops.AltColorgrade>()
                                      .Where(c => c.Enabled
                                               && c.FadeAlphaMultiplier > 0f
                                               && c.Color.A > 0);

            foreach (var bg in bgs) {
                var a = GFX.ColorGrades.GetOrDefault(bg.ColorgradeA, GFX.ColorGrades["none"]);
                var b = GFX.ColorGrades.GetOrDefault(bg.ColorgradeB, GFX.ColorGrades["none"]);
                var fac = bg.LerpFactor;
                var blendState = FrostHelper.API.API.GetBackdropBlendState(bg) ?? BlendState.AlphaBlend;

                Engine.Instance.GraphicsDevice.SetRenderTarget(temp);
                ColorGrade.Set(a, b, fac);
                Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, GFX.FxColorGrading, Matrix.Identity);
                Draw.SpriteBatch.Draw(buffer, Vector2.Zero, Color.White);
                Draw.SpriteBatch.End();

                Engine.Instance.GraphicsDevice.SetRenderTarget(buffer);
                Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                Draw.SpriteBatch.Draw(temp, Vector2.Zero, bg.Color * bg.FadeAlphaMultiplier);
                Draw.SpriteBatch.End();
            }
        }

    }

}
