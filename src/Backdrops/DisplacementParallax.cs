using Monocle;
using Celeste.Mod.Backdrops;

namespace Celeste.Mod.RainTools.Backdrops {
    [CustomBackdrop("RainTools/DisplacementParallax")]
    public class DisplacementParallax : Parallax, IDisplacementStyleground {

        public bool DisplacementVisible { get; private set; } = false;

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

        public void RenderDisplacement(Scene scene) {
            Render(scene);
        }

    }
}
