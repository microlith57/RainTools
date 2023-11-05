using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

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

            // hide if paused, the player is retrying, or if a cutscene is being skipped (idk why, might remove some of that)
            if (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene)
                return;

            float thicknessPercent = 0.05f; // this could be a controllable field
            float thickness = Engine.Height * thicknessPercent; // thickness in pixels
            float barHeight = (1f - (1f - barEase) * (1f - barEase)) * thicknessPercent * Engine.Height; // eased thickness
            float bottomBarY = Engine.Height - barHeight; // y position for the bottom bar

            Draw.Rect(0f, bottomBarY, Engine.Width, barHeight + 10f, Color.Black);
            Draw.Rect(0f, -10f, Engine.Width, barHeight + 10f, Color.Black);

            float fontHeight = ActiveFont.Measure(text).Y; // the height of the current font
            float textHeight = (thicknessPercent * Engine.Height - .25f * thickness) / fontHeight; // the height as a scalar that the text should have

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

            // reset timer and ease bars then text
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

            // wait to close it
            while (timer < Duration - (EaseTime * 2))
                yield return null;
            yield return Close();
        }

        private IEnumerator Close() {
            if (!closing) {
                closing = true;

                // make sure timer is at the appropriate time, then ease text then bars
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
            }
        }
    }
}
