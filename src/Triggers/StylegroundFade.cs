using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.RainTools {
    [CustomEntity("RainTools/StylegroundFade")]
    public class StylegroundFadeTrigger : Trigger {
        public Color ColorFrom, ColorTo;
        public Ease.Easer ColorEase = Ease.Linear;

        public float AlphaFrom, AlphaTo;
        public Ease.Easer AlphaEase = Ease.Linear;

        public ColorAlphaChangeMode ChangeMode;

        public PositionModes mode;
        public string SearchTag;

        public StylegroundFadeTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            ChangeMode = data.Enum<ColorAlphaChangeMode>("mode", ColorAlphaChangeMode.Both);

            if (ChangeMode != ColorAlphaChangeMode.AlphaOnly
                && data.Attr("colorFrom") != ""
                && data.Attr("colorTo") != "") {

                ColorFrom = Calc.HexToColorWithAlpha(data.Attr("colorFrom"));
                ColorTo = Calc.HexToColorWithAlpha(data.Attr("colorTo"));

                if (data.Attr("colorEase") != "")
                    ColorEase = FrostHelper.EaseHelper.GetEase(data.Attr("colorEase"));
            }

            if (ChangeMode != ColorAlphaChangeMode.ColorOnly
                && data.Float("alphaFrom", -1f) >= 0f
                && data.Float("alphaTo", -1f) >= 0f) {

                AlphaFrom = data.Float("alphaFrom");
                AlphaTo = data.Float("alphaTo");

                if (data.Attr("alphaEase") != "")
                    AlphaEase = FrostHelper.EaseHelper.GetEase(data.Attr("alphaEase"));
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
                if (ChangeMode != ColorAlphaChangeMode.AlphaOnly)
                    fg.Color = color;
                if (ChangeMode != ColorAlphaChangeMode.ColorOnly)
                    fg.FadeAlphaMultiplier = alpha;
            }

            var bgs = (Scene as Level).Background.GetEach<Backdrop>(SearchTag);
            foreach (var bg in bgs) {
                if (ChangeMode != ColorAlphaChangeMode.AlphaOnly)
                    bg.Color = color;
                if (ChangeMode != ColorAlphaChangeMode.ColorOnly)
                    bg.FadeAlphaMultiplier = alpha;
            }
        }
    }
}
