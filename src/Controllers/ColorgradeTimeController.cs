using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/ColorgradeTimeController")]
    public class ColorgradeTimeController : Entity {
        public CircularColorgradeLerper Colorgrades;

        private EntityData _data;
        private Vector2 _offset;

        public ColorgradeTimeController() : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            Colorgrades = new();
        }

        public ColorgradeTimeController(EntityData data, Vector2 offset) : this() {
            _data = data;
            _offset = offset;
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<ColorgradeTimeController>()
                                        .Cast<ColorgradeTimeController>();

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

            Colorgrades.Add(angle, data.Attr("colorgrade", "none"), data.Attr("colorgradeEase"));
        }

        public override void Update() {
            base.Update();

            Level level = Scene as Level;

            float sunAngle = RainToolsModule.Session.SunAngle;
            var blend = Colorgrades.Get(sunAngle);

            level.lastColorGrade = blend.a;
            level.Session.ColorGrade = blend.b;
            level.colorGradeEase = blend.fac;
        }
    }
}
