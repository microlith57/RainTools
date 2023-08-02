using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.RainTools {
    [CustomEntity("RainTools/LightingBlurFade")]
    public class LightingBlurFadeTrigger : Trigger {
        public string StyleTag;

        public PositionModes PositionMode;
        public BlurLayerChangeMode ChangeMode;

        public float Blur1From, Blur1To, Blur2From, Blur2To;

        public LightingBlurFadeTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            Tag |= Tags.TransitionUpdate;

            StyleTag = data.Attr("styleTag");

            PositionMode = data.Enum("positionMode", PositionModes.NoEffect);

            Blur1From = data.Float("blur1From", 1f);
            Blur1To = data.Float("blur1To", 1f);
            Blur2From = data.Float("blur2From", 1f);
            Blur2To = data.Float("blur2To", 1f);

            ChangeMode = data.Enum<BlurLayerChangeMode>("mode", BlurLayerChangeMode.Both);
        }

        public override void OnStay(Player player) {
            float fac = MathHelper.Clamp(GetPositionLerp(player, PositionMode), 0f, 1f);
            float blur1 = MathHelper.Lerp(Blur1From, Blur1To, fac);
            float blur2 = MathHelper.Lerp(Blur2From, Blur2To, fac);

            var level = Scene as Level;

            var effects = level.Foreground.GetEach<Sunlight>(StyleTag)
                                          .Cast<Sunlight>();

            foreach (var effect in effects) {
                if (ChangeMode != BlurLayerChangeMode.OnlyBlur2)
                    effect.Blur1 = blur1;
                if (ChangeMode != BlurLayerChangeMode.OnlyBlur1)
                    effect.Blur2 = blur2;
            }
        }
    }
}
