using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/CycleController")]
    public class CycleController : Entity {
        public string CycleTag;
        public float Offset, SecondsPerCycle;
        public string Flag;

        public CycleController(EntityData data, Vector2 offset) : base() {
            Tag |= Tags.Global;

            CycleTag = data.Attr("cycleTag");
            Offset = data.Float("progressionOffset");
            SecondsPerCycle = data.Float("secondsPerCycle");

            if (data.Bool("transitionUpdate"))
                Tag |= Tags.TransitionUpdate;
            if (data.Bool("frozenUpdate"))
                Tag |= Tags.FrozenUpdate;
            if (data.Bool("pauseUpdate"))
                Tag |= Tags.PauseUpdate;

            if (Offset != 0)
                Depth = -2;
            else
                Depth = -1;

            Flag = data.Attr("flag");
        }

        public override void Update() {
            base.Update();

            if (Flag != "" && !(Scene as Level).Session.GetFlag(Flag))
                return;

            if (!Cycles.Has(CycleTag))
                Cycles.SetProgression(CycleTag, Offset);

            if (SecondsPerCycle > 0)
                Cycles.AddProgression(CycleTag, Engine.DeltaTime / SecondsPerCycle);
        }
    }
}
