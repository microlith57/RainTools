using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.Pipes {
    [Tracked]
    public class Endpoint : Component, IPart {

        public Pipe Pipe { get; set; }

        public Vector2 Position = Vector2.Zero;

        Vector2 IPart.Position => Entity.Position + Position;
        public Vector2 EndPosition => Entity.Position + Position;

        public virtual float Offset { get; set; }
        public virtual float Length => 0f;

        public Endpoint(Vector2 relPosition) : base(false, false) {
            Position = relPosition;
        }

        public override void EntityAdded(Scene scene) {
            base.EntityAdded(scene);

            var controller = Controller.AddIfAbsent(scene as Level);
            controller.AddPart(this);
        }

        public override void DebugRender(Camera camera) {
            base.DebugRender(camera);

            Color col;
            if (Pipe == null || !Pipe.Valid)
                col = (Engine.FrameCounter / 10 % 2 == 0) ? Color.Red : Color.DarkRed;
            else if (!Pipe.Active)
                col = Color.LightGray;
            else
                col = Color.Purple;

            Draw.Rect(Position - Vector2.One, 3, 3, col);
        }

    }
}
