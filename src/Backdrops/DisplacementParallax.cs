using Monocle;
using Celeste.Mod.Backdrops;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoMod.Utils;

namespace Celeste.Mod.RainTools.Backdrops {
    [CustomBackdrop("RainTools/DisplacementParallax")]
    public class DisplacementParallax : Parallax {

        public bool DisplacementVisible;

        public DisplacementParallax(MTexture texture) : base(texture) { }

        public DisplacementParallax(BinaryPacker.Element data)
            : this(GFX.Game[data.Attr("texture")]) {

            Scroll = new(data.AttrFloat("scrollx", 1f), data.AttrFloat("scrolly", 1f));
            Speed = new(data.AttrFloat("speedx"), data.AttrFloat("speedy"));
            Color = Calc.HexToColor(data.Attr("color", "ffffff")) * data.AttrFloat("alpha");
            FlipX = data.AttrBool("flipx");
            FlipY = data.AttrBool("flipy");
            LoopX = data.AttrBool("loopx", true);
            LoopY = data.AttrBool("loopy", true);
            InstantIn = data.AttrBool("instantIn");
            InstantOut = data.AttrBool("instantOut");
            DoFadeIn = data.AttrBool("fadeIn");
            WindMultiplier = data.AttrFloat("wind");

        }

        public override void Update(Scene scene) {
            base.Update(scene);

            DisplacementVisible = Visible;
            Visible = false;
        }

        #region hook

        internal static void Load() {
            IL.Celeste.DisplacementRenderer.BeforeRender += modBeforeRenderDisplacement;
        }

        internal static void Unload() {
            IL.Celeste.DisplacementRenderer.BeforeRender -= modBeforeRenderDisplacement;
        }

        private static void modBeforeRenderDisplacement(ILContext context) {
            ILCursor cursor = new(context);

            cursor.GotoNext(MoveType.Before, i => i.MatchCallOrCallvirt(out var m)
                                               && m.FullName.EndsWith("Monocle.Draw::get_SpriteBatch()"));

            cursor.Emit(OpCodes.Ldarg_1);
            cursor.EmitDelegate((Scene scene) => {
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                var effects = (scene as Level).Foreground.GetEach<DisplacementParallax>();

                foreach (var effect in effects)
                    if (effect.DisplacementVisible)
                        effect.Render(scene);

                Draw.SpriteBatch.End();
            });
        }

        #endregion

    }
}
