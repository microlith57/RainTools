using Monocle;
using Celeste.Mod.Entities;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [CustomEntity("RainTools/RegionGate")]
    public class RegionGate : Entity {

        public readonly string LeftSID;
        public readonly string RightSID;
        public AreaMode? LeftMode;
        public AreaMode? RightMode;

        public Facings Facing;
        public string DestinationSID => Facing == Facings.Left ? LeftSID : RightSID;
        public AreaMode? DestinationMode => Facing == Facings.Left ? LeftMode : RightMode;
        public bool Enabled = true;

        private AreaKey? areaKey;
        private AsyncLoader loader;

        private string RoomName => (Scene as Level).Session.LevelData.Name;

        private Coroutine closeRoutine, openRoutine;

        public bool Closing => closeRoutine != null;
        public bool Opening => openRoutine != null;

        private float timer = 0f;

        public RegionGate(EntityData data, Vector2 offset) : base(data.Position + offset) {
            LeftSID = data.Attr("leftSID");
            RightSID = data.Attr("rightSID");

            if (data.Attr("leftSide") != "" && data.Attr("leftSide") != "SameAsCurrentMap")
                LeftMode = data.Enum("leftSide", AreaMode.Normal);

            if (data.Attr("rightSide") != "" && data.Attr("rightSide") != "SameAsCurrentMap")
                RightMode = data.Enum("rightSide", AreaMode.Normal);
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var level = Scene as Level;
            var sid = level.Session.Area.SID;
            var mode = level.Session.Area.Mode;

            if (sid == LeftSID)
                Facing = Facings.Left;
            else if (sid == RightSID)
                Facing = Facings.Right;
            else
                throw new Exception($"this gate connects maps with SID '{LeftSID}' and '{RightSID}', but is in a map with SID '{sid}'!");

            LeftMode ??= mode;
            RightMode ??= mode;

            areaKey = GetAreaKey();

            level.Session.SetFlag($"region_gate_middle", true);
        }

        public override void Update() {
            base.Update();

            if (closeRoutine != null && closeRoutine.Finished)
                closeRoutine = null;

            if (openRoutine != null && openRoutine.Finished)
                openRoutine = null;
        }

        public bool Activate(RegionGateActivationZone zone) {
            if (Opening || !areaKey.HasValue)
                return false;
            if (Closing || loader != null)
                return true;

            // if (zone == null) {
            //     var zones = Scene.Tracker.GetEntities<RegionGateActivationZone>()
            //                              .Cast<RegionGateActivationZone>()
            //                              .Where(z => z.Facing == Facing);
            //     if (!zones.Any())
            //         return false;

            //     zone = zones.First();
            // }

            Add(closeRoutine = new(CloseRoutine(zone)));
            return true;
        }

        private IEnumerator CloseRoutine(RegionGateActivationZone activationZone) {
            var level = Scene as Level;

            level.Session.SetFlag($"region_gate_left", true);
            level.Session.SetFlag($"region_gate_middle", true);
            level.Session.SetFlag($"region_gate_right", true);

            // var doors = level.Tracker.GetEntities<RegionGateDoor>().Cast<RegionGateDoor>();
            // doors.Select(d => d.ShouldBeOpen = false);

            // while (true) {
            //     if (!activationZone.Triggered) {
            //         doors.Where(d => d.Facing != RegionGateDoor.Facings.Middle).Select(d => d.ShouldBeOpen = true);
            //         yield break;
            //     }

            //     if (doors.All(d => d.IsSolid))
            //         break;

            //     yield return null;
            // }

            level.PauseLock = true;
            level.CanRetry = false;
            level.SaveQuitDisabled = true;

            Add(loader = new(areaKey.Value, RoomName) {
                OnLoad = OnLoadFinished
            });

            for (timer = 0; timer < 1f; timer += Engine.DeltaTime / 2f) {
                yield return null;
            }

            loader.ForceLoadSync();
        }

        private IEnumerator OpenRoutine() {
            var level = Scene as Level;

            for (; timer < 2f; timer += Engine.DeltaTime / 2f) {
                yield return null;
            }

            level.Session.SetFlag($"region_gate_left", false);
            level.Session.SetFlag($"region_gate_middle", true);
            level.Session.SetFlag($"region_gate_right", false);
        }

        private void OnLoadFinished(Level levelA, Level levelB) {
            var gateA = this;
            var gateB = levelB.Tracker.GetEntity<RegionGate>()
                      ?? throw new Exception("the room on the other side of this gate has no gate in it!");

            var delta = gateB.Position - gateA.Position;

            levelB.Camera.Position = levelA.Camera.Position + delta;
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

            gateB.Add(gateB.openRoutine = new(gateB.OpenRoutine()));
            gateB.timer = gateA.timer;

            if (Engine.Scene == levelA)
                Engine.Scene = levelB;
        }

        private AreaKey? GetAreaKey() {
            var areaData = AreaData.Get(DestinationSID)
                         ?? throw new Exception($"gate cannot go to nonexistent region {DestinationSID}!");

            MapData mapData = null;
            if ((int) DestinationMode <= areaData.Mode.Length)
                mapData = areaData.Mode[(int) DestinationMode]?.MapData;

            if (mapData == null)
                throw new Exception("gate cannot go to a nonexistent side!");

            var room = RoomName;
            var dest = mapData.Levels.Where(level => level.Name == room);
            if (!dest.Any())
                throw new Exception("no corresponding gate room on the other side!");

            return areaData.ToKey();
        }

    }
}
