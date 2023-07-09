using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.RainTools {
    public static class ShadowRenderer {
        public class State {
            public readonly Vector2 CirclePos;
            public readonly float CircleRad;

            public Vector2 Light;

            public VertexPositionColor[] verts;
            public int v;

            public State(int capacity, Vector2 light, Vector2 pos, float radius) {
                Light = light;
                CirclePos = pos;
                CircleRad = radius;

                verts = new VertexPositionColor[capacity];
                v = 0;
            }

            public void Triangle(Vector3 a, Vector3 b, Vector3 c, Color col) {
                verts[v].Position = a;
                verts[v++].Color = col;

                verts[v].Position = b;
                verts[v++].Color = col;

                verts[v].Position = c;
                verts[v++].Color = col;
            }

            public void Parallelogram(Vector2 a, Vector2 b, float length, Color col) {
                float depthA = ZPositionFor(a);
                float depthB = ZPositionFor(b);

                Vector3 pos1 = new Vector3(a, depthA);
                Vector3 pos2 = new Vector3(b, depthB);
                Vector3 pos3 = new Vector3(a + Light * length, depthA);
                Vector3 pos4 = new Vector3(b + Light * length, depthB);

                Triangle(pos1, pos2, pos3, col);
                Triangle(pos2, pos3, pos4, col);
            }

            public float ZPositionFor(Vector2 point) {
                Vector2 axis = Light * CircleRad;
                // todo: make this work even for distant shadows (w/ sigmoid?)
                return 0.5f + 0.1f * Vector2.Dot(point - (CirclePos - axis), 2 * axis) / (CircleRad * 2 * CircleRad * 2);
            }
        }
    }
}