using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Linq;
using Celeste.Mod.Backdrops;

namespace Celeste.Mod.RainTools {
    [CustomBackdrop("RainTools/AltColorgrade")]
    public class AltColorgrade : Backdrop {
        public bool Enabled = true;
        public float Alpha = 1f;
        public string ColorgradeA = "none";
        public string ColorgradeB = "none";
        public float LerpFactor = 0f;

        public bool HasEffect => Enabled
                                && (Alpha > 0f)
                                && (Color.A > 0)
                                && !(ColorgradeA == "none" && (ColorgradeB == "none" || LerpFactor == 0f))
                                && !(ColorgradeB == "none" && LerpFactor == 1f);

        public AltColorgrade() : base() {
            Enabled = true;
            Visible = false;
        }

        public AltColorgrade(BinaryPacker.Element data) {
            Color = Calc.HexToColor(data.Attr("color"));
            Alpha = data.AttrFloat("alpha", 1f);

            ColorgradeA = data.Attr("colorgradeA", "none");
            ColorgradeB = data.Attr("colorgradeA", ColorgradeA);
            LerpFactor = data.AttrFloat("blendFactor");
        }

        public override void Update(Scene scene) {
            base.Update(scene);

            Enabled = Visible;
            Visible = false;
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
            cursor.EmitDelegate(renderAll);
        }

        private static void renderAll(Level level) {
            if (!ColorGrade.Enabled)
                return;

            var buffer = GameplayBuffers.Level;
            var temp = GameplayBuffers.TempA;

            var bgs = level.Foreground.GetEach<AltColorgrade>()
                                      .Cast<AltColorgrade>()
                                      .Where(c => c.HasEffect);

            foreach (var bg in bgs) {
                MTexture a = GFX.ColorGrades.GetOrDefault(bg.ColorgradeA, GFX.ColorGrades["none"]);
                MTexture b = GFX.ColorGrades.GetOrDefault(bg.ColorgradeB, GFX.ColorGrades["none"]);

                Engine.Instance.GraphicsDevice.SetRenderTarget(temp);
                ColorGrade.Set(a, b, bg.LerpFactor);
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, GFX.FxColorGrading, Matrix.Identity);
                Draw.SpriteBatch.Draw(buffer, Vector2.Zero, Color.White);
                Draw.SpriteBatch.End();

                Engine.Instance.GraphicsDevice.SetRenderTarget(buffer);
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                Draw.SpriteBatch.Draw(temp, Vector2.Zero, bg.Color * bg.Alpha * bg.FadeAlphaMultiplier);
                Draw.SpriteBatch.End();
            }
        }

        #endregion
    }
}
