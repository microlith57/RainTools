using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools {
    [CustomEntity("RainTools/LightingColorFade")]
    public class LightingColorFadeTrigger : Trigger {
        public PositionModes PositionMode;

        public bool ColorOnly;
        public Color ColorFrom, ColorTo;
        public float AlphaFrom, AlphaTo;

        public string[] EffectTags;

        public LightingColorFadeTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            Tag |= Tags.TransitionUpdate;

            PositionMode = data.Enum("positionMode", PositionModes.NoEffect);

            ColorFrom = Calc.HexToColor(data.Attr("colorFrom"));
            ColorTo = Calc.HexToColor(data.Attr("colorTo"));
            AlphaFrom = data.Float("alphaFrom", 1f);
            AlphaFrom = data.Float("alphaTo", 1f);

            ColorOnly = data.Bool("colorOnly");

            EffectTags = data.Attr("tag", "sun").Split(',');
        }

        public override void OnStay(Player player) {
            float fac = MathHelper.Clamp(GetPositionLerp(player, PositionMode), 0f, 1f);
            Color col = Color.Lerp(ColorFrom, ColorTo, fac);
            float alpha = MathHelper.Lerp(AlphaFrom, AlphaTo, fac);

            var level = Scene as Level;

            var effects = level.Foreground.Backdrops
                .FindAll((e) => e is LightingStyleground l
                             && l.Tags.Intersect(EffectTags).Any())
                .Cast<LightingStyleground>();

            foreach (var effect in effects) {
                if (ColorOnly) {
                    effect.Color = col * effect.Color.A;
                } else {
                    effect.Color = col;
                }
            }
        }
    }
}
