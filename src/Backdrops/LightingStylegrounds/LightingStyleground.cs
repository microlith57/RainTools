using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.Backdrops {
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

            if (Color.A == 0 || (Color.R == 0 && Color.G == 0 && Color.B == 0))
                LightVisible = false;
        }

        public virtual void BeforeRenderLighting(Scene scene) { }
        public virtual void RenderLighting(Scene scene) { }

    }
}
