using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public override void UpdateVerts(ref VertexPositionColor[] verts, ref int v, Vector2 lightAngle, Vector2 pos, float radius) {
            Vector2 a, b, c;
            if (lightAngle.Y >= 0) {
                if (lightAngle.X >= 0) {
                    a = BottomLeft + lightAngle * Offset;
                    b = TopLeft + lightAngle * Offset;
                    c = TopRight + lightAngle * Offset;
                } else {
                    a = TopLeft + lightAngle * Offset;
                    b = TopRight + lightAngle * Offset;
                    c = BottomRight + lightAngle * Offset;
                }
            } else {
                if (lightAngle.X >= 0) {
                    a = TopLeft + lightAngle * Offset;
                    b = BottomLeft + lightAngle * Offset;
                    c = BottomRight + lightAngle * Offset;
                } else {
                    a = BottomLeft + lightAngle * Offset;
                    b = BottomRight + lightAngle * Offset;
                    c = TopRight + lightAngle * Offset;
                }
            }
            Vector2 offsetE = lightAngle * ShadowLength;
            Vector2 diag = (a - b) + (c - b);
            Vector2 offsetM = offsetE + diag;

            float depthA = DepthForPoint(lightAngle, pos, radius, a);
            float depthB = DepthForPoint(lightAngle, pos, radius, b);
            float depthC = DepthForPoint(lightAngle, pos, radius, c);

            verts[v].Position = new Vector3(a, depthA);           verts[v++].Color = Color;
            verts[v].Position = new Vector3(a + offsetE, depthA); verts[v++].Color = Color;
            verts[v].Position = new Vector3(b, depthB);           verts[v++].Color = Color;

            verts[v].Position = new Vector3(a + offsetE, depthA); verts[v++].Color = Color;
            verts[v].Position = new Vector3(b, depthB);           verts[v++].Color = Color;
            verts[v].Position = new Vector3(b + offsetM, depthB); verts[v++].Color = Color;

            verts[v].Position = new Vector3(b, depthB);           verts[v++].Color = Color;
            verts[v].Position = new Vector3(b + offsetM, depthB); verts[v++].Color = Color;
            verts[v].Position = new Vector3(c, depthC);           verts[v++].Color = Color;

            verts[v].Position = new Vector3(b + offsetM, depthB); verts[v++].Color = Color;
            verts[v].Position = new Vector3(c, depthC);           verts[v++].Color = Color;
            verts[v].Position = new Vector3(c + offsetE, depthC); verts[v++].Color = Color;
        }
    }
}
