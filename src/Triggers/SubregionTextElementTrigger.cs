using Celeste.Mod.Entities;
using Celeste.Mod.RainTools.Subregion;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.RainTools.Triggers {
    [CustomEntity("RainTools/SubregionTextElementTrigger")]
    [Tracked]
    public class SubregionTextElementTrigger : Trigger {

        private string SubregionID;

        public SubregionTextElementTrigger(EntityData data, Vector2 offset)
            : base(data, offset) {

            #region Entity Data
            SubregionID = data.Attr("subregionID", "");
            #endregion
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);
            List<Entity> controllers = (Scene as Level).Tracker.GetEntities<Controller>();

            // check for a controller with the same subregion ID and activate it
            foreach (Controller controller in controllers) {
                if (controller.SubregionID != SubregionID)
                    continue;
                controller.HandleTransition(true);
                RemoveSelf();
                return;
            }
        }
    }
}
