using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/AltColorgradeCycleController")]
    public class AltColorgradeCycleController : Entity {
        public CircularColorgradeInterpolator Colorgrades;
        public CircularFloatInterpolator Alphas;

        public string CycleTag;
        public string StyleTag;
        public float Alpha;

        private EntityData _data;
        private Vector2 _offset;

        public AltColorgradeCycleController(string cycleTag, string styleTag) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            Colorgrades = new();
            Alphas = new();

            CycleTag = cycleTag;
            StyleTag = styleTag;
        }

        public AltColorgradeCycleController(EntityData data, Vector2 offset)
            : this(data.Attr("cycleTag"),
                   data.Attr("styleTag")) {

            _data = data;
            _offset = offset;
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<AltColorgradeCycleController>()
                                        .Cast<AltColorgradeCycleController>()
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

            var change = data.Enum<ColorgradeAlphaChangeMode>("mode", ColorgradeAlphaChangeMode.Both);

            if (change != ColorgradeAlphaChangeMode.AlphaOnly
                && data.Attr("colorgrade") != "") {

                Colorgrades.Add(angle, data.Attr("colorgrade", "none"), data.Attr("colorgradeEase"));
            }

            if (change != ColorgradeAlphaChangeMode.ColorgradeOnly
                && data.Float("alpha", -1f) >= 0) {

                Alphas.Add(angle, data.Float("alpha", 1f), data.Attr("alphaEase"));
            }
        }

        public override void Update() {
            base.Update();

            var fgs = (Scene as Level).Foreground.GetEach<AltColorgrade>(StyleTag)
                                                 .Cast<AltColorgrade>();
            if (!fgs.Any())
                return;

            float angle = Cycles.GetAngle(CycleTag);
            var blend = Colorgrades.GetOrDefault(angle);
            var alpha = Alphas.GetOrDefault(angle);

            foreach (var backdrop in fgs) {
                if (Colorgrades.Any) {
                    backdrop.ColorgradeA = blend.a;
                    backdrop.ColorgradeB = blend.b;
                    backdrop.LerpFactor = blend.fac;
                }
                if (Alphas.Any)
                    backdrop.Alpha = alpha;
            }
        }
    }
}
