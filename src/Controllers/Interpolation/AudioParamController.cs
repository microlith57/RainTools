using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/AudioParamCycleController")]
    public class AudioParamCycleController : Entity {

        public string CycleTag;
        public string Flag = "";
        public bool IsAmbience;
        public string Param;

        public CircularFloatInterpolator Values;

        private EntityData _data;
        private Vector2 _offset;

        public AudioParamCycleController(string cycleTag, string param, bool ambience) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            CycleTag = cycleTag;
            Param = param;
            IsAmbience = ambience;

            Values = new();
        }

        public AudioParamCycleController(EntityData data, Vector2 offset)
            : this(data.Attr("cycleTag"), data.Attr("param", "fade"), data.Bool("ambience")) {

            _data = data;
            _offset = offset;
            Flag = data.Attr("flag");
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<AudioParamCycleController>()
                                        .Cast<AudioParamCycleController>()
                                        .Where((c) => c.IsAmbience == IsAmbience && c.Param == Param);

            if (existing.Any((c) => c != this)) {
                existing.First().AddStop(_data, _offset);
                RemoveSelf();
                return;
            }

            AddStop(_data, _offset);
            _data = null;
        }

        public void AddStop(EntityData data, Vector2 offset) {
            Vector2 pos = data.Position + offset;
            Vector2 nodePos = data.NodesOffset(offset)[0];
            var angle = (nodePos - pos).Angle();

            Values.Add(angle, data.Float("value"));
        }

        public override void Update() {
            base.Update();

            var level = Scene as Level;
            if (Flag != "" && !level.Session.GetFlag(Flag))
                return;

            float angle = Cycles.GetAngle(CycleTag);
            var val = Values.Get(angle);

            if (IsAmbience) {
                level.Session.Audio.Ambience.Param(Param, val);
                level.Session.Audio.Apply();
            } else {
                Audio.SetMusicParam(Param, val);
            }
        }

    }
}
