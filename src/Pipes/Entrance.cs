using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.RainTools.Pipes {
    [CustomEntity("RainTools/PipeEntrance")]
    [Tracked(true)]
    public class Entrance : Entity {

        public enum Directions {
            Left, Right, Up, Down
        }
        public Directions Direction;

        public Endpoint Endpoint;

        public Entrance(EntityData data, Vector2 offset) : base(data.Position + offset) {
            Direction = data.Enum<Directions>("direction");

            Add(Endpoint = new(Vector2.Zero));

            Collider = new Hitbox(10, 10, -1, -1);
        }

        public override void Update() {
            base.Update();

            var level = Scene as Level;
            var player = level.Tracker.GetEntity<Player>();

            if (!player.InControl || !CollideCheck(player))
                return;

            switch (Direction) {
                case Directions.Left:
                    if (Input.MoveX.Value > 0 && player.Speed.X >= 0)
                        Entered(player);
                    break;
                case Directions.Right:
                    if (Input.MoveX.Value < 0 && player.Speed.X <= 0)
                        Entered(player);
                    break;
                case Directions.Up:
                    if (Input.MoveY.Value < 0 && player.Speed.Y <= 0)
                        Entered(player);
                    break;
                case Directions.Down:
                    if (Input.MoveY.Value > 0 && player.Speed.Y >= 0)
                        Entered(player);
                    break;
            }
        }

        private void Entered(Player player) {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;
        }

        public override void Render() {
            base.Render();
        }

    }
}
