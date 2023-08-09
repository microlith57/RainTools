using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/SunlightAngleCycleController")]
    public class SunlightAngleCycleController : Entity {

        public string CycleTag;
        public string StyleTag;
        public float Offset;
        public int Multiplier;

        public SunlightAngleCycleController(EntityData data, Vector2 offset) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            CycleTag = data.Attr("cycleTag");
            StyleTag = data.Attr("styleTag");
            Offset = data.Float("angleOffset", 0f);
            Multiplier = data.Int("angleMultiplier", 1);
        }

        public override void Update() {
            base.Update();

            var fgs = (Scene as Level).Foreground.GetEach<Backdrops.Sunlight>(StyleTag)
                                                 .Cast<Backdrops.Sunlight>();
            if (!fgs.Any())
                return;

            float angle = Calc.WrapAngle(Cycles.GetAngle(CycleTag) * Multiplier + Offset);

            foreach (var backdrop in fgs) {
                backdrop.Angle = angle;
            }
        }

    }
}
