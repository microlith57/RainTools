using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Linq;

namespace Celeste.Mod.RainTools {
    public static class AltColorgrade {
        [Tracked]
        public class Controller : Component {
            public bool Enabled = true;
            public float Alpha = 1f;
            public string ColorgradeA = "none";
            public string ColorgradeB = "none";
            public float LerpFactor = 0f;

            public string Tag;

            public bool HasEffect => Enabled
                                  && (Alpha > 0f)
                                  && !(ColorgradeA == "none" && (ColorgradeB == "none" || LerpFactor == 0f))
                                  && !(ColorgradeB == "none" && LerpFactor == 1f);

            public Controller() : base(false, false) { }
        }

        private static void Render(Level level) {
            if (!ColorGrade.Enabled)
                return;

            var buffer = GameplayBuffers.Level;
            var temp = GameplayBuffers.TempA;

            var controllers = level.Tracker.GetComponents<AltColorgrade.Controller>()
                                           .Cast<AltColorgrade.Controller>()
                                           .Where((c) => c.HasEffect)
                                           .OrderBy((c) => c.Entity.Depth);

            foreach (var controller in controllers) {
                MTexture a = GFX.ColorGrades.GetOrDefault(controller.ColorgradeA, GFX.ColorGrades["none"]);
                MTexture b = GFX.ColorGrades.GetOrDefault(controller.ColorgradeB, GFX.ColorGrades["none"]);

                Engine.Instance.GraphicsDevice.SetRenderTarget(temp);
                ColorGrade.Set(a, b, controller.LerpFactor);
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, GFX.FxColorGrading, Matrix.Identity);
                Draw.SpriteBatch.Draw(buffer, Vector2.Zero, Color.White);
                Draw.SpriteBatch.End();

                Engine.Instance.GraphicsDevice.SetRenderTarget(buffer);
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                Draw.SpriteBatch.Draw(temp, Vector2.Zero, Color.White * controller.Alpha);
                Draw.SpriteBatch.End();
            }
        }

        #region hook

        internal static void Load() {
            IL.Celeste.Level.Render += modLevelRender;
        }
        internal static void Unload() {
            IL.Celeste.Level.Render -= modLevelRender;
        }

        private static void modLevelRender(ILContext context) {
            var cursor = new ILCursor(context);

            cursor.GotoNext(MoveType.AfterLabel,
                            i => i.MatchCallOrCallvirt<Engine>("get_Instance"),
                            i => i.MatchCallOrCallvirt<Game>("get_GraphicsDevice"),
                            i => i.MatchLdnull(),
                            i => i.MatchCallOrCallvirt<GraphicsDevice>("SetRenderTarget"));

            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate(Render);
        }

        #endregion
    }
}
