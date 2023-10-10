using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.Pipes {
    [CustomEntity("RainTools/PipeSegment")]
    [Tracked(true)]
    public class Segment : Entity {

        public Edge Edge;

        public Segment(EntityData data, Vector2 offset) : base(data.Position + offset) {
            var points = data.NodesOffset(-data.Position);

            Add(Edge = new(Vector2.Zero, points));
        }

        public override void Render() {
            base.Render();
        }

    }
}