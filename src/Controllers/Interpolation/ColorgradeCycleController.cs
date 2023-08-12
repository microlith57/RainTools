using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/ColorgradeCycleController")]
    public class ColorgradeCycleController : Entity {

        public BlendedCircularInterpolator<string> Colorgrades;

        public string CycleTag;
        public string Flag = "";

        private EntityData _data;
        private Vector2 _offset;

        public ColorgradeCycleController(string cycleTag) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            CycleTag = cycleTag;

            Colorgrades = new();
        }

        public ColorgradeCycleController(EntityData data, Vector2 offset) : this(data.Attr("cycleTag")) {
            _data = data;
            _offset = offset;
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<ColorgradeCycleController>()
                                        .Cast<ColorgradeCycleController>();

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

            if (Flag == "")
                Flag = data.Attr("flag");
        }

        public override void Update() {
            base.Update();

            Level level = Scene as Level;

            if (Flag != "" && !level.Session.GetFlag(Flag))
                return;

            float angle = Cycles.GetAngle(CycleTag);
            var blend = Colorgrades.Get(angle);

            level.lastColorGrade = blend.a;
            level.Session.ColorGrade = blend.b;
            level.colorGradeEase = blend.fac;
        }

    }
}
