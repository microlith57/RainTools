using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools {
    public class DirectionalLightingRenderer {

        // todo
        public readonly Vector2 CenterPos = Vector2.Zero;
        public readonly float CircleRad = 1000f;

        public Vector2 Light;
        private Vector2? prevLight;

        public VertexPositionColor[] verts;
        public int v;

        private List<ShadowCaster> Shadows;

        public DirectionalLightingRenderer(Vector2 light, IEnumerable<ShadowCaster> shadows) {
            Light = light;
            prevLight = null;
            // CenterPos = pos;

            Shadows = shadows.ToList();
            var capacity = Shadows.Sum((shadow) => shadow.MaxTriCount) * 3;

            verts = new VertexPositionColor[capacity];
            v = 0;
        }

        public bool ShouldRegen() {
            return !prevLight.HasValue
                || ((Light.Angle() - prevLight.Value.Angle()) % (2f * Math.PI)) > 0.01f
                || (Light.Length() - prevLight.Value.Length()) > 0.01f;
        }

        public void RegenGeometry(bool force = false) {
            if (!force && !ShouldRegen())
                return;

            v = 0;
            foreach (var shadow in Shadows) {
                shadow.UpdateVerts(this);
            }
            prevLight = Light;
        }

        public void Draw(Matrix mat) {
            if (v > 0)
                GFX.DrawVertices(mat, verts, v);
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
            return 0.5f + 0.1f * Vector2.Dot(point - (CenterPos - axis), 2 * axis) / (CircleRad * 2 * CircleRad * 2);
        }
    }
}