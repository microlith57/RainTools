using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.RainTools {
    [CustomEntity("RainTools/ShadowLine")]
    public class ShadowLine : ShadowCaster {
        public float ShadowLength, Offset;
        public Color Color;
        public Vector2[] Nodes;

        public ShadowLine(EntityData data, Vector2 offset)
            : base(data.Position + offset, maxTriCount: data.Nodes.Length * 2) {

            Offset = data.Float("offset", 0f);
            ShadowLength = data.Float("length", 400f);

            var letsInLight = data.Bool("letsInLight", false);
            var alpha = data.Float("alpha", 1f);

            if (letsInLight) {
                Color = Color.White * alpha;
            } else {
                Color = Color.Black * alpha;
            }

            Nodes = data.NodesOffset(offset);
        }

        public override void UpdateVerts(DirectionalLightingRenderer state) {
            var posA = Position + Offset * state.Light;
            for (int i = 0; i < Nodes.Length; i++) {
                var posB = Nodes[i] + Offset * state.Light;
                state.Parallelogram(posA, posB, ShadowLength, Color);
                posA = posB;
            }
        }

        public override void DebugRender(Camera camera) {
            base.DebugRender(camera);

            var posA = Position;
            for (int i = 0; i < Nodes.Length; i++) {
                var posB = Nodes[i];
                Draw.Line(posA, posB, Color.Magenta);
                posA = posB;
            }
        }
    }
}
