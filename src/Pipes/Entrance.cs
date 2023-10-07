using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.Pipes {
    [CustomEntity("RainTools/PipeEntrance")]
    [Tracked(true)]
    public class Entrance : Endpoint {

        public enum Directions {
            Left, Right, Up, Down
        }

        public Directions Direction;

        public Entrance(EntityData data, Vector2 offset) : base(data, offset) {
            Direction = data.Enum<Directions>("direction");
        }

        public override void Render() {
            base.Render();
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
