using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools {
    [CustomEntity("RainTools/StylegroundFade")]
    public class StylegroundFadeTrigger : Trigger {
        public Color ColorFrom, ColorTo;
        public bool ChangeColor = false;

        public float AlphaFrom, AlphaTo;
        public bool ChangeAlpha = false;

        public PositionModes mode;
        public string SearchTag;

        public StylegroundFadeTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            var change = data.Enum<ColorAlphaChangeMode>("mode", ColorAlphaChangeMode.Both);

            if (change != ColorAlphaChangeMode.AlphaOnly
                && data.Attr("colorFrom") != ""
                && data.Attr("colorTo") != "") {

                ColorFrom = Calc.HexToColorWithAlpha(data.Attr("colorFrom"));
                ColorTo = Calc.HexToColorWithAlpha(data.Attr("colorTo"));
                ChangeColor = true;
            }

            if (change != ColorAlphaChangeMode.ColorOnly
                && data.Float("alphaFrom", -1f) >= 0f
                && data.Float("alphaTo", -1f) >= 0f) {

                AlphaFrom = data.Float("alphaFrom");
                AlphaTo = data.Float("alphaTo");
                ChangeAlpha = true;
            }

            mode = data.Enum<PositionModes>("positionMode");
            SearchTag = data.Attr("tag");
        }

        public override void OnStay(Player player) {
            float fac = GetPositionLerp(player, mode);

            Color color = Color.Lerp(ColorFrom, ColorTo, fac);
            float alpha = Calc.ClampedMap(fac, 0f, 1f, AlphaFrom, AlphaTo);

            var fgs = (Scene as Level).Foreground.GetEach<Backdrop>(SearchTag);
            foreach (var fg in fgs) {
                if (ChangeColor)
                    fg.Color = color;
                if (ChangeAlpha)
                    fg.FadeAlphaMultiplier = alpha;
            }

            var bgs = (Scene as Level).Background.GetEach<Backdrop>(SearchTag);
            foreach (var bg in bgs) {
                if (ChangeColor)
                    bg.Color = color;
                if (ChangeAlpha)
                    bg.FadeAlphaMultiplier = alpha;
            }
        }
    }
}
