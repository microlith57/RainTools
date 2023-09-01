using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Celeste.Mod.RainTools.Pipes {
    public class Pipe : Component {
        public struct Pulse {
            public float Distance;
            public readonly float Velocity;
            public readonly Color Color;
        }

        public struct Flash {
            public float Factor;
            public readonly Color Color;
            public readonly float Duration;
        }

        public Endpoint Start;
        public List<Segment> Segments;
        public Endpoint End;
        public bool Valid => Start != null && End != null && Segments.Count > 0;

        private List<Flash> flashes;
        public ReadOnlyCollection<Flash> Flashes => flashes.AsReadOnly();
        private List<Pulse> pulses;
        public ReadOnlyCollection<Pulse> Pulses => pulses.AsReadOnly();

        public float TotalLength = 0f;

        public Pipe() : base(false, false) { }

        public override void Update() {
            flashes.RemoveAll(flash => {
                flash.Factor = Calc.Approach(flash.Factor, 0f, Engine.DeltaTime / flash.Duration);
                return flash.Factor <= 0f;
            });
            pulses.RemoveAll(pulse => {
                pulse.Distance += pulse.Velocity * Engine.DeltaTime;
                return pulse.Distance > 0f && pulse.Distance < TotalLength;
            });

            if (pulses.Count == 0 && flashes.Count == 0)
                Active = false;
        }

        public void AddPulse(Pulse pulse) {
            pulses.Add(pulse);
            Active = true;
        }

        public void AddFlash(Flash flash) {
            flashes.Add(flash);
            Active = true;
        }

        public Color ColorAt(float distance) {
            // todo
            return Color.Black;
        }
    }
}