using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.RainTools.ShadowCasters {
    [CustomEntity("RainTools/ShadowRectangle=Load",
                  "RainTools/ShadowRectangleLinearColors=LoadLinearColors")]
    public class ShadowRectangle : ShadowCaster {

        public float ShadowLength, Offset;
        public Color TopLeftColor, TopRightColor, BottomLeftColor, BottomRightColor;
        public bool Concave;

        public ShadowRectangle(Vector2 position,
                               float width, float height,
                               float shadowLength, float shadowOffset,
                               Color topLeft, Color topRight, Color bottomLeft, Color bottomRight,
                               bool concave)
            : base(position, maxTriCount: 4) {

            Collider = new Hitbox(width, height);
            Collidable = false;

            ShadowLength = shadowLength;
            Offset = shadowOffset;

            TopLeftColor = topLeft;
            TopRightColor = topRight;
            BottomLeftColor = bottomLeft;
            BottomRightColor = bottomRight;

            Concave = concave;
        }

        public ShadowRectangle(Vector2 position,
                               float width, float height,
                               float shadowLength, float shadowOffset,
                               Color color,
                               bool concave)
            : this(position, width, height, shadowLength, shadowOffset, color, color, color, color, concave) { }

        public static ShadowRectangle Load(Level level, LevelData levelData, Vector2 offset, EntityData data) {

            var shadowLength = data.Float("length");
            var shadowOffset = data.Float("offset");
            var color = data.Bool("letsInLight") ? Color.White : Color.Black;
            color *= data.Float("alpha", 1f);
            var concave = data.Bool("concave");

            return new(data.Position + offset,
                       data.Width, data.Height,
                       shadowLength, shadowOffset,
                       color,
                       concave);
        }

        public static ShadowRectangle LoadLinearColors(Level level, LevelData levelData, Vector2 offset, EntityData data) {

            var shadowLength = data.Float("length");
            var shadowOffset = data.Float("offset");

            var topLeft = Calc.HexToColor(data.Attr("topLeftColor", "000000"));
            var topRight = Calc.HexToColor(data.Attr("topRightColor", "000000"));
            var bottomLeft = Calc.HexToColor(data.Attr("bottomLeftColor", "000000"));
            var bottomRight = Calc.HexToColor(data.Attr("bottomRightColor", "000000"));
            topLeft *= data.Float("topLeftAlpha", 1f);
            topRight *= data.Float("topRightAlpha", 1f);
            bottomLeft *= data.Float("bottomLeftAlpha", 1f);
            bottomRight *= data.Float("bottomRightAlpha", 1f);

            var concave = data.Bool("concave");

            return new(data.Position + offset,
                       data.Width, data.Height,
                       shadowLength, shadowOffset,
                       topLeft, topRight, bottomLeft, bottomRight,
                       concave);
        }

        public override void UpdateVerts(DirectionalLightingRenderer state) {
            // todo check if this is right
            bool left_illuminated = Concave ? (state.Light.X <= 0) : (state.Light.X > 0);
            bool top_illuminated = Concave ? (state.Light.Y <= 0) : (state.Light.Y > 0);

            // the light touches three points, which are the corners of the two illuminated sides
            VertexVector2Color a, b, c;
            if (top_illuminated) {
                if (left_illuminated) {
                    a = new(BottomLeft + state.Light * Offset, BottomLeftColor);
                    b = new(TopLeft + state.Light * Offset, TopLeftColor);
                    c = new(TopRight + state.Light * Offset, TopRightColor);
                } else {
                    a = new(TopLeft + state.Light * Offset, TopLeftColor);
                    b = new(TopRight + state.Light * Offset, TopRightColor);
                    c = new(BottomRight + state.Light * Offset, BottomRightColor);
                }
            } else {
                if (left_illuminated) {
                    a = new(TopLeft + state.Light * Offset, TopLeftColor);
                    b = new(BottomLeft + state.Light * Offset, BottomLeftColor);
                    c = new(BottomRight + state.Light * Offset, BottomRightColor);
                } else {
                    a = new(BottomLeft + state.Light * Offset, BottomLeftColor);
                    b = new(BottomRight + state.Light * Offset, BottomRightColor);
                    c = new(TopRight + state.Light * Offset, TopRightColor);
                }
            }

            Vector2 offset = state.Light * ShadowLength;
            Vector2 diag = (a.Position - b.Position) + (c.Position - b.Position);

            // _n means the point near the light, _f means far

            float depthA = state.ZPositionFor(a.Position);
            VertexPositionColor a_n = new(new(a.Position, depthA), a.Color);
            VertexPositionColor a_f = new(new(a.Position + offset, depthA), a.Color);

            float depthB = state.ZPositionFor(b.Position);
            VertexPositionColor b_n = new(new(b.Position, depthB), b.Color);
            VertexPositionColor b_f = new(new(b.Position + offset + diag, depthB), b.Color);

            float depthC = state.ZPositionFor(c.Position);
            VertexPositionColor c_n = new(new(c.Position, depthC), c.Color);
            VertexPositionColor c_f = new(new(c.Position + offset, depthC), c.Color);

            state.Triangle(a_n, a_f, b_n);
            state.Triangle(a_f, b_n, b_f);
            state.Triangle(b_n, b_f, c_n);
            state.Triangle(b_f, c_n, c_f);
        }

    }
}
