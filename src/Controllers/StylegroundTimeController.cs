using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/StylegroundTimeController")]
    public class StylegroundTimeController : Entity {
        public string SearchTag;
        public CircularColorLerper Colors;
        public CircularFloatLerper Alphas;

        private EntityData _data;
        private Vector2 _offset;

        public StylegroundTimeController(string tag) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            SearchTag = tag;
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
                                        .Where((c) => c.SearchTag == SearchTag);

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

            var change = data.Enum<ColorAlphaChangeMode>("mode", ColorAlphaChangeMode.Both);

            if (change != ColorAlphaChangeMode.AlphaOnly
                && data.Attr("color").Length > 0) {

                Colors.Stops[angle] = Calc.HexToColorWithAlpha(data.Attr("color"));
            }

            if (change != ColorAlphaChangeMode.ColorOnly
                && data.Float("alpha", -1) >= 0) {

                Alphas.Stops[angle] = data.Float("alpha");
            }
        }

        public override void Update() {
            base.Update();

            float sunAngle = RainToolsModule.Session.SunAngle;
            Color color = Colors.GetOrDefault(sunAngle);
            float alpha = Alphas.GetOrDefault(sunAngle);

            bool changeColor = Colors.Any;
            bool changeAlpha = Alphas.Any;

            var fgs = (Scene as Level).Foreground.GetEach<Backdrop>(SearchTag);
            foreach (var fg in fgs) {
                if (changeColor)
                    fg.Color = color;
                if (changeAlpha)
                    fg.FadeAlphaMultiplier = alpha;
            }

            var bgs = (Scene as Level).Background.GetEach<Backdrop>(SearchTag);
            foreach (var bg in bgs) {
                if (changeColor)
                    bg.Color = color;
                if (changeAlpha)
                    bg.FadeAlphaMultiplier = alpha;
            }
        }
    }
}
