using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools {
    [CustomEntity("RainTools/ShadowRectangle")]
    public class ShadowRectangle : ShadowCaster {
        public float ShadowLength, Offset;
        public Color Color;

        public ShadowRectangle(EntityData data, Vector2 offset)
            : base(data.Position + offset, maxTriCount: 4) {

            Collider = new Hitbox(data.Width, data.Height);
            Collidable = false;

            Offset = data.Float("offset", 0f);
            ShadowLength = data.Float("length", 400f);

            var letsInLight = data.Bool("letsInLight", false);
            var alpha = data.Float("alpha", 1f);

            if (letsInLight) {
                Color = Color.White * alpha;
            } else {
                Color = Color.Black * alpha;
            }
        }

        public override void UpdateVerts(ShadowRenderer.State state) {
            Vector2 a, b, c;
            if (state.Light.Y >= 0) {
                if (state.Light.X >= 0) {
                    a = BottomLeft + state.Light * Offset;
                    b = TopLeft + state.Light * Offset;
                    c = TopRight + state.Light * Offset;
                } else {
                    a = TopLeft + state.Light * Offset;
                    b = TopRight + state.Light * Offset;
                    c = BottomRight + state.Light * Offset;
                }
            } else {
                if (state.Light.X >= 0) {
                    a = TopLeft + state.Light * Offset;
                    b = BottomLeft + state.Light * Offset;
                    c = BottomRight + state.Light * Offset;
                } else {
                    a = BottomLeft + state.Light * Offset;
                    b = BottomRight + state.Light * Offset;
                    c = TopRight + state.Light * Offset;
                }
            }

            Vector2 offset = state.Light * ShadowLength;
            Vector2 diag = (a - b) + (c - b);

            float depthA = state.ZPositionFor(a);
            var a_n = new Vector3(a, depthA);
            var a_f = new Vector3(a + offset, depthA);

            float depthB = state.ZPositionFor(b);
            var b_n = new Vector3(b, depthB);
            var b_f = new Vector3(b + offset + diag, depthB);

            float depthC = state.ZPositionFor(c);
            var c_n = new Vector3(c, depthC);
            var c_f = new Vector3(c + offset, depthC);

            state.Triangle(a_n, a_f, b_n, Color);
            state.Triangle(a_f, b_n, b_f, Color);
            state.Triangle(b_n, b_f, c_n, Color);
            state.Triangle(b_f, c_n, c_f, Color);
        }
    }
}
