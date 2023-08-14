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

        // private Image image;
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

            // Vector2 pos = Center - Position;

            // Add(image = new(GFX.Game["objects/RainTools/RegionGate/icon"]) {
            //     Position = pos,
            //     Origin = new(image.Width / 2f, image.Height / 2f)
            // });

            // Add(new CustomBloom(image.Render));
        }

        public override void Update() {
            base.Update();

            // todo make this not bad
            // image.Color = Color.White * Calc.ClampedMap(timer / ActivationDelay, 0f, 0.8f, 1f, 0f);
        }

        public override void OnStay(Player player) {
            base.OnStay(player);
            if (activated)
                return;

            timer += Engine.DeltaTime;

            if (timer >= ActivationDelay) {
                var gate = (Scene as Level).Tracker.GetEntity<RegionGate>();

                gate.Activate(this);
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
