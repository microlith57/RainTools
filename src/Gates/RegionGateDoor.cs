using Monocle;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.RainTools {
    [Tracked]
    [CustomEntity("RainTools/RegionGate")]
    public class RegionGateDoor : Entity {

        public bool FacesLeft;
        public bool FacesRight;

        private Sprite sprite;
        private bool shouldBeOpen = false;

        public bool Opened {
            get => sprite.CurrentAnimationID == "opened";
            set {
                shouldBeOpen = true;
                sprite.Play("opened");
            }
        }

        public bool Closed {
            get => sprite.CurrentAnimationID == "closed";
            set {
                shouldBeOpen = false;
                sprite.Play("closed");
            }
        }

        public RegionGateDoor(EntityData data, Vector2 offset) : base(data.Position + offset) {
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

            Add(sprite = new(GFX.Game, data.Attr("sprite", "raintools_regiongate_door")));
            sprite.OnChange += (from, to) => {
                Collidable = to == "opened";
            };
        }

        public void Open() {
            shouldBeOpen = true;
        }

        public void Close() {
            shouldBeOpen = false;
        }

        public override void Update() {
            base.Update();

            if (Closed && shouldBeOpen)
                sprite.Play("opening");
            else if (Opened && !shouldBeOpen)
                sprite.Play("closing");
        }

    }
}
