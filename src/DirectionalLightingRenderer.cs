using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools {
    public class DirectionalLightingRenderer {
        private const int RESIZE_DOWN_BUFFER = 28 * 3;
        private const int RESIZE_UP_BUFFER = 16 * 3;

        // todo
        public Vector2 CenterPos = Vector2.Zero;
        public float CircleRad = 1000f;

        public Vector2 _light;
        public Vector2 Light {
            get => _light;
            set {
                if ((_light - value).LengthSquared() <= 0.1f)
                    InvalidateGeometry();
                _light = value;
            }
        }

        public VertexPositionColor[] verts;
        public int v;
        private bool valid;
        private int capacity_target;

        public List<ShadowCaster> Shadows;

        public DirectionalLightingRenderer(Vector2 light) {
            _light = light;

            Shadows = new();
            capacity_target = 0;
            verts = new VertexPositionColor[0];
            v = 0;
            valid = true;
        }

        public DirectionalLightingRenderer(Vector2 light, IEnumerable<ShadowCaster> shadows) {
            _light = light;

            Shadows = shadows.ToList();
            InvalidateCapacity();
        }

        public void InvalidateGeometry() {
            valid = false;
        }

        public void Add(ShadowCaster shadow) {
            Shadows.Add(shadow);
            capacity_target += shadow.MaxTriCount;
            InvalidateGeometry();
        }

        public bool Remove(ShadowCaster shadow) {
            if (Shadows.Remove(shadow)) {
                capacity_target -= shadow.MaxTriCount;
                InvalidateGeometry();
                return true;
            }
            return false;
        }

        public void InvalidateCapacity() {
            capacity_target = Shadows.Sum((shadow) => shadow.MaxTriCount) * 3;
            InvalidateGeometry();
        }

        public void Generate(bool force = false) {
            if (!force && valid)
                return;

            if (capacity_target < 0)
                capacity_target = 0;

            if (verts == null || capacity_target > verts.Length || capacity_target + RESIZE_DOWN_BUFFER < verts.Length)
                verts = new VertexPositionColor[capacity_target + RESIZE_UP_BUFFER];

            v = 0;
            foreach (var shadow in Shadows) {
                shadow.UpdateVerts(this);
            }
            valid = true;
        }

        public void Draw(Matrix mat) {
            if (verts != null && v > 0)
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
