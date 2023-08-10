using Monocle;
using Celeste.Mod.Entities;
using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [CustomEntity("RainTools/RegionGate")]
    // todo get hitbox position based on what door is open etc., so that Position is in the middle of the gate
    public class RegionGate : Trigger {

        public string SID;
        public AreaMode? Mode;

        private AreaKey? areaKey;
        private AsyncLoader loader;

        private string RoomName => (Scene as Level).Session.LevelData.Name;

        private float Timer;

        Color debugColor = Color.LightGray;

        public RegionGate(EntityData data, Vector2 offset) : base(data, offset) {
            SID = data.Attr("destinationSID");

            if (data.Attr("destinationMode") != "")
                Mode = data.Enum("destinationMode", AreaMode.Normal);
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            Mode ??= (Scene as Level).Session.Area.Mode;
            areaKey = GetAreaKey();
        }

        public override void OnStay(Player player) {
            base.OnStay(player);

            if (loader == null) {
                if (Timer > 1f) {
                    if (areaKey.HasValue) {
                        BeginLoad();
                    } else {
                        debugColor = Color.Red;
                    }
                } else {
                    Timer += Engine.DeltaTime;
                    debugColor = Color.Orange;
                }
            }
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);

            if (loader == null)
                debugColor = Color.LightGray;

            Timer = 0f;
        }

        public override void DebugRender(Camera camera) {
            Collider?.Render(camera, debugColor);
            Components.DebugRender(camera);
        }

        private void BeginLoad() {
            if (!areaKey.HasValue || loader != null)
                return;

            debugColor = Color.Green;

            // coarse pass on position is ok for now; will copy actual player position later
            loader = new(areaKey.Value, RoomName, Position) {
                OnLoad = EndLoad
            };
            Add(loader);

            var level = Scene as Level;
            level.PauseLock = true;
            level.CanRetry = false;
            level.SaveQuitDisabled = true;
        }

        private void EndLoad(Level levelA, Level levelB) {
            // todo use specifically gate offsets
            var offsetA = levelA.LevelOffset;
            var offsetB = levelB.LevelOffset;
            var delta = offsetB - offsetA;

            levelB.Camera.Position = levelA.Camera.Position - offsetA + offsetB;
            levelB.Wipe.Cancel();

            var playerA = levelA.Tracker.GetEntity<Player>();
            var playerB = levelB.Tracker.GetEntity<Player>();

            playerB.Position = playerA.Position + delta;
            levelB.Session.RespawnPoint = levelB.GetSpawnPoint(playerB.Position);

            playerB.Speed = playerA.Speed;
            playerB.Dashes = playerA.Dashes;
            playerB.Stamina = playerA.Stamina;

            playerB.Facing = playerA.Facing;

            playerB.Sprite.Play(playerA.Sprite.CurrentAnimationID);
            playerB.Sprite.SetAnimationFrame(playerA.Sprite.CurrentAnimationFrame);
            playerB.Sprite.animationTimer = playerA.Sprite.animationTimer;
            playerB.Sprite.Scale = playerA.Sprite.Scale;

            playerB.sweatSprite.Play(playerA.sweatSprite.CurrentAnimationID);
            playerB.sweatSprite.SetAnimationFrame(playerA.sweatSprite.CurrentAnimationFrame);
            playerB.sweatSprite.animationTimer = playerA.sweatSprite.animationTimer;

            playerB.StateMachine.State = playerA.StateMachine.State;

            playerB.Hair.Nodes = playerA.Hair.Nodes.Select(n => n + delta).ToList();
            playerB.Hair.Color = playerA.Hair.Color;
            playerB.hairFlashTimer = playerA.hairFlashTimer;

            playerB.dashAttackTimer = playerA.dashAttackTimer;
            playerB.dashCooldownTimer = playerA.dashCooldownTimer;
            playerB.dashRefillCooldownTimer = playerA.dashRefillCooldownTimer;
            playerB.dashStartedOnGround = playerA.dashStartedOnGround;
            playerB.dashTrailCounter = playerA.dashTrailCounter;
            playerB.dashTrailTimer = playerA.dashTrailTimer;

            levelB.Particles.particles = levelA.Particles.particles;
            levelB.ParticlesBG.particles = levelA.ParticlesBG.particles;
            levelB.ParticlesFG.particles = levelA.ParticlesFG.particles;

            levelB.Displacement.points = levelA.Displacement.points.Select(
                p => {
                    var burst = new DisplacementRenderer.Burst(
                        p.Texture,
                        p.Position + delta,
                        p.Origin + delta,
                        p.Duration) {
                        Percent = p.Percent,
                        ScaleFrom = p.ScaleFrom,
                        ScaleTo = p.ScaleTo,
                        ScaleEaser = p.ScaleEaser,
                        AlphaFrom = p.AlphaFrom,
                        AlphaTo = p.AlphaTo,
                        AlphaEaser = p.AlphaEaser,
                        WorldClipPadding = p.WorldClipPadding
                    };
                    if (p.WorldClipRect.HasValue) {
                        var rect = p.WorldClipRect.Value;
                        burst.WorldClipRect = new(
                            rect.X + (int) delta.X, rect.Y + (int) delta.Y,
                            rect.Width, rect.Height
                        );
                    }

                    return burst;
                }
            ).ToList();

            if (Engine.Scene == levelA)
                Engine.Scene = levelB;
        }

        private AreaKey? GetAreaKey() {
            var areaData = AreaData.Get(SID)
                         ?? throw new Exception($"gate cannot go to nonexistent region {SID}!");

            MapData mapData = null;
            if ((int) Mode <= areaData.Mode.Length)
                mapData = areaData.Mode[(int) Mode]?.MapData;

            if (mapData == null)
                throw new Exception("gate cannot go to a nonexistent side!");

            var room = RoomName;
            var dest = mapData.Levels.Where(level => level.Name == room);
            if (!dest.Any())
                throw new Exception("no corresponding gate room on the other side!");

            // var destData = new DynamicData(dest.First());
            // if (!destData.TryGet("", out bool isGate) || !isGate)
            //     throw new Exception("the room on the other side of this gate has no gate in it!");

            return areaData.ToKey();
        }

    }
}
