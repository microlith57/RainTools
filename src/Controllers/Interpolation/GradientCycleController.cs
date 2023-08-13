using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/GradientCycleController")]
    public class GradientCycleController : Entity {

        public string CycleTag;
        public string StyleTag;
        public string Flag = "";

        public BlendedCircularInterpolator<Color[]> Colors;

        private EntityData _data;
        private Vector2 _offset;

        public GradientCycleController(string cycleTag, string styleTag) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            CycleTag = cycleTag;
            StyleTag = styleTag;

            Colors = new();
        }

        public GradientCycleController(EntityData data, Vector2 offset)
            : this(data.Attr("cycleTag"),
                   data.Attr("styleTag")) {

            _data = data;
            _offset = offset;
            Flag = data.Attr("flag");
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<GradientCycleController>()
                                        .Cast<GradientCycleController>()
                                        .Where((c) => c.StyleTag == StyleTag);

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

            Colors.Add(angle, data.Attr("colors", "7bbedf,0c56c2")
                                  .Split(',')
                                  .Select(part => Calc.HexToColorWithAlpha(part.Trim()))
                                  .ToArray(),
                       data.Attr("ease", "Linear"));
        }

        public override void Update() {
            base.Update();

            var level = Scene as Level;
            if (Flag != "" && !level.Session.GetFlag(Flag))
                return;

            float angle = Cycles.GetAngle(CycleTag);
            var blend = Colors.Get(angle);

            var fgs = level.Foreground.GetEach<Backdrops.Gradient>(StyleTag);
            var bgs = level.Background.GetEach<Backdrops.Gradient>(StyleTag);

            foreach (var backdrop in fgs.Union(bgs))
                backdrop.SetColors(blend);
        }

    }
}