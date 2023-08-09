using Celeste.Mod.Backdrops;
using Monocle;

namespace Celeste.Mod.RainTools.Backdrops {
    [CustomBackdrop("RainTools/AmbientLight")]
    public class AmbientLight : LightingStyleground {

        public AmbientLight(BinaryPacker.Element data) {
            UseSpritebatch = true;
            Color = Calc.HexToColor(data.Attr("lightColor")) * data.AttrFloat("alpha", 1f);
        }

        public override void RenderLighting(Scene scene) {
            Draw.Rect(0, 0, 320, 180, Color * FadeAlphaMultiplier);
        }

    }
}
