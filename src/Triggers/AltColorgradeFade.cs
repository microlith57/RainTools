using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools {
    [CustomEntity("RainTools/AltColorgradeFade")]
    public class AltColorgradeFadeTrigger : Trigger {
        public float AlphaFrom, AlphaTo;
        public PositionModes mode;
        public string SearchTag;

        public AltColorgradeFadeTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            SearchTag = data.Attr("tag");
            AlphaFrom = data.Float("alphaFrom");
            AlphaTo = data.Float("alphaTo");
            mode = data.Enum<PositionModes>("positionMode");
        }

        public override void OnStay(Player player) {
            float alpha = Calc.ClampedMap(GetPositionLerp(player, mode), 0f, 1f, AlphaFrom, AlphaTo);

            var controllers = Scene.Tracker.GetComponents<AltColorgrade.Controller>()
                                           .Cast<AltColorgrade.Controller>()
                                           .Where((c) => SearchTag == "" ? true : c.Tag == SearchTag);

            foreach (var controller in controllers) {
                controller.Alpha = alpha;
            }
        }
    }
}
