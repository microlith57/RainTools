using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [Tracked(true)]
    [GlobalEntity]
    [CustomEntity("RainTools/CloudscapeCycleController")]
    public class CloudscapeCycleController : Entity {

        public string CycleTag;
        public string StyleTag;

        public CircularColorInterpolator BackgroundColors;
        public CircularColorGradientInterpolator RingColors;

        private EntityData _data;
        private Vector2 _offset;

        public CloudscapeCycleController(string cycleTag, string styleTag) : base() {
            Tag |= Tags.Global | Tags.TransitionUpdate | Tags.FrozenUpdate;

            CycleTag = cycleTag;
            StyleTag = styleTag;

            BackgroundColors = new();
            RingColors = new();
        }

        public CloudscapeCycleController(EntityData data, Vector2 offset)
            : this(data.Attr("cycleTag"),
                   data.Attr("styleTag")) {

            _data = data;
            _offset = offset;
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            var existing = scene.Tracker.GetEntities<CloudscapeCycleController>()
                                        .Cast<CloudscapeCycleController>()
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

            BackgroundColors.Add(angle, Calc.HexToColorWithAlpha(data.Attr("bgcolor", "ffffff")));

            RingColors.Add(angle, data.Attr("colors", "ffffff")
                                      .Split(',')
                                      .Select(part => Calc.HexToColorWithAlpha(part.Trim()))
                                      .ToArray());
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);

            if (!ModIntegration.CommunalHelper.Loaded) {
                Audio.SetMusic(null);
                LevelEnter.ErrorMessage = "{big}Oops!{/big}{n}To use {# F94A4A}Cloudscape Cycle Controllers{#}, you need to have {# d678db}Communal Helper{#} installed!";
                LevelEnter.Go(new Session(SceneAs<Level>().Session.Area), fromSaveData: false);
            }
        }

        public override void Update() {
            base.Update();

            float angle = Cycles.GetAngle(CycleTag);

            var bg = BackgroundColors.Get(angle);
            var blend = RingColors.Get(angle);

            ModIntegration.CommunalHelper.ConfigureCloudscapes(Scene as Level, StyleTag, bg, blend.a, blend.b, blend.fac);
        }

    }
}