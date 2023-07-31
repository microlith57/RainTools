using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/StylegroundTimeController")]
    public class StylegroundTimeController : Entity {
        public string StylegroundTag;
        public CircularColorLerper Colors;
        public CircularFloatLerper Alphas;

        private EntityData _data;
        private Vector2 _offset;

        public StylegroundTimeController(string tag) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            StylegroundTag = tag;
            Colors = new();
            Alphas = new();
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

            if (data.Attr("color").Length > 0)
                Colors.Stops[angle] = Calc.HexToColorWithAlpha(data.Attr("color"));

            if (data.Float("alpha", -1) >= 0)
                Alphas.Stops[angle] = data.Float("alpha");
        }

        public override void Update() {
            base.Update();

            float sunAngle = RainToolsModule.Session.SunAngle;
            Color color = Colors.GetOrDefault(sunAngle);

            if (Alphas.Any) {
                color *= Alphas.GetOrDefault(sunAngle);

                foreach (var bg in (Scene as Level).Background.GetEach<Backdrop>(StylegroundTag))
                    bg.Color = color;

                foreach (var fg in (Scene as Level).Foreground.GetEach<Backdrop>(StylegroundTag))
                    fg.Color = color;
            } else {
                foreach (var bg in (Scene as Level).Background.GetEach<Backdrop>(StylegroundTag))
                    bg.Color = color * (bg.Color.A / 255f);

                foreach (var fg in (Scene as Level).Foreground.GetEach<Backdrop>(StylegroundTag))
                    fg.Color = color * (fg.Color.A / 255f);
            }
        }
    }
}
