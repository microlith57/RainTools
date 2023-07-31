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

        public string GroupTag;
        public float Alpha;

        private AltColorgrade.Controller controller;

        private EntityData _data;
        private Vector2 _offset;

        public AltColorgradeTimeController(string tag) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            GroupTag = tag;
            Colorgrades = new();
            Alphas = new();

            Add(controller = new());
            controller.Tag = GroupTag;
        }

        public AltColorgradeTimeController(EntityData data, Vector2 offset) : this(data.Attr("tag")) {
            _data = data;
            _offset = offset;
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<AltColorgradeTimeController>()
                                        .Cast<AltColorgradeTimeController>()
                                        .Where((c) => c.GroupTag == GroupTag);

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

            Colorgrades.Stops[angle] = data.Attr("colorgrade", "none");
            Alphas.Stops[angle] = data.Float("alpha", 1f);
        }

        public override void Update() {
            base.Update();

            float sunAngle = RainToolsModule.Session.SunAngle;

            var blend = Colorgrades.Get(sunAngle);
            var alpha = Alphas.Get(sunAngle);

            controller.ColorgradeA = blend.a;
            controller.ColorgradeB = blend.b;
            controller.LerpFactor = blend.fac;
            controller.Alpha = alpha;
        }
    }
}
