using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.Pipes {
    [Tracked]
    public class Edge : Component, IPart {

        public Pipe Pipe { get; set; }

        public Vector2 Position = Vector2.Zero;
        public Vector2 AbsPosition => Entity.Position + Position;

        public Vector2[] Points;

        Vector2 IPart.Position => AbsPosition;
        public Vector2 EndPosition => Points[^1] + AbsPosition;

        public virtual float Offset { get; set; }
        public float Length {
            get {
                var length = Points[0].Length();

                for (int i = 1; i < Points.Length; i++)
                    length += (Points[i] - Points[i - 1]).Length();

                return length;
            }
        }

        public Edge(Vector2 relPosition, Vector2[] points) : base(false, false) {
            Position = relPosition;
            Points = points;
        }

        public override void EntityAdded(Scene scene) {
            base.EntityAdded(scene);

            var controller = Controller.AddIfAbsent(Scene as Level);
            controller.AddPart(this);
        }

        public override void DebugRender(Camera camera) {
            Color col;
            if (Pipe == null || !Pipe.Valid)
                col = Color.Red;
            else if (!Pipe.Active)
                col = Color.LightGray;
            else
                col = Color.Purple;

            Draw.Line(AbsPosition, Points[0] + AbsPosition, col * 0.5f);
            for (int i = 1; i < Points.Length; i++)
                Draw.Line(Points[i - 1] + AbsPosition, Points[i] + AbsPosition, col * 0.5f);

            Draw.HollowRect(AbsPosition - Vector2.One, 3, 3, col * 0.8f);
            Draw.HollowRect(EndPosition - Vector2.One, 3, 3, col * 0.8f);
        }

    }
}