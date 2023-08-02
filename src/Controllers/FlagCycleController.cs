using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/FlagCycleController")]
    public class FlagCycleController : CycleTriggerController {
        public string Flag;
        public bool StateOnEnter, StateOnLeave;

        public FlagCycleController(EntityData data, Vector2 offset) : base(data, offset) {
            Flag = data.Attr("flag");
            StateOnEnter = data.Bool("stateOnEnter");
            StateOnLeave = data.Bool("stateOnLeave");
        }

        public override void OnEnter() {
            (Scene as Level).Session.SetFlag(Flag, StateOnEnter);
        }

        public override void OnLeave() {
            (Scene as Level).Session.SetFlag(Flag, StateOnLeave);
        }
    }
}
