using Monocle;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [CustomEntity("RainTools/RegionGateActivationZone")]
    public class RegionGateActivationZone : Trigger {

        public bool FacesLeft;
        public bool FacesRight;
        public float ActivationDelay;

        private Sprite sprite;
        private float timer;
        private bool activated;

        public RegionGateActivationZone(EntityData data, Vector2 offset) : base(data, offset) {
            Depth = Depths.Below;

            switch (data.Attr("facing")) {
                case "Middle":
                    FacesLeft = false;
                    FacesRight = false;
                    break;
                case "Left":
                    FacesLeft = true;
                    FacesRight = false;
                    break;
                case "Right":
                    FacesLeft = false;
                    FacesRight = true;
                    break;
                case "LeftAndRight":
                    FacesLeft = true;
                    FacesRight = true;
                    break;
                default:
                    goto case "LeftAndRight";
            }

            ActivationDelay = data.Float("activationDelay", 1f);

            Add(sprite = new(GFX.Game, data.Attr("sprite", "raintools_regiongate_icon")) {
                Position = data.NodesOffset(offset)[0] - Position,
                Origin = new(sprite.Width / 2f, sprite.Height / 2f)
            });

            Add(new CustomBloom(sprite.Render));
        }

        public override void Update() {
            base.Update();

            // todo make this not bad
            sprite.Color = Color.White * Calc.ClampedMap(timer / ActivationDelay, 0f, 0.8f, 1f, 0f);
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
            if (activated)
                return;

            timer += Engine.DeltaTime;

            if (timer >= ActivationDelay) {
                (Scene as Level).Tracker.GetEntity<RegionGate>().Activate(this);
                activated = true;
            }
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
            timer = 0;
            activated = false;
        }

    }
}
