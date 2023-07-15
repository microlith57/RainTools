using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Mod.Backdrops;
using Monocle;

namespace Celeste.Mod.RainTools {
    [CustomBackdrop("RainTools/AmbientLight")]
    public class AmbientLight : LightingStyleground {
        public AmbientLight(BinaryPacker.Element data) {
            UseSpritebatch = true;
            Color = Calc.HexToColor(data.Attr("lightColor"));
        }

        public override void RenderLighting(Scene scene) {
            Draw.Rect(0, 0, 320, 180, Color);
        }
    }
}
