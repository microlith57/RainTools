using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.RainTools.Pipes {
    [Tracked]
    public class Endpoint : Component, IPart {

        public Pipe Pipe { get; set; }

        public Vector2 Position = Vector2.Zero;
        public Vector2 AbsPosition => Entity.Position + Position;

        Vector2 IPart.Position => AbsPosition;
        public Vector2 EndPosition => AbsPosition;

        public virtual float Offset { get; set; }
        public virtual float Length => 0f;

        public Action<Pipe.Vessel> OnVesselArrival;

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

        public void AddVessel(float velocity, float length, Color color) {
            Pipe.Add(new Pipe.Vessel(Offset, Offset > 0 ? -velocity : velocity, length, color));
        }

        public void VesselArrived(Pipe.Vessel vessel) {
            OnVesselArrival?.Invoke(vessel);
        }

    }
}
