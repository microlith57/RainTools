using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/SunlightAngleTimeController")]
    public class SunlightAngleTimeController : Entity {
        public string StylegroundTag;
        public float SecondsPerCycle;

        public SunlightAngleTimeController(EntityData data, Vector2 offset) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            StylegroundTag = data.Attr("tag");
            SecondsPerCycle = data.Float("seconds_per_cycle", 0f);
        }

        public override void Update() {
            base.Update();

            if (SecondsPerCycle != 0f)
                RainToolsModule.Session.Time += Engine.DeltaTime / SecondsPerCycle;

            float sunAngle = RainToolsModule.Session.SunAngle;

            foreach (var fg in (Scene as Level).Foreground.GetEach<Sunlight>(StylegroundTag)) {
                fg.Angle = sunAngle;
            }
        }
    }
}
