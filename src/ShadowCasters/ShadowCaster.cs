using System.Numerics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    public abstract class ShadowCaster : Entity {
        public readonly int MaxTriCount;

        public ShadowCaster(Vector2 position, int maxTriCount) : base(position) {
            MaxTriCount = maxTriCount;
        }

        public abstract void UpdateVerts(ref VertexPositionColor[] verts, ref int v, Vector2 lightAngle, Vector2 pos, float radius);

        public static void Parallelogram(ref VertexPositionColor[] verts, ref int v, Vector2 lightAngle, Vector2 pos, float radius, Vector2 a, Vector2 b, float length, Color col) {
            float depthA = DepthForPoint(lightAngle, pos, radius, a);
            float depthB = DepthForPoint(lightAngle, pos, radius, b);

            Vector3 pos1 = new Vector3(a, depthA);
            Vector3 pos2 = new Vector3(b, depthB);
            Vector3 pos3 = new Vector3(a + lightAngle * length, depthA);
            Vector3 pos4 = new Vector3(b + lightAngle * length, depthB);

            verts[v].Position = pos1; verts[v++].Color = col;
            verts[v].Position = pos2; verts[v++].Color = col;
            verts[v].Position = pos3; verts[v++].Color = col;
            verts[v].Position = pos2; verts[v++].Color = col;
            verts[v].Position = pos3; verts[v++].Color = col;
            verts[v].Position = pos4; verts[v++].Color = col;
        }

        public static float DepthForPoint(Vector2 lightAngle, Vector2 pos, float radius, Vector2 point) {
            Vector2 axis = lightAngle * radius;
            return 0.5f + 0.1f * Vector2.Dot(point - (pos - axis), 2 * axis) / (radius * 2 * radius * 2);
        }
    }
}
