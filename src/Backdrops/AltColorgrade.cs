using Monocle;
using Celeste.Mod.Backdrops;

namespace Celeste.Mod.RainTools.Backdrops {
    [CustomBackdrop("RainTools/AltColorgrade")]
    public class AltColorgrade : Backdrop {

        public bool Enabled = true;
        public string ColorgradeA = "none";
        public string ColorgradeB = "none";
        public float LerpFactor = 0f;

        public AltColorgrade() : base() {
            Enabled = true;
            Visible = false;
        }

        public AltColorgrade(BinaryPacker.Element data) {
            Color = Calc.HexToColor(data.Attr("color")) * data.AttrFloat("alpha", 1f);

            ColorgradeA = data.Attr("colorgradeA", "none");
            ColorgradeB = data.Attr("colorgradeA", ColorgradeA);
            LerpFactor = data.AttrFloat("blendFactor");
        }

        public override void Update(Scene scene) {
            base.Update(scene);

            Enabled = Visible;
            Visible = false;
        }

    }
}
