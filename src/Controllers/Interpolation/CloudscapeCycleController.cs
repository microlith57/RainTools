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
        public string Flag = "";

        public CircularColorInterpolator BackgroundColors;
        public BlendedCircularInterpolator<Color[]> RingColors;

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
            Flag = data.Attr("flag");
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

            RingColors.Add(angle, data.Attr("ringColors", "6d8ada,aea0c1,d9cbbc")
                                      .Split(',')
                                      .Select(part => Calc.HexToColorWithAlpha(part.Trim()))
                                      .ToArray(),
                           data.Attr("ringEase", "Linear"));

            BackgroundColors.Add(angle, Calc.HexToColorWithAlpha(data.Attr("backgroundColor", "4f9af7")), data.Attr("backgroundEase", "Linear"));
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

            var level = Scene as Level;
            if (Flag != "" && !level.Session.GetFlag(Flag))
                return;

            float angle = Cycles.GetAngle(CycleTag);

            var bg = BackgroundColors.Get(angle);
            var blend = RingColors.Get(angle);

            ModIntegration.CommunalHelper.ConfigureCloudscapes(Scene as Level, StyleTag, bg, blend.a, blend.b, blend.fac);
        }

    }
}