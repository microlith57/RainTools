using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.Pipes {
    [CustomEntity("RainTools/PipeSegment")]
    public class Segment(EntityData data, Vector2 offset) : Entity(data.Position + offset), IPart {

        public Pipe Pipe { get; set; }

        public Vector2[] Points = data.NodesOffset(-data.Position);
        public Vector2 EndPosition => Points[^1] + Position;

        public float Length {
            get {
                var length = Points[0].Length();

                for (int i = 1; i < Points.Length; i++)
                    length += (Points[i] - Points[i - 1]).Length();

                return length;
            }
        }

        public float StartDistance;
        public float EndDistance;

        public override void Added(Scene scene) {
            base.Added(scene);

            var level = Scene as Level;
            var controller = Controller.AddIfAbsent(level);

            controller.Add(this);
        }

        public override void DebugRender(Camera camera) {
            base.DebugRender(camera);

            Color col;
            if (Pipe == null || !Pipe.Valid)
                col = Color.Red;
            else if (!Pipe.Active)
                col = Color.LightGray;
            else
                col = Color.Purple;

            for (int i = 1; i < Points.Length; i++)
                Draw.Line(Points[i - 1] + Position, Points[i] + Position, col * 0.5f);

            Draw.HollowRect(Position - Vector2.One, 3, 3, col * 0.8f);
            Draw.HollowRect(EndPosition - Vector2.One, 3, 3, col * 0.8f);
        }

    }
}