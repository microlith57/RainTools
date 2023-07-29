using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/ColorgradeTimeController")]
    public class ColorgradeTimeController : Entity {
        public SortedList<float, string> Keyframes;

        private EntityData _data;
        private Vector2 _offset;

        public ColorgradeTimeController() : base() {
            Tag |= Tags.Persistent | Tags.TransitionUpdate | Tags.FrozenUpdate;

            Keyframes = new();
        }

        public ColorgradeTimeController(EntityData data, Vector2 offset) : this() {
            _data = data;
            _offset = offset;
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<ColorgradeTimeController>()
                                        .Cast<ColorgradeTimeController>()
                                        .ToList();

            if (existing.Any((c) => c != this)) {
                existing.First().AddStop(_data, _offset);
                RemoveSelf();
                return;
            }

            AddStop(_data, _offset);
            _data = null;
        }

        public void AddStop(EntityData data, Vector2 offset) {
            Vector2 pos = data.Position + offset;
            Vector2 nodePos = data.NodesOffset(offset)[0];
            var angle = (nodePos - pos).Angle();

            Keyframes.Add(angle, data.Attr("colorgrade", "none"));
        }

        public override void Update() {
            base.Update();

            float sunAngle = RainToolsModule.Session.SunAngle;
            var closest = Keyframes.OrderBy((kvp) => Calc.AbsAngleDiff(kvp.Key, sunAngle));

            string colorgrade_a;
            string colorgrade_b;
            float fac;

            switch (closest.Count()) {
                case 0:
                    throw new NotImplementedException("unreachable code reached!");
                case 1:
                    var kvp = closest.First();

                    colorgrade_a = kvp.Value;
                    colorgrade_b = kvp.Value;

                    fac = 0f;
                    break;
                default:
                    var a = closest.ElementAt(0);
                    var b = closest.ElementAt(1);

                    colorgrade_a = a.Value;
                    colorgrade_b = b.Value;

                    float dist_a_sun = Calc.AbsAngleDiff(a.Key, sunAngle);
                    float dist_b_sun = Calc.AbsAngleDiff(b.Key, sunAngle);
                    fac = Calc.Clamp((dist_a_sun) / (dist_a_sun + dist_b_sun), 0f, 1f);

                    break;
            }

            Level level = Scene as Level;

            level.lastColorGrade = colorgrade_a;
            level.Session.ColorGrade = colorgrade_b;
            level.colorGradeEase = fac;
        }
    }
}
