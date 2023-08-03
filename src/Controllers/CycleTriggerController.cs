using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools {
    public abstract class CycleTriggerController : Entity {

        public string CycleTag;
        public float CenterAngle;
        public float ArcAngle;

        private bool state;

        public CycleTriggerController(EntityData data, Vector2 offset) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            CycleTag = data.Attr("cycleTag");

            Vector2 pos = data.Position + offset;
            Vector2 nodePos = data.NodesOffset(offset)[0];
            CenterAngle = (nodePos - pos).Angle();

            ArcAngle = data.Float("arcAngle");
        }

        public override void Update() {
            base.Update();

            float angle = Cycles.GetAngle(CycleTag);
            bool newState = Calc.AbsAngleDiff(CenterAngle, angle) <= CenterAngle / 2f;

            if (newState && !state) {
                OnEnter();
                state = newState;
            } else if (!newState && state) {
                OnLeave();
                state = newState;
            } else if (state) {
                OnStay();
            }
        }

        public virtual void OnEnter() { }
        public virtual void OnLeave() { }
        public virtual void OnStay() { }

    }
}
