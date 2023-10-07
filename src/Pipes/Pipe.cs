using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Celeste.Mod.RainTools.Pipes {
    [Tracked]
    public class Pipe : Component {

        // public struct Pulse {
        //     public float Distance;
        //     public readonly float Velocity;
        //     public readonly Color Color;
        // }

        // public struct Flash {
        //     public float Factor;
        //     public readonly Color Color;
        //     public readonly float Duration;
        // }

        public List<IPart> Parts = new();

        public Endpoint Start => Parts.Count > 0 ? (Parts[0] as Endpoint) : null;
        public Endpoint End => Parts.Count > 0 ? (Parts[^1] as Endpoint) : null;

        public bool Valid => Start != null && End != null;

        // private List<Flash> flashes = new();
        // public ReadOnlyCollection<Flash> Flashes => flashes.AsReadOnly();
        // private List<Pulse> pulses = new();
        // public ReadOnlyCollection<Pulse> Pulses => pulses.AsReadOnly();

        public float TotalLength = 0f;

        public Pipe() : base(false, false) { }

        public Pipe(IPart part) : this() => Add(part, false);

        public Pipe(List<IPart> parts) : this() {
            foreach (var part in parts)
                Add(part, false);
        }

        public void Add(IPart part, bool addToEnd) {
            part.Pipe = this;

            if (addToEnd)
                Parts.Add(part);
            else
                Parts.Insert(0, part);
        }

        public void AddFrom(Pipe pipe, bool addToEnd, bool takeFromEnd) {
            var source = pipe.Parts.AsEnumerable();
            if (takeFromEnd)
                source = source.Reverse();

            foreach (var part in source)
                Add(part, addToEnd);
        }

        public override void EntityAwake() {
            float pos = 0f;

            foreach (var part in Parts) {
                part.Offset = pos;
                pos += part.Length;
            }
        }

        // public override void Update() {
        //     flashes.RemoveAll(flash => {
        //         flash.Factor = Calc.Approach(flash.Factor, 0f, Engine.DeltaTime / flash.Duration);
        //         return flash.Factor <= 0f;
        //     });
        //     pulses.RemoveAll(pulse => {
        //         pulse.Distance += pulse.Velocity * Engine.DeltaTime;
        //         return pulse.Distance > 0f && pulse.Distance < TotalLength;
        //     });

        //     if (pulses.Count == 0 && flashes.Count == 0)
        //         Active = false;
        // }

        // public void AddPulse(Pulse pulse) {
        //     pulses.Add(pulse);
        //     Active = true;
        // }

        // public void AddFlash(Flash flash) {
        //     flashes.Add(flash);
        //     Active = true;
        // }

        // public Color ColorAt(float distance) {
        //     // todo
        //     return Color.Black;
        // }

    }
}