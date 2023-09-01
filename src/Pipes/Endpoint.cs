using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.Pipes {
    [CustomEntity("RainTools/PipeEndpoint")]
    public abstract class Endpoint(EntityData data, Vector2 offset) : Entity(data.Position + offset), IPart {

        public Pipe Pipe { get; set; }
        public Vector2 EndPosition => Position;

        public virtual float Length => 0f;

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
                col = (Engine.FrameCounter / 10 % 2 == 0) ? Color.Red : Color.DarkRed;
            else if (!Pipe.Active)
                col = Color.LightGray;
            else
                col = Color.Purple;

            Draw.Rect(Position - Vector2.One, 3, 3, col);
        }
    }
}
