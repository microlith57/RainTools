using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;

namespace Celeste.Mod.RainTools {
    [CustomEntity("RainTools/StylegroundFade")]
    public class StylegroundFadeTrigger : Trigger {
        public string StyleTag;

        public Color ColorFrom, ColorTo;
        public Ease.Easer ColorEase = Ease.Linear;

        public float AlphaFrom, AlphaTo;
        public Ease.Easer AlphaEase = Ease.Linear;

        public PositionModes PositionMode;
        public ColorRGBAAlphaChangeMode ChangeMode;

        public StylegroundFadeTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            StyleTag = data.Attr("styleTag");

            PositionMode = data.Enum<PositionModes>("positionMode");
            ChangeMode = data.Enum<ColorRGBAAlphaChangeMode>("mode", ColorRGBAAlphaChangeMode.ColorTimesPrevA);

            ColorFrom = Calc.HexToColorWithAlpha(data.Attr("colorFrom"));
            ColorTo = Calc.HexToColorWithAlpha(data.Attr("colorTo"));

            if (data.Attr("colorEase") != "")
                ColorEase = FrostHelper.API.API.GetEaser(data.Attr("colorEase"));

            AlphaFrom = data.Float("alphaFrom");
            AlphaTo = data.Float("alphaTo");

            if (data.Attr("alphaEase") != "")
                AlphaEase = FrostHelper.API.API.GetEaser(data.Attr("alphaEase"));
        }

        public override void OnStay(Player player) {
            float fac = GetPositionLerp(player, PositionMode);
            Color color = Color.Lerp(ColorFrom, ColorTo, fac);
            float alpha = Calc.ClampedMap(fac, 0f, 1f, AlphaFrom, AlphaTo);

            Apply(Scene as Level, StyleTag, color, alpha, ChangeMode);
        }

        public static void Apply(Level level, string tag, Color color, float alpha, ColorRGBAAlphaChangeMode mode) {
            var fgs = level.Foreground.GetEach<Backdrop>(tag);
            var bgs = level.Background.GetEach<Backdrop>(tag);
            var backdrops = fgs.Union(bgs);

            switch (mode) {
                case ColorRGBAAlphaChangeMode.ColorTimesAlpha:
                    color *= alpha;
                    goto case ColorRGBAAlphaChangeMode.ColorOnly;

                case ColorRGBAAlphaChangeMode.ColorOnly:
                    foreach (var backdrop in backdrops)
                        backdrop.Color = color;
                    break;

                case ColorRGBAAlphaChangeMode.ColorTimesPrevA:
                    foreach (var backdrop in backdrops)
                        backdrop.Color = color * (backdrop.Color.A / 255f);
                    break;

                case ColorRGBAAlphaChangeMode.AlphaOnly:
                    foreach (var backdrop in backdrops)
                        backdrop.FadeAlphaMultiplier = alpha;
                    break;

                case ColorRGBAAlphaChangeMode.ColorAndAlpha:
                    foreach (var backdrop in backdrops) {
                        backdrop.Color = color;
                        backdrop.FadeAlphaMultiplier = alpha;
                    }
                    break;
            }
        }
    }
}
