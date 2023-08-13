using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/StylegroundCycleController")]
    public class StylegroundCycleController : Entity {

        public string CycleTag;
        public string StyleTag;
        public string Flag = "";

        public CircularColorInterpolator Colors;
        public CircularFloatInterpolator Alphas;

        public ColorRGBAAlphaChangeMode ChangeMode;

        private EntityData _data;
        private Vector2 _offset;

        public StylegroundCycleController(string cycleTag, string styleTag, ColorRGBAAlphaChangeMode changeMode) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            CycleTag = cycleTag;
            StyleTag = styleTag;

            Colors = new();
            Alphas = new();

            ChangeMode = changeMode;
        }

        public StylegroundCycleController(EntityData data, Vector2 offset)
            : this(data.Attr("cycleTag"),
                   data.Attr("styleTag"),
                   data.Enum("mode", ColorRGBAAlphaChangeMode.ColorAndAlpha)) {

            _data = data;
            _offset = offset;
            Flag = data.Attr("flag");
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<StylegroundCycleController>()
                                        .Cast<StylegroundCycleController>()
                                        .Where((c) => c.StyleTag == StyleTag);

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

            if (ChangeMode != ColorRGBAAlphaChangeMode.AlphaOnly) {
                Colors.Add(angle, Calc.HexToColorWithAlpha(data.Attr("color")), data.Attr("colorEase"));
            }

            if (ChangeMode != ColorRGBAAlphaChangeMode.ColorOnly
                && ChangeMode != ColorRGBAAlphaChangeMode.ColorTimesPrevA) {

                Alphas.Add(angle, data.Float("alpha"), data.Attr("alphaEase"));
            }
        }

        public override void Update() {
            base.Update();

            float angle = Cycles.GetAngle(CycleTag);
            Color color = Colors.GetOrDefault(angle);
            float alpha = Alphas.GetOrDefault(angle);

            Triggers.StylegroundFade.Apply(Scene as Level, StyleTag, color, alpha, ChangeMode);
        }

    }
}
