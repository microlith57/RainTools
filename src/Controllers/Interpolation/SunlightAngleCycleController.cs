using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/StylegroundAngleCycleController")]
    public class StylegroundAngleCycleController : Entity {

        public string CycleTag;
        public string StyleTag;
        public string Flag = "";
        public float Offset;
        public int Multiplier;

        public StylegroundAngleCycleController(EntityData data, Vector2 offset) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            CycleTag = data.Attr("cycleTag");
            StyleTag = data.Attr("styleTag");
            Flag = data.Attr("flag");
            Offset = data.Float("angleOffsetDegrees", 0) * Calc.DegToRad;
            Multiplier = data.Int("angleMultiplier", 1);
        }

        public override void Update() {
            base.Update();

            var level = Scene as Level;
            if (Flag != "" && !level.Session.GetFlag(Flag))
                return;

            var bgs = level.Background.GetEach<Backdrops.IHasAngle>(StyleTag)
                                      .Cast<Backdrops.IHasAngle>();
            var fgs = level.Foreground.GetEach<Backdrops.IHasAngle>(StyleTag)
                                      .Cast<Backdrops.IHasAngle>();

            float angle = Calc.WrapAngle(Cycles.GetAngle(CycleTag) * Multiplier + Offset);

            foreach (var backdrop in bgs.Union(fgs)) {
                backdrop.SetAngle(angle);
            }
        }

    }
}
