using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools.Pipes {
    [Tracked(true)]
    public abstract class Endpoint : Entity, IPart {

        public Endpoint(EntityData data, Vector2 offset) : base(data.Position + offset) { }

        public Pipe Pipe { get; set; }

        Vector2 IPart.Position => Position;
        public Vector2 EndPosition => Position;

        public virtual float Offset { get; set; }
        public virtual float Length => 0f;

        public override void Awake(Scene scene) {
            base.Awake(scene);

            var level = Scene as Level;
            var controller = Controller.AddIfAbsent(level);

            controller.Add(this);
        }

    }
}
