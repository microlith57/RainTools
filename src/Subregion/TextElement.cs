using System;
using Monocle;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using FMOD;

namespace Celeste.Mod.RainTools.Subregion {

    [Tracked]
    public class TextElement : Entity {

        private Coroutine routine;

        private bool closing;

        private float barEase;

        private float Delay;

        private float Duration;

        private float EaseTime;

        private float timer;

        private float textEase;

        private int cycleNum;

        private string subregionText;

        public TextElement(string cycleTag, string dialogKey, float duration, float easeTime, float delay) {
            Tag = Tags.HUD | Tags.TransitionUpdate | Tags.Persistent;

            #region Entity Data
            cycleNum = (int) Math.Floor(Cycles.GetProgression(cycleTag));
            Delay = delay;
            Duration = duration;
            EaseTime = easeTime;
            subregionText = Dialog.Clean(dialogKey.Trim());
            #endregion

            #region Components
            Add(routine = new Coroutine(Routine()));
            routine.UseRawDeltaTime = true;
            Add(new TransitionListener {

                OnOutBegin = delegate {
                    if (!closing)
                        routine.Replace(Close());
                }

            });
            #endregion
        }

        public override void Update() {
            timer += Engine.DeltaTime;
            if ((base.Scene as Level).RetryPlayerCorpse != null && !closing) {
                routine.Replace(Close());
            }
            base.Update();
        }

        public override void Render() {
            Level level = Scene as Level;
            string text = $"Cycle {cycleNum} ~ {Dialog.Clean(AreaData.Areas[SaveData.Instance.Areas_Safe[level.Session.Area.ID].ID_Safe].Name)}, {subregionText}";
            if (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene)
                return;

            float thicknessPercent = 0.05f;
            float thickness = Engine.Height * thicknessPercent;
            float barHeight = (1f - (1f - barEase) * (1f - barEase)) * thicknessPercent * Engine.Height;
            float bottomBarY = Engine.Height - barHeight;

            Draw.Rect(0f, bottomBarY, Engine.Width, barHeight + 10f, Color.Black);
            Draw.Rect(0f, 0f, Engine.Width, barHeight + 10f, Color.Black);

            float fontHeight = ActiveFont.Measure(text).Y;
            float textHeight = (thicknessPercent * Engine.Height - .25f * thickness) / fontHeight;

            ActiveFont.Draw(text, new Vector2(thickness, bottomBarY + Engine.Height * thicknessPercent / 2f), new Vector2(0f, 0.5f), Vector2.One * textHeight, Color.White * textEase);

            base.Render();
        }

        private IEnumerator Routine() {
            // close others
            List<Entity> entities = Scene.Tracker.GetEntities<TextElement>();
            foreach (TextElement item in entities) {
                if (item != this) {
                    item.Components.Get<Coroutine>().Replace(item.Close());
                }
            }
            // wait for them
            if (entities.Count > 1) {
                yield return EaseTime * 2 + 0.5f;
            }

            // set the timer for delay
            timer = -Delay;
            while (timer <= 0f) {
                yield return null;
            }

            // reset timer
            timer = 0f;
            while (timer < EaseTime) {
                barEase += Engine.DeltaTime / EaseTime;
                if (closing)
                    yield break;
                yield return null;
            }
            barEase = 1f;
            while (timer < EaseTime * 2) {
                textEase += Engine.DeltaTime / EaseTime;
                if (closing)
                    yield break;
                yield return null;
            }
            textEase = 1f;

            while (timer < Duration - (EaseTime * 2))
                yield return null;
            Logger.Log(nameof(RainToolsModule), "starting close routine");
            yield return Close();
        }

        private IEnumerator Close() {
            if (!closing) {
                closing = true;
                timer = Duration - (EaseTime * 2);
                while (timer < Duration - EaseTime) {
                    textEase -= Engine.DeltaTime / EaseTime;
                    yield return null;
                }
                textEase = 0f;
                while (timer <= Duration) {
                    barEase -= Engine.DeltaTime / EaseTime;
                    yield return null;
                }
                barEase = 0f;
                RemoveSelf();
                Logger.Log(nameof(RainToolsModule), "removed self");
            }
        }
    }
}
