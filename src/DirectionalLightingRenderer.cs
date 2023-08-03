using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
                Array.Resize(ref verts, capacity_target + RESIZE_UP_BUFFER);

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

        public void Triangle(VertexPositionColor a, VertexPositionColor b, VertexPositionColor c) {
            verts[v++] = a;
            verts[v++] = b;
            verts[v++] = c;
        }

        public void Triangle(Vector3 a, Vector3 b, Vector3 c, Color col) {
            verts[v].Position = a;
            verts[v++].Color = col;

            verts[v].Position = b;
            verts[v++].Color = col;

            verts[v].Position = c;
            verts[v++].Color = col;
        }

        public void Parallelogram(VertexVector2Color a, VertexVector2Color b, float length) {
            float depthA = ZPositionFor(a.Position);
            float depthB = ZPositionFor(b.Position);

            VertexPositionColor a1 = new(new(a.Position, depthA), a.Color);
            VertexPositionColor a2 = new(new(a.Position + Light * length, depthA), a.Color);
            VertexPositionColor b1 = new(new(b.Position, depthB), b.Color);
            VertexPositionColor b2 = new(new(b.Position + Light * length, depthB), b.Color);

            Triangle(a1, b1, a2);
            Triangle(b1, a2, b2);
        }

        public float ZPositionFor(Vector2 point) {
            // todo fix
            Vector2 axis = Light * CircleRad;
            // todo: make this work even for distant shadows (w/ sigmoid?)
            return 0.5f + 0.1f * Vector2.Dot(point - (CenterPos - axis), 2 * axis) / (CircleRad * 2 * CircleRad * 2);
        }
    }

    public struct VertexVector2Color {
        public Vector2 Position;
        public Color Color;

        public VertexVector2Color(Vector2 position, Color color) {
            Position = position;
            Color = color;
        }

        public static VertexVector2Color operator +(VertexVector2Color vertex, Vector2 offset) => new(vertex.Position + offset, vertex.Color);
        public static VertexVector2Color operator -(VertexVector2Color vertex, Vector2 offset) => new(vertex.Position - offset, vertex.Color);
    }
}
