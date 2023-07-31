using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/AltColorgradeTimeController")]
    public class AltColorgradeTimeController : Entity {
        public CircularColorgradeLerper Colorgrades;
        public CircularFloatLerper Alphas;

        public string SearchTag;
        public float Alpha;

        private EntityData _data;
        private Vector2 _offset;

        public AltColorgradeTimeController(string tag) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            SearchTag = tag;
            Colorgrades = new();
            Alphas = new();
        }

        public AltColorgradeTimeController(EntityData data, Vector2 offset) : this(data.Attr("tag")) {
            _data = data;
            _offset = offset;
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<AltColorgradeTimeController>()
                                        .Cast<AltColorgradeTimeController>()
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

            if (data.Attr("colorgrade") != "")
                Colorgrades.Stops[angle] = data.Attr("colorgrade", "none");

            if (data.Float("alpha", -1f) >= 0)
                Alphas.Stops[angle] = data.Float("alpha", 1f);
        }

        public override void Update() {
            base.Update();

            float sunAngle = RainToolsModule.Session.SunAngle;

            var bgs = (Scene as Level).Foreground.GetEach<AltColorgrade>(SearchTag)
                                                 .Cast<AltColorgrade>();

            var blend = Colorgrades.GetOrDefault(sunAngle);
            var alpha = Alphas.GetOrDefault(sunAngle);

            foreach (var bg in bgs) {
                if (Colorgrades.Any) {
                    bg.ColorgradeA = blend.a;
                    bg.ColorgradeB = blend.b;
                    bg.LerpFactor = blend.fac;
                }
                if (Alphas.Any)
                    bg.Alpha = alpha;
            }
        }
    }
}
