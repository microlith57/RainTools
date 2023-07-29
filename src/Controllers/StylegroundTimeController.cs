using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMod.Utils;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/StylegroundTimeController")]
    public class StylegroundTimeController : Entity {
        public struct Keyframe {
            public Color Color;
            public float Alpha;
        }

        public string StylegroundTag;
        public SortedList<float, Keyframe> Keyframes;

        private EntityData _data;
        private Vector2 _offset;

        public StylegroundTimeController(string tag) : base() {
            Tag |= Tags.Persistent | Tags.TransitionUpdate | Tags.FrozenUpdate;

            StylegroundTag = tag;
            Keyframes = new();
        }

        public StylegroundTimeController(EntityData data, Vector2 offset) : this(data.Attr("tag")) {
            _data = data;
            _offset = offset;
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<StylegroundTimeController>()
                                        .Cast<StylegroundTimeController>()
                                        .Where((c) => c.StylegroundTag == StylegroundTag);

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

            Keyframe kf = new();

            kf.Color = Calc.HexToColorWithAlpha(data.Attr("color"));
            kf.Alpha = data.Float("alpha", 1);

            Keyframes.Add(angle, kf);
        }

        public override void Update() {
            base.Update();

            float sunAngle = RainToolsModule.Session.SunAngle;

            Color color;
            float alpha;

            switch (Keyframes.Count()) {
                case 0:
                    throw new NotImplementedException("unreachable code reached!");
                case 1:
                    var kvp = Keyframes.First();
                    color = kvp.Value.Color;
                    alpha = kvp.Value.Alpha;
                    break;
                default:
                    var closest = Keyframes.OrderBy((kvp) => Calc.AbsAngleDiff(kvp.Key, sunAngle));

                    var a = closest.ElementAt(0);
                    var b = closest.ElementAt(1);

                    float dist_a_sun = Calc.AbsAngleDiff(a.Key, sunAngle);
                    float dist_b_sun = Calc.AbsAngleDiff(b.Key, sunAngle);
                    float fac = Calc.Clamp((dist_a_sun) / (dist_a_sun + dist_b_sun), 0f, 1f);

                    color = Color.Lerp(a.Value.Color, b.Value.Color, fac);
                    alpha = MathHelper.Lerp(a.Value.Alpha, b.Value.Alpha, fac);

                    break;
            }

            foreach (var bg in (Scene as Level).Background.GetEach<Backdrop>(StylegroundTag)) {
                bg.Color = color;
                DynamicData.For(bg).Set("Alpha", alpha);
            }

            foreach (var fg in (Scene as Level).Foreground.GetEach<Backdrop>(StylegroundTag)) {
                fg.Color = color;
                DynamicData.For(fg).Set("Alpha", alpha);
            }
        }
    }
}
