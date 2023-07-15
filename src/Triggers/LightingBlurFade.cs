using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.RainTools {
    [CustomEntity("RainTools/LightingBlurFade")]
    public class LightingBlurFadeTrigger : Trigger {
        public PositionModes PositionMode;

        public bool FadeBlur1, FadeBlur2;
        public float Blur1From, Blur1To, Blur2From, Blur2To;

        public string[] EffectTags;

        public LightingBlurFadeTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            Tag |= Tags.TransitionUpdate;

            PositionMode = data.Enum("positionMode", PositionModes.NoEffect);

            Blur1From = data.Float("blur1From", 1f);
            Blur1To = data.Float("blur1To", 1f);
            Blur2From = data.Float("blur2From", 1f);
            Blur2To = data.Float("blur2To", 1f);

            FadeBlur1 = data.Bool("fadeBlur1", false);
            FadeBlur2 = data.Bool("fadeBlur2", true);

            EffectTags = data.Attr("tag", "sun").Split(',');
        }

        public override void OnStay(Player player) {
            float fac = MathHelper.Clamp(GetPositionLerp(player, PositionMode), 0f, 1f);
            float blur1 = MathHelper.Lerp(Blur1From, Blur1To, fac);
            float blur2 = MathHelper.Lerp(Blur2From, Blur2To, fac);

            var level = Scene as Level;

            var effects = level.Foreground.Backdrops
                .FindAll((e) => e is Sunlight l
                             && l.Tags.Intersect(EffectTags).Any())
                .Cast<Sunlight>();

            foreach (var effect in effects) {
                if (FadeBlur1)
                    effect.Blur1 = blur1;
                if (FadeBlur2)
                    effect.Blur2 = blur2;
            }
        }
    }
}
