using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools.Pipes {
    [Tracked]
    public class Pipe : Component {

        public class Vessel {
            public float Offset;
            public float Velocity;
            public float Length;
            public Color Color;

            public Vessel(float offset, float velocity, float length, Color color) {
                Offset = offset;
                Velocity = velocity;
                Length = length;
                Color = color;
            }

            public Tuple<float, float> Endpoints {
                get {
                    if (Velocity >= 0) {
                        return new(Offset - Length, Offset);
                    } else {
                        return new(Offset, Offset + Length);
                    }
                }
            }

            public bool Arrived(float pipeLength) {
                if (Velocity >= 0) {
                    return Offset >= pipeLength;
                } else {
                    return Offset <= 0;
                }
            }

            public bool Touches(float point) {
                var endpoints = Endpoints;
                return point >= endpoints.Item1 && point <= endpoints.Item2;
            }

            public void Update() {
                Offset += Velocity * Engine.DeltaTime;
            }
        }

        // public struct Flash {
        //     public float Factor;
        //     public readonly Color Color;
        //     public readonly float Duration;
        // }

        public List<IPart> Parts = new();

        public Endpoint Start => Parts.Count > 0 ? (Parts[0] as Endpoint) : null;
        public Endpoint End => Parts.Count > 0 ? (Parts[^1] as Endpoint) : null;

        public bool Valid => Start != null && End != null && Start != End;

        private List<Vessel> vessels = new();
        // private List<Flash> flashes = new();

        public float TotalLength = 0f;

        public Pipe() : base(true, false) { }

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
            TotalLength = 0f;

            foreach (var part in Parts) {
                part.Offset = TotalLength;
                TotalLength += part.Length;
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

        public void Add(Vessel pulse) {
            vessels.Add(pulse);
            // Active = true;
        }

        // public void AddFlash(Flash flash) {
        //     flashes.Add(flash);
        //     Active = true;
        // }

        // public Color ColorAt(float offset) {
        //     // todo
        //     return Color.Black;
        // }

        public override void Update() {
            base.Update();

            vessels.RemoveAll((vessel) => {
                vessel.Update();

                bool arrived = vessel.Arrived(TotalLength);

                if (arrived) {
                    if (vessel.Touches(0f)) {
                        Start.VesselArrived(vessel);
                    } else {
                        End.VesselArrived(vessel);
                    }
                }

                return arrived;
            });
        }

    }
}