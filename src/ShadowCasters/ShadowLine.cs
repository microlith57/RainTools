using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.ShadowCasters {
    [CustomEntity("RainTools/ShadowLine=Load",
                  "RainTools/ShadowLineLinearColors=LoadLinearColors")]
    public class ShadowLine : ShadowCaster {

        public VertexVector2Color[] Vertices;
        public float ShadowLength, Offset;

        public ShadowLine(Vector2 position, VertexVector2Color[] vertices, float shadowLength, float shadowOffset)
            : base(position, vertices.Length * 2) {

            Vertices = vertices;
            ShadowLength = shadowLength;
            Offset = shadowOffset;
        }

        public static ShadowLine Load(Level level, LevelData levelData, Vector2 offset, EntityData data) {
            var position = data.Position + offset;

            var shadowOffset = data.Float("offset", 0f);
            var shadowLength = data.Float("length", 400f);

            var color = data.Bool("letsInLight") ? Color.White : Color.Black;
            color *= data.Float("alpha", 1f);

            var verts = new VertexVector2Color[data.Nodes.Length + 1];

            verts[0] = new(position, color);
            for (int i = 0; i < data.Nodes.Length; i++)
                verts[i + 1] = new(data.Nodes[i] + offset, color);

            return new(position, verts, shadowLength, shadowOffset);
        }

        public static ShadowLine LoadLinearColors(Level level, LevelData levelData, Vector2 offset, EntityData data) {
            var position = data.Position + offset;
            var nodes = data.NodesOffset(offset);

            var shadowOffset = data.Float("offset", 0f);
            var shadowLength = data.Float("length", 400f);

            var colorA = Calc.HexToColor(data.Attr("colorA", "000000"));
            colorA *= data.Float("alphaA", 1f);
            var colorB = Calc.HexToColor(data.Attr("colorB", "000000"));
            colorB *= data.Float("alphaB", 1f);

            float totalLength = 0f;
            var prev = position;
            foreach (var node in nodes) {
                totalLength += (node - prev).Length();
                prev = node;
            }

            var verts = new VertexVector2Color[data.Nodes.Length + 1];

            verts[0] = new(position, colorA);

            float length = 0f;
            prev = position;
            for (int i = 0; i < nodes.Length; i++) {
                length += (nodes[i] - prev).Length();
                var color = Color.Lerp(colorA, colorB, length / totalLength);

                verts[i + 1] = new(nodes[i], color);
                prev = nodes[i];
            }

            return new(position, verts, shadowLength, shadowOffset);
        }

        public override void UpdateVerts(DirectionalLightingRenderer state) {
            for (int i = 1; i < Vertices.Length; i++) {

                state.Parallelogram(Vertices[i - 1] + Offset * state.Light,
                                    Vertices[i] + Offset * state.Light,
                                    ShadowLength);
            }
        }

        public override void DebugRender(Camera camera) {
            base.DebugRender(camera);

            for (int i = 1; i < Vertices.Length; i++) {

                Draw.Line(Vertices[i - 1].Position,
                          Vertices[i].Position,
                          Color.Magenta);
            }
        }

    }
}
