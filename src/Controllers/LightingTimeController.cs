using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    public class StylegroundTimeController : Entity {
        public struct Keyframe {
            public Color Color;
            public float Alpha;
        }

        public string StylegroundTag;
        public SortedList<float, Keyframe> Keyframes;

        public StylegroundTimeController(string tag) : base() {
            Tag |= Tags.Persistent | Tags.TransitionUpdate | Tags.FrozenUpdate;

            StylegroundTag = tag;
            Keyframes = new();
        }

        public StylegroundTimeController Load(Level level, LevelData levelData, Vector2 offset, EntityData data) {
            var tag = data.Attr("tag");

            var existing = level.Tracker.GetEntities<StylegroundTimeController>()
                                        .Cast<StylegroundTimeController>()
                                        .Where((c) => c.StylegroundTag == tag);

            if (existing.Any()) {
                existing.First().AddStop(data, offset);
                return null;
            }

            StylegroundTimeController controller = new(tag);
            controller.AddStop(data, offset);
            return controller;
        }

        public void AddStop(EntityData data, Vector2 offset) {
            Vector2 pos = data.Position + offset;
            Vector2 nodePos = data.NodesOffset(offset)[0];
            var angle = (pos - nodePos).Angle();

            Keyframe kf = new();

            kf.Color = Calc.HexToColorWithAlpha(data.Attr("color"));
            kf.Alpha = data.Float("alpha", 1);

            Keyframes.Add(angle, kf);
        }

        public override void Update() {
            base.Update();

            float time = 0f;

            var closest = Keyframes.OrderBy((kf) => {
                var dist = Math.Abs(kf.Key - time);
                if (dist > Math.PI)
                    dist -= (float) Math.PI;
                return dist;
            });

            foreach (var bg in (Scene as Level).Background.GetEach<Backdrop>(StylegroundTag)) {
            }

            foreach (var fg in (Scene as Level).Foreground.GetEach<Backdrop>(StylegroundTag)) {
            }
        }
    }
}
